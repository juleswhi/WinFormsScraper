global using static WinFormsScraper.ScrapeType;
using System.Collections.Immutable;

namespace WinFormsScraper;
public enum ScrapeType
{
    ALL,
    IMPORTANT,
    SIZE_AND_LOCATION,
    CUSTOM
}

public static class WinFormsScraperConfig
{
    public static string Path { get; set; } = "FormDetails.txt";

    public static ScrapeType Type { get; set; } = IMPORTANT;

    public static HashSet<string> ScrapeFilter { get; set; } = [];
}

public static class ScrapeSets
{

    public static readonly ImmutableHashSet<string> All =
        ImmutableHashSet.Create("Size", "Location", "ForeColour", "BackColour", "Font");
    
    public static readonly ImmutableHashSet<string> Important =
        ImmutableHashSet.Create("Size", "Location", "ForeColour", "BackColour", "Font");

    public static readonly ImmutableHashSet<string> SizeLocation =
        ImmutableHashSet.Create("Size", "Location", "ForeColour", "BackColour", "Font");

    public static readonly ImmutableHashSet<string> Font = 
        ImmutableHashSet.Create("Font");

    public static ImmutableHashSet<string> Custom => 
        WinFormsScraperConfig.ScrapeFilter.ToImmutableHashSet();
}