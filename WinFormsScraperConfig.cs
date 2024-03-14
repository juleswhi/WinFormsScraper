global using static WinFormsScraper.ScrapeType;
using System.Collections.Immutable;

namespace WinFormsScraper;

public static class WinFormsScraperConfig
{
    static WinFormsScraperConfig()
    {
        ScrapeFilter = ImmutableHashSet<string>.Empty;
    }
    public static string Path { get; set; } = "FormDetails.txt";

    public static ScrapeType Type { get; set; } = IMPORTANT;

    public static ImmutableHashSet<string> ScrapeFilter { get; set; } = ImmutableHashSet<string>.Empty;

    public static readonly Dictionary<ScrapeType, ImmutableHashSet<string>> ScrapeTypeToFilter = new()
    {
        { SIZE_AND_LOCATION, Size_And_Location_Filter! },
        { IMPORTANT, Important_Filter! }
    };

    private static readonly ImmutableHashSet<string> Size_And_Location_Filter =
        ImmutableHashSet.Create("Size", "Location");

    private static readonly ImmutableHashSet<string> Important_Filter =
        ImmutableHashSet.Create("Size", "Location", "ForeColour", "BackColour", "Font");
}
