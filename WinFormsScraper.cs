using System.Reflection;

namespace WinFormsScraper;

public static class WinFormsScraper
{
    public static string Path = "FormDetails.txt";
    private static List<string> s_printedForms = new();
    
    public static void Scrape<T>(T form) where T : Form
    {
        using StreamWriter streamWriter = new StreamWriter(Path, true);

        if (s_printedForms.Any(form => form.GetType() == typeof(T)))
            return;

        streamWriter.WriteLine($"-------------");
        streamWriter.WriteLine($"{typeof(T).Name}");
        streamWriter.WriteLine($"-------------");
        streamWriter.WriteLine($"");

        foreach(Control control in form.Controls)
        {
            streamWriter.WriteLine($"{control.Name}:");
            streamWriter.WriteLine($"\t\tSize: [{control.Size.Width}, {control.Size.Height}]");
            streamWriter.WriteLine($"\t\tLocation: [{control.Location.X}, {control.Location.Y}]");
            streamWriter.WriteLine($"\t\tForeColour: {control.ForeColor.Name}");
            streamWriter.WriteLine($"\t\tBackColour: {control.BackColor.Name}");
            streamWriter.WriteLine($"\t\tFont: {control.Font.Name}");
            streamWriter.WriteLine($"\t\tFontSize: {control.Font.Size}");
            streamWriter.WriteLine();
        }
        streamWriter.WriteLine();
        streamWriter.WriteLine();
    }
}
