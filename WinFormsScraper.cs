using System.Reflection;
using System.Collections.Immutable;
using System.Diagnostics;

using Config = WinFormsScraper.WinFormsScraperConfig;

namespace WinFormsScraper;

/// <summary>
/// Deals with scraping property data from Forms
/// </summary>
public static class WinFormsScraper
{
    /// <summary>
    /// Scrapes all forms based on a certain hashset
    /// </summary>
    /// <param name="hashset"></param>
    public static void Scrape(List<List<object>> p) 
    {
        if(p.Any(x => x.FirstOrDefault() is not string))
        {
            return;
        }

        Scrape(p, ALL, Assembly.GetCallingAssembly());
    }
    /// <summary>
    /// Scrapes every form in the assembly
    /// </summary>
    /// <param name="assembly">The assembly to check</param>
    /// <param name="scrapeType">What properties of control to print out</param>
    public static void Scrape(List<List<object>> obj_list, ScrapeType? scrapeType = null, Assembly? assembly = null)
    {
        scrapeType ??= Config.Type;
        assembly ??= Assembly.GetCallingAssembly();

        // Ensure the file exists
        if(!File.Exists(Config.Path))
        {
            File.Create(Config.Path);
        }

        // Clear the file
        File.WriteAllText(Config.Path, string.Empty);

        // Create the streamwriter
        using StreamWriter sw = new StreamWriter(Config.Path, true);

        // Check for null
        if(sw is null)
        {
            return;
        }

        // Get all types
        Type[] types = assembly.GetTypes();

        // Filter for forms
        Type form = typeof(Form);
        List<Type> forms = types.Where(x => x.IsSubclassOf(form) || x == form).ToList();

        foreach(Type type in forms)
        {

            var form_params = obj_list.FirstOrDefault(x => (x.First() as string) == type.Name);

            form_params?.RemoveAt(0);

            if(form_params is null)
            {
                form_params = [];
            }

            // Create the instance of the form
            var instance = Activator.CreateInstance(type, form_params);

            // The name of the form
            sw!.WriteLine($"--- {type.Name} ---");
            // Extra line for clarity
            sw!.WriteLine();

            // Find the control property
            var controlProp = type.GetProperty("Controls");

            // Check if the form has the control property ( logically should always )
            if(controlProp is null)
            {
                continue;
            }

            // Take the controls from the property;
            Control.ControlCollection? controls = controlProp.GetValue(instance) as Control.ControlCollection;

            // Make sure the collection exists
            if(controls is null)
            {
                continue;
            }

            // Check what Information should be printed
            ImmutableHashSet<string> filter = scrapeType switch
            {
                ALL => ScrapeSets.All,
                IMPORTANT => ScrapeSets.Important,
                SIZE_AND_LOCATION => ScrapeSets.SizeLocation,
                CUSTOM => ScrapeSets.Custom,
                _ => ImmutableHashSet<string>.Empty
            };

            if(filter is null)
            {
                continue;
            }

            // Iterate through all of the controls
            foreach (Control control in controls)
            {
                sw!.WriteLine($"{control.Name}:");

                // Write all the properties
                foreach(var str in GetInformation(control, filter))
                {
                    sw.WriteLine(str);
                }

                // Line for clarity
                sw!.WriteLine();
            }
            sw!.WriteLine();
        }

        // Collect all that garbage from potentially large amount of objects created
        GC.Collect();
    }

    private static IEnumerable<string> GetInformation<T>(T control, ImmutableHashSet<string> filter) where T : Control
    {
        var props = control.GetType().GetProperties();

        foreach (var prop in props)
        {
            if (filter is null || !filter.Contains(prop.Name))
            {
                continue;
            }

            string str;
            try
            {
                // Grab the value of the property
                str = $"\t\t{prop.Name}: {PrettyPrint(prop.GetMethod?.Invoke(control, new object[] { }))}";
            }
            catch (Exception)
            {
                // Some weird property values may encounter this exception
                // In that case, just ignore it
                continue;
            }

            yield return str;
        }
    }

    private static string? PrettyPrint(object? obj)
    {
        if(obj is Font font)
        {
            return $"{font.Name} - {font.Size}px";
        }

        if(obj is Point point)
        {
            return $"[{point.X}, {point.Y}]";
        }

        if(obj is Size size)
        {
            return $"[{size.Width}, {size.Height}]";
        }

        return (string?)obj;
    } 
}

