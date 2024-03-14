using Config = WinFormsScraper.WinFormsScraperConfig;
using System.Reflection;
using System.Collections.Immutable;
using System.Diagnostics;

namespace WinFormsScraper;

public enum ScrapeType
{
    ALL,
    IMPORTANT,
    SIZE_AND_LOCATION
}

/// <summary>
/// Deals with scraping property data from Forms
/// </summary>
public static class WinFormsScraper
{
    /// <summary>
    /// A Hashset of all forms previously visited
    /// </summary>
    private static HashSet<string> s_printedForms = new();

    /// <summary>
    /// Scrapes everyform in the assembly
    /// </summary>
    /// <param name="assembly">The assembly to check</param>
    /// <param name="scrapeType">What properties of control to print out</param>
    public static void ScrapeAll(Assembly? assembly = null, ScrapeType? scrapeType = null)
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
            // Create the instance of the form
            var instance = Activator.CreateInstance(type);

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
                ALL => ImmutableHashSet<string>.Empty,
                ScrapeType scrape => Config.ScrapeTypeToFilter[scrape]
            };

            if(filter is null)
            {
                continue;
            }

            // Iterate through all of the controls
            foreach(Control control in controls)
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

        foreach(var prop in props)
        {
            if(filter is null)
            {
                Debug.Print($"Filter is null");
                continue;
            }

            if(filter.Count != 0 && !filter.Contains(prop.Name))
            {
                continue;
            }


            string str;
            try {
                // Grab the value of the property
                str = $"\t\t{prop.Name}: {prop.GetMethod?.Invoke(control, new object[] { })}";
            }
            catch(Exception)
            {
                // Some weird property values may encounter this exception
                // In that case, just ignore it
                continue;
            }

            yield return str;
        }
    }

}

