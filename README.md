# Winforms Form Scraper

 A collection of classes that uses reflection to print out property information about forms.

 ## Installation

 Dotnet CLI:
 ```sh
dotnet add package WinFormsScraper --version 1.3.0
```

NuGet Command:
```sh
NuGet\Install-Package WinFormsScraper -Version 1.3.0
```

Or alternatively, through Visual Studio, navigate to: 
    Tools -> NuGet Package Manager -> Manage NuGet Packages For Solution -> Browse -> Search for `Winforms Scraper`

  
## Usage

The scraping of the forms can be done by calling the `Scrape` method on the `WinFormsScraper` class.
This will output information about your form in the [Output File](#Output)

*_Note: Please remove the scraping code from your project once you have used it._*



Example:

 ```cs
WinFormsScraper.WinFormsScraper.Scrape();
```

### <a name="Output"></a>Output File

The output file will default to `FormDetails.txt` in your bin/Debug/netX.X-windows folder.

To customise the location of this file, simply change the property, `Path` in the `WinFormsScraperConfig` class.

```cs
WinFormsScraper.WinFormsScraperConfig.Path = "Custom_Path.txt";
```


### <a name="Filters"></a>Filters

You can filter what properties are printed by using a custom Hashset with the names of the properties you want.

You can pass them into the `Scrape` method like this:

```cs
WinFormsScraper.WinFormsScraper.Scrape(["Font", "Location"]);
```

Alternatively, You can modify the `ScrapeFilter` property in the `WinFormsScraperConfig` class to define your own Hashset.

```cs
WinFormsScraper.WinFormsScraperConfig.ScrapeFilter = [ "Size", "DockType" ];

WinFormsScraper.WinFormsScraper.Scrape(WinFormsScraper.ScrapeType.Custom);
```

Finally, If you don't want to define your own Hashsets, you can use the range of prebuilt Hashsets by passing a `ScrapeType` through to the `Scrape` function.

The following code snippet will only print out the `Size` and `Location` properties.

```cs
WinFormsScraper.WinFormsScraper.Scrape(WinFormsScraper.ScrapeType.SIZE_AND_LOCATION);
```

