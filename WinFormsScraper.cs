using System.Reflection;

namespace WinFormsScraper;

public enum ScrapeType
{
    IMPORTANT,
    // ALL
}

/// <summary>
/// Deals with scraping property data from Forms
/// </summary>
public static class WinFormsScraper
{
    public static string Path = "FormDetails.txt";

    public static ScrapeType Type = ScrapeType.IMPORTANT;
    
    private static HashSet<string> s_printedForms = new();
    
    /// <summary>
    /// Pretty-Prints all the scraped data to file
    /// </summary>
    /// <typeparam name="T">The Type of form</typeparam>
    /// <param name="form">The Form to scrape</param>
    public static void Scrape<T>(T form) where T : Form
    {
        if(!File.Exists(Path))
        {
            File.Create(Path);
        }
        using StreamWriter streamWriter = new StreamWriter(Path, true);

        // Could potentially update the value here
        if (s_printedForms.Any(formName => formName == typeof(T).Name))
            return;

        s_printedForms.Add(typeof(T).Name);

        streamWriter?.WriteLine($"--- {typeof(T).Name} ---");
        streamWriter?.WriteLine($"");

        foreach(Control control in form.Controls)
        {
            streamWriter?.WriteLine($"{control.Name}:");
            streamWriter?.WriteLine(PrettyPrint("Size", control.Size));
            streamWriter?.WriteLine(PrettyPrint("Location", control.Location));
            streamWriter?.WriteLine(PrettyPrint("ForeColour", control.ForeColor));
            streamWriter?.WriteLine(PrettyPrint("BackColour", control.BackColor));
            streamWriter?.WriteLine(PrettyPrint("Font", control.Font));

            streamWriter?.WriteLine();
        }
        streamWriter?.WriteLine();
    }

    private static string FormatSize(Size size) => $"[{size.Width}, {size.Height}]";
    private static string FormatLocation(Point location) => $"[{location.X}, {location.Y}]";

    private static string PrettyPrint(string name, object value)
    {
        value = name switch
        {
            "Size" => FormatSize((Size)value),
            "Location" => FormatLocation((Point)value),
            "Font" => $"{((Font)value).Name}\n\t\tFont Size: {((Font)value).Size}",
            _ => value
        };

        return $"\t\t{name}: {value}";
    }
}

