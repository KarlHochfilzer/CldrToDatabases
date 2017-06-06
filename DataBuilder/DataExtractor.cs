using CldrToDatabases.DBModels;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CldrToDatabases.DataBuilder
{
    public class DataExtractor
    {
        private readonly string _DataPath;
        private readonly string _CldrPath;
        private readonly string _NzdPath;

        private TzdbDateTimeZoneSource _tzdbSource;
        private IDateTimeZoneProvider _tzdbProvider;

        public List<Languages> LanguagesList = new List<Languages>();
        public List<Scripts> ScriptsList = new List<Scripts>();
        public List<Territories> TerritoriesList = new List<Territories>();
        public List<Currencies> CurrenciesList = new List<Currencies>();
        public List<TimeZones> TimeZonesList = new List<TimeZones>();
        public List<MetaZones> MetaZonesList = new List<MetaZones>();
        public List<WindowsZones> WindowsZonesList = new List<WindowsZones>();

        public List<LanguageNames> LanguageNamesList = new List<LanguageNames>();
        public List<ScriptNames> ScriptNamesList = new List<ScriptNames>();
        public List<TerritoryNames> TerritoryNamesList = new List<TerritoryNames>();
        public List<CurrencyNames> CurrencyNamesList = new List<CurrencyNames>();
        public List<TimeZoneNames> TimeZoneNamesList = new List<TimeZoneNames>();
        public List<MetaZoneNames> MetaZoneNamesList = new List<MetaZoneNames>();
        // No Locale data for WindowsZones available.
        // WindowsZones.Code == System.TimeZoneInfo.Id.
        // System.TimeZoneInfo.DisplayName depends on installed System language.
        // Microsoft is not interested in giving any translation without installing System language data.
        // That's not usable for all web sites.

        private DataExtractor(string dataPath)
        {
            _DataPath = dataPath + "\\";
            _CldrPath = Path.Combine(dataPath, "cldr") + "\\";
            _NzdPath = Path.Combine(dataPath, "nzd") + "\\";
        }

        public static DataExtractor Load(string dataPath, bool downloadData)
        {
            var data = new DataExtractor(dataPath);

            if (downloadData || !Directory.Exists(dataPath))
            {
                data.DownloadData();
            }
            data.LoadData();

            return data;
        }

        private void DownloadData()
        {
            if (Directory.Exists(_CldrPath))
                Directory.Delete(_CldrPath, true);

            if (Directory.Exists(_NzdPath))
                Directory.Delete(_NzdPath, true);

            Task.WaitAll(
                Downloader.DownloadCldrAsync(_CldrPath),
                Downloader.DownloadNzdAsync(_NzdPath));
        }

        private void LoadData()
        {
            // init noda time
            using (var stream = File.OpenRead(Directory.GetFiles(_NzdPath)[0]))
            {
                _tzdbSource = TzdbDateTimeZoneSource.FromStream(stream);
                _tzdbProvider = new DateTimeZoneCache(_tzdbSource);
            }

            // "UTC" is Not in CLDR! But "Etc/GMT" can be used instead.

            Console.WriteLine("Start load base-data.");
            LoadBaseData();

            Console.WriteLine("Start load locale-data.");
            LoadLanguagesData();

            Console.WriteLine("Start merge locale-data.");
            MergeLanguagesDataLists();

            // Update IdStatus
            // From: _cldrPath + @"common\validity\*.xml" (region.xml, currency.xml, script.xml, language.xml, subdivision.xml)

            Console.WriteLine("Start update with validity-data.");
            UpdateFromValidityData();

            // Update supplementals
            // from _cldrPath + @"common\supplemental\supplementalData.xml"
            // and from _cldrPath + @"common\supplemental\subdivisions.xml"

            Console.WriteLine("Start update with supplemental-data.");
            UpdateFromSupplementalData();
        }

        private void LoadBaseData()
        {
            // Load Languages, Scripts, Territories, MetaZones
            using (var stream = File.OpenRead(_CldrPath + @"common\main\en.xml"))
            {
                var doc = XDocument.Load(stream);

                // Languages: "/ldml/localeDisplayNames/languages"
                var languagesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/languages");
                if (languagesElement != null)
                {
                    AddLanguageEntries(languagesElement);
                }

                // Scripts: "/ldml/localeDisplayNames/scripts"
                var scriptsElement = doc.XPathSelectElement("/ldml/localeDisplayNames/scripts");
                if (scriptsElement != null)
                {
                    AddScriptEntries(scriptsElement);
                }

                // Territories: "/ldml/localeDisplayNames/territories"
                var territoriesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/territories");
                if (territoriesElement != null)
                {
                    AddTerritoryEntries(territoriesElement);
                }

                // MetaZones: "/ldml/dates/timeZoneNames" (Elements("metazone"))
                var metaZonesElement = doc.XPathSelectElement("/ldml/dates/timeZoneNames");
                if (metaZonesElement != null)
                {
                    AddMetaZoneEntries(metaZonesElement);
                }
            }
            // Load Currencies
            using (var stream = File.OpenRead(_CldrPath + @"common\bcp47\currency.xml"))
            {
                var doc = XDocument.Load(stream);

                // Currencies: "/ldmlBCP47/keyword" <key name="cu"...> (Elements("type"))
                var currenciesElement = doc.XPathSelectElement("/ldmlBCP47/keyword/key[@name='cu']");
                if (currenciesElement != null)
                {
                    AddCurrencyEntries(currenciesElement);
                }
            }
            // Load TimeZones
            using (var stream = File.OpenRead(_CldrPath + @"common\bcp47\timezone.xml"))
            {
                var doc = XDocument.Load(stream);

                // TimeZones: "/ldmlBCP47/keyword/key" (Elements("type"))
                var timeZonesElement = doc.XPathSelectElement("/ldmlBCP47/keyword/key");
                if (timeZonesElement != null)
                {
                    AddTimeZoneEntries(timeZonesElement);
                }
            }
            // Load Territory subdivisions
            using (var stream = File.OpenRead(_CldrPath + @"common\subdivisions\en.xml"))
            {
                var doc = XDocument.Load(stream);

                // Territory subdivisions: "/ldml/localeDisplayNames/subdivisions"
                var subdivisionsElement = doc.XPathSelectElement("/ldml/localeDisplayNames/subdivisions");
                if (subdivisionsElement != null)
                {
                    AddTerritorySubdivisionEntries(subdivisionsElement);
                }
            }
            // Load WindowsZones
            using (var stream = File.OpenRead(_CldrPath + @"common\supplemental\windowsZones.xml"))
            {
                var doc = XDocument.Load(stream);

                // WindowsZones: "/supplementalData/windowsZones/mapTimezones"
                var windowsZonesElement = doc.XPathSelectElement("/supplementalData/windowsZones/mapTimezones");
                if (windowsZonesElement != null)
                {
                    AddWindowsZoneEntries(windowsZonesElement);
                }
            }
        }

        private void AddLanguageEntries(XContainer languagesElement)
        {
            var languages = languagesElement.Elements("language");

            foreach (var lang in languages)
            {
                string code = lang.Attribute("type").Value.Replace("_", "-");
                Languages l = new Languages(code);

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = LanguagesList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        LanguagesList.Add(l);
                    }
                }
            }
        }

        private void AddScriptEntries(XContainer scriptsElement)
        {
            var scripts = scriptsElement.Elements("script");

            foreach (var scri in scripts)
            {
                string code = scri.Attribute("type").Value;
                Scripts s = new Scripts(code);

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = ScriptsList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        ScriptsList.Add(s);
                    }
                }
            }
        }

        private void AddTerritoryEntries(XContainer territoriesElement)
        {
            var territories = territoriesElement.Elements("territory");

            foreach (var terr in territories)
            {
                string code = terr.Attribute("type").Value;
                Territories t = new Territories(code);

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = TerritoriesList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        TerritoriesList.Add(t);
                    }
                }
            }
        }

        private void AddCurrencyEntries(XContainer currenciesElement)
        {
            var currencies = currenciesElement.Elements("type");

            foreach (var curr in currencies)
            {
                string code = curr.Attribute("name").Value.ToUpper();
                Currencies c = new Currencies(code);

                var description = curr.Attribute("description");
                c.Description = description != null ? description.Value : string.Empty;
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = CurrenciesList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        CurrenciesList.Add(c);
                    }
                }
            }
        }

        private void AddMetaZoneEntries(XContainer metaZonesElement)
        {
            var metaZ = metaZonesElement.Elements("metazone");

            foreach (var meta in metaZ)
            {
                string code = meta.Attribute("type").Value;
                MetaZones m = new MetaZones(code);

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = MetaZonesList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        MetaZonesList.Add(m);
                    }
                }
            }
        }

        private void AddTimeZoneEntries(XContainer timeZonesElement)
        {
            var timeZ = timeZonesElement.Elements("type");
            Instant Now = SystemClock.Instance.GetCurrentInstant();

            foreach (var tz in timeZ)
            {
                var code = tz.Attribute("alias");
                var description = tz.Attribute("description");
                if (code != null)
                {
                    // slit Value by Space
                    string[] insertTz = SplitDataToArray(code.Value);
                    foreach (var itz in insertTz)
                    {
                        TimeZones t = new TimeZones(itz);

                        t.Description = description != null ? description.Value : string.Empty;
                        bool hasProvider = _tzdbProvider.Ids.Where(x => x == itz).Any();
                        if (hasProvider)
                            t.StandardOffset = _tzdbProvider[itz].GetZoneInterval(Now).StandardOffset.Seconds;
                        var containList = TimeZonesList.Where(u => itz.Equals(u.Code));
                        if (!containList.Any())
                        {
                            TimeZonesList.Add(t);
                        }
                    }
                }
            }
        }

        private void AddTerritorySubdivisionEntries(XContainer subdivisionsElement)
        {
            var subdivitions = subdivisionsElement.Elements("subdivision");

            foreach (var subd in subdivitions)
            {
                string code = subd.Attribute("type").Value;
                Territories s = new Territories(code);

                s.Type = "subdivision";
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var containList = TerritoriesList.Where(u => code.Equals(u.Code));
                    if (!containList.Any())
                    {
                        TerritoriesList.Add(s);
                    }
                }
            }
        }

        private void AddWindowsZoneEntries(XContainer windowsZonesElement)
        {
            var windowsZones = windowsZonesElement.Elements("mapZone");

            foreach (var winz in windowsZones)
            {
                string codes = winz.Attribute("type").Value;
                // Split attribute 'type' by Space
                string[] tzCodes = SplitDataToArray(codes);
                string winZoneCode = winz.Attribute("other").Value;
                string territoryCode = winz.Attribute("territory").Value;
                foreach (var tzCode in tzCodes)
                {
                    WindowsZones w = new WindowsZones(winZoneCode, tzCode, territoryCode);

                    WindowsZonesList.Add(w);
                }
            }
        }

        private void LoadLanguagesData()
        {
            var mainLanguages = Directory.GetFiles(_CldrPath + @"common\main")
                .Select(Path.GetFileName)
                .Select(x => x.Substring(0, x.Length - 4));

            foreach (var language in mainLanguages)
            {
                LoadMainLanguagesData(language);
            }

            var subdivisionLanguages = Directory.GetFiles(_CldrPath + @"common\subdivisions")
                .Select(Path.GetFileName)
                .Select(x => x.Substring(0, x.Length - 4));

            foreach (var language in subdivisionLanguages)
            {
                LoadSubdivisionLanguagesData(language);
            }
        }

        private void LoadMainLanguagesData(string language)
        {
            // Load Languages, Scripts, Territories, Currencies, TimeZoneNames, MetaZoneNames
            using (var stream = File.OpenRead(_CldrPath + @"common\main\" + language + ".xml"))
            {
                var doc = XDocument.Load(stream);
                string lang = language.Replace("_", "-");

                // Languages: "/ldml/localeDisplayNames/languages"
                var languageNamesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/languages");
                if (languageNamesElement != null)
                {
                    AddLanguageNameEntries(languageNamesElement, lang);
                }

                // Scripts: "/ldml/localeDisplayNames/scripts"
                var scriptNamesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/scripts");
                if (scriptNamesElement != null)
                {
                    AddScriptNameEntries(scriptNamesElement, lang);
                }

                // Territories: "/ldml/localeDisplayNames/territories"
                var territoryNamesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/territories");
                if (territoryNamesElement != null)
                {
                    AddTerritoryNameEntries(territoryNamesElement, lang);
                }

                // Currencies: "/ldml/numbers/currencies"
                var currencyNamesElement = doc.XPathSelectElement("/ldml/numbers/currencies");
                if (currencyNamesElement != null)
                {
                    AddCurrencyNameEntries(currencyNamesElement, lang);
                }

                // TimeZoneNames: "/ldml/dates/timeZoneNames" (Elements("zone"))
                var timeZoneNamesElement = doc.XPathSelectElement("/ldml/dates/timeZoneNames");
                if (timeZoneNamesElement != null)
                {
                    AddTimeZoneNameEntries(timeZoneNamesElement, lang);
                }

                // MetaZoneNames: "/ldml/dates/timeZoneNames" (Elements("metazone"))
                var metaZoneNamesElement = doc.XPathSelectElement("/ldml/dates/timeZoneNames");
                if (metaZoneNamesElement != null)
                {
                    AddMetaZoneNameEntries(metaZoneNamesElement, lang);
                }
            }
        }

        private void LoadSubdivisionLanguagesData(string language)
        {
            // Load Territory subdivisions
            using (var stream = File.OpenRead(_CldrPath + @"common\subdivisions\" + language + ".xml"))
            {
                var doc = XDocument.Load(stream);
                string lang = language.Replace("_", "-");

                // Territory subdivisions: "/ldml/localeDisplayNames/subdivisions"
                var subdivisionNamesElement = doc.XPathSelectElement("/ldml/localeDisplayNames/subdivisions");
                if (subdivisionNamesElement != null)
                {
                    AddTerritorySubdivisionNameEntries(subdivisionNamesElement, lang);
                }
            }
        }

        private void AddLanguageNameEntries(XContainer languageNamesElement, string language)
        {
            var languages = languageNamesElement.Elements("language");

            // Insert all elements, no matter if "Code, Locale" still exists!
            // To check here would extremly slow down this function.
            // Duplicates can be merged later. (MergeLanguageNamesList())
            foreach (var lang in languages)
            {
                string code = lang.Attribute("type").Value.Replace("_", "-");

                LanguageNames langName = new LanguageNames(code, language);
                // e.g. alt="short", alt="long", alt="variant", alt="secondary"
                if (lang.Attribute("alt") != null)
                {
                    string altType = lang.Attribute("alt").Value;
                    if (altType == "short")
                        langName.ShortName = lang.Value;
                    else if (altType == "long")
                        langName.LongName = lang.Value;
                    else if (altType == "variant")
                        langName.VariantName = lang.Value;
                    else if (altType == "secondary")
                        langName.SecondaryName = lang.Value;
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("*** ERROR-AddLanguageNameEntries ***:\r\nAlt-Type: '{0}' not included!", altType);
                        Console.WriteLine();
                    }
                }
                else
                {
                    langName.Name = lang.Value;
                }

                LanguageNamesList.Add(langName);
            }
        }

        private void AddScriptNameEntries(XContainer scriptNamesElement, string language)
        {
            var scripts = scriptNamesElement.Elements("script");

            // Insert all elements, no matter if "Code, Locale" still exists!
            // To check here would extremly slow down this function.
            // Duplicates can be merged later. (MergeScriptNamesList())
            foreach (var scri in scripts)
            {
                string code = scri.Attribute("type").Value;

                ScriptNames scriName = new ScriptNames(code, language);
                // e.g. alt="short", alt="stand-alone", alt="variant", alt="secondary"
                if (scri.Attribute("alt") != null)
                {
                    string altType = scri.Attribute("alt").Value;
                    if (altType == "short")
                        scriName.ShortName = scri.Value;
                    else if (altType == "stand-alone")
                        scriName.StandAloneName = scri.Value;
                    else if (altType == "variant")
                        scriName.VariantName = scri.Value;
                    else if (altType == "secondary")
                        scriName.SecondaryName = scri.Value;
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("*** ERROR-AddScriptNameEntries ***:\r\nAlt-Type: '{0}' not included!", altType);
                        Console.WriteLine();
                    }
                }
                else
                {
                    scriName.Name = scri.Value;
                }

                ScriptNamesList.Add(scriName);
            }
        }

        private void AddTerritoryNameEntries(XContainer territoryNamesElement, string language)
        {
            var territories = territoryNamesElement.Elements("territory");

            // Insert all elements, no matter if "Code, Locale" still exists!
            // To check here would extremly slow down this function.
            // Duplicates can be merged later. (MergeTerritoryNamesList())
            foreach (var terr in territories)
            {
                string code = terr.Attribute("type").Value;

                TerritoryNames terrName = new TerritoryNames(code, language);
                // e.g. alt="short", alt="variant"
                if (terr.Attribute("alt") != null)
                {
                    string altType = terr.Attribute("alt").Value;
                    if (altType == "short")
                        terrName.ShortName = terr.Value;
                    else if (altType == "variant")
                        terrName.VariantName = terr.Value;
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("*** ERROR-AddTerritoryNameEntries ***:\r\nAlt-Type: '{0}' not included!", altType);
                        Console.WriteLine();
                    }
                }
                else
                {
                    terrName.Name = terr.Value;
                }

                TerritoryNamesList.Add(terrName);
            }
        }

        private void AddCurrencyNameEntries(XContainer currencyNamesElement, string language)
        {
            var currencies = currencyNamesElement.Elements("currency");

            foreach (var curr in currencies)
            {
                string code = curr.Attribute("type").Value;
                CurrencyNames c = new CurrencyNames(code, language);

                var name = curr.Elements("displayName").Where(x => x.Attribute("count") == null);
                c.Name = name.Any() == true ? name.First().Value : string.Empty;
                var sym = curr.Elements("symbol").Where(x => x.Attribute("alt") == null);
                c.Symbol = sym.Any() == true ? sym.First().Value : string.Empty;
                var symNarrow = curr.Elements("symbol").Where(x => x.Attribute("alt") != null).Where(x => x.Attribute("alt").Value == "narrow");
                var symN = symNarrow.Any() == true ? symNarrow.Where(x => x.Attribute("alt").Value == "narrow") : null;
                c.SymbolNarrow = symN != null ? symN.First().Value : string.Empty;
                // Some languages do not have a 'displayName' element without Attribute "count"
                // Currencies.Description needed as fallback if at least no CurrencyNames.Locale 'en' entry exists
                // insert and use Description from Currencies
                if (!string.IsNullOrWhiteSpace(code))
                    CurrencyNamesList.Add(c);
            }
        }

        private void AddTimeZoneNameEntries(XContainer timeZoneNamesElement, string language)
        {
            // Names for UTC - Not in CLDR!
            // !! Name "UTC" and"Etc/GMT" are not stored, but Name "Etc/UTC"
            var tzNames = timeZoneNamesElement.Elements("zone");

            foreach (var tzna in tzNames)
            {
                string code = tzna.Attribute("type").Value;
                TimeZoneNames t = new TimeZoneNames(code, language);

                var longElem = tzna.Elements("long");
                if (longElem.Any())
                {
                    var standard = longElem.First().Elements("standard");
                    t.Name = standard.Any() == true ? standard.First().Value : string.Empty;
                }
                else
                    t.Name = string.Empty;
                var city = tzna.Elements("exemplarCity");
                t.City = city.Any() == true ? city.First().Value : string.Empty;
                if (!string.IsNullOrWhiteSpace(code))
                    TimeZoneNamesList.Add(t);
            }
        }

        private void AddMetaZoneNameEntries(XContainer metaZoneNamesElement, string language)
        {
            var metaNames = metaZoneNamesElement.Elements("metazone");

            foreach (var meta in metaNames)
            {
                string code = meta.Attribute("type").Value;
                MetaZoneNames m = new MetaZoneNames(code, language);

                var longElem = meta.Elements("long");
                if (longElem.Any())
                {
                    var generic = meta.Elements("long").First().Elements("generic");
                    var standard = meta.Elements("long").First().Elements("standard");
                    m.Name = generic.Any() == true ? generic.First().Value : standard.First().Value;
                }
                else
                    m.Name = string.Empty;
                var shortElem = meta.Elements("short");
                if (shortElem.Any())
                {
                    var shortGeneric = meta.Elements("short").First().Elements("generic");
                    var shortStandard = meta.Elements("short").First().Elements("standard");
                    m.ShortName = shortGeneric.Any() == true ? shortGeneric.First().Value : shortStandard.First().Value;
                }
                else
                    m.ShortName = string.Empty;
                if (!string.IsNullOrWhiteSpace(code))
                    MetaZoneNamesList.Add(m);
            }
        }

        private void AddTerritorySubdivisionNameEntries(XContainer subdivisionNamesElement, string language)
        {
            var subdivitions = subdivisionNamesElement.Elements("subdivision");

            foreach (var subd in subdivitions)
            {
                string code = subd.Attribute("type").Value;
                TerritoryNames s = new TerritoryNames(code, language);
                s.Name = subd.Value;
                s.Type = "subdivision";
                TerritoryNamesList.Add(s);
            }
        }

        private void MergeLanguagesDataLists()
        {
            MergeLanguageNamesList();
            MergeScriptNamesList();
            MergeTerritoryNamesList();
        }

        private void MergeLanguageNamesList()
        {
            var duplicateLanguageNames = LanguageNamesList.GroupBy(g => new { g.Locale, g.Code })
                    .Where(c => c.Count() > 1)
                    .Select(s => new
                    {
                        MostRecent = s.FirstOrDefault(),
                        Others = s.Skip(1).ToList()
                });

            foreach (var item in duplicateLanguageNames)
            {
                LanguageNames mostRecent = item.MostRecent;
                List<LanguageNames> others = item.Others;
                foreach (var other in others)
                {
                    if (string.IsNullOrWhiteSpace(mostRecent.Name))
                        mostRecent.Name = other.Name;
                    if (string.IsNullOrWhiteSpace(mostRecent.ShortName))
                        mostRecent.ShortName = other.ShortName;
                    if (string.IsNullOrWhiteSpace(mostRecent.LongName))
                        mostRecent.LongName = other.LongName;
                    if (string.IsNullOrWhiteSpace(mostRecent.VariantName))
                        mostRecent.VariantName = other.VariantName;
                    if (string.IsNullOrWhiteSpace(mostRecent.SecondaryName))
                        mostRecent.SecondaryName = other.SecondaryName;
                    LanguageNamesList.Remove(other);
                }
            }
        }

        private void MergeScriptNamesList()
        {
            var duplicateScriptNames = ScriptNamesList.GroupBy(g => new { g.Locale, g.Code })
                    .Where(c => c.Count() > 1)
                    .Select(s => new
                    {
                        MostRecent = s.FirstOrDefault(),
                        Others = s.Skip(1).ToList()
                    });

            foreach (var item in duplicateScriptNames)
            {
                ScriptNames mostRecent = item.MostRecent;
                List<ScriptNames> others = item.Others;
                foreach (var other in others)
                {
                    if (string.IsNullOrWhiteSpace(mostRecent.Name))
                        mostRecent.Name = other.Name;
                    if (string.IsNullOrWhiteSpace(mostRecent.ShortName))
                        mostRecent.ShortName = other.ShortName;
                    if (string.IsNullOrWhiteSpace(mostRecent.StandAloneName))
                        mostRecent.StandAloneName = other.StandAloneName;
                    if (string.IsNullOrWhiteSpace(mostRecent.VariantName))
                        mostRecent.VariantName = other.VariantName;
                    if (string.IsNullOrWhiteSpace(mostRecent.SecondaryName))
                        mostRecent.SecondaryName = other.SecondaryName;
                    ScriptNamesList.Remove(other);
                }
            }
        }

        private void MergeTerritoryNamesList()
        {
            var duplicateTerritoryNames = TerritoryNamesList.GroupBy(g => new { g.Locale, g.Code })
                    .Where(c => c.Count() > 1)
                    .Select(s => new
                    {
                        MostRecent = s.FirstOrDefault(),
                        Others = s.Skip(1).ToList()
                    });

            foreach (var item in duplicateTerritoryNames)
            {
                TerritoryNames mostRecent = item.MostRecent;
                List<TerritoryNames> others = item.Others;
                foreach (var other in others)
                {
                    if (string.IsNullOrWhiteSpace(mostRecent.Name))
                        mostRecent.Name = other.Name;
                    if (string.IsNullOrWhiteSpace(mostRecent.ShortName))
                        mostRecent.ShortName = other.ShortName;
                    if (string.IsNullOrWhiteSpace(mostRecent.VariantName))
                        mostRecent.VariantName = other.VariantName;
                    TerritoryNamesList.Remove(other);
                }
            }
        }

        private void UpdateFromValidityData()
        {
            // Update Languages, Scripts, Territories, Currencies, Subdivisions validities

            // Update Languages validities
            using (var stream = File.OpenRead(_CldrPath + @"common\validity\language.xml"))
            {
                var doc = XDocument.Load(stream);

                // Languages: "/supplementalData/idValidity"
                var languagesElement = doc.XPathSelectElement("/supplementalData/idValidity");
                if (languagesElement != null)
                {
                    UpdateLanguagesValidity(languagesElement);
                }
            }
            // Update Scripts validities
            using (var stream = File.OpenRead(_CldrPath + @"common\validity\script.xml"))
            {
                var doc = XDocument.Load(stream);

                // Scripts: "/supplementalData/idValidity"
                var scriptsElement = doc.XPathSelectElement("/supplementalData/idValidity");
                if (scriptsElement != null)
                {
                    UpdateScriptsValidity(scriptsElement);
                }
            }
            // Update Territories validities
            using (var stream = File.OpenRead(_CldrPath + @"common\validity\region.xml"))
            {
                var doc = XDocument.Load(stream);

                // Territories: "/supplementalData/idValidity"
                var territoriesElement = doc.XPathSelectElement("/supplementalData/idValidity");
                if (territoriesElement != null)
                {
                    UpdateTerritoriesValidity(territoriesElement);
                }
            }
            // Update Currencies validities
            using (var stream = File.OpenRead(_CldrPath + @"common\validity\currency.xml"))
            {
                var doc = XDocument.Load(stream);

                // Currencies: "/supplementalData/idValidity"
                var currenciesElement = doc.XPathSelectElement("/supplementalData/idValidity");
                if (currenciesElement != null)
                {
                    UpdateCurrenciesValidity(currenciesElement);
                }
            }
            // Update Subdivisions validities
            using (var stream = File.OpenRead(_CldrPath + @"common\validity\subdivision.xml"))
            {
                var doc = XDocument.Load(stream);

                // Subdivisions: "/supplementalData/idValidity"
                var subdivisionsElement = doc.XPathSelectElement("/supplementalData/idValidity");
                if (subdivisionsElement != null)
                {
                    UpdateSubdivisionsValidity(subdivisionsElement);
                }
            }
        }

        private void UpdateLanguagesValidity(XContainer languagesElement)
        {
            // from each like <id type="language" idStatus="..."> regular, special, deprecated, private, unknown
            var languages = languagesElement.Elements("id").Where(x => x.Attribute("type").Value == "language");

            foreach (var lang in languages)
            {
                // split Value by Space
                string[] updateLn = GenerateCodeArray(lang);
                // Update each LanguagesList by Code set IdStatus from Attribute idStatus
                // LanguagesList
                string status = lang.Attribute("idStatus").Value;
                var containList = LanguagesList.Where(u => updateLn.Contains(u.Code));
                if (containList.Any())
                {
                    foreach (var ln in containList)
                    {
                        ln.IdStatus = status;
                    }
                }
                // Status is not available in AddLanguageEntries(..)
                // Now remove "deprecated", "private_use" languages
                int removeCountDepr = LanguagesList.RemoveAll(u => u.IdStatus == "deprecated");
                int removeCountPriv = LanguagesList.RemoveAll(u => u.IdStatus == "private_use");
                // Remove all from LanguageNamesList not in LanguagesList
                int removeCountNames = LanguageNamesList.RemoveAll(item => !LanguagesList.Select(s => s.Code).Contains(item.Code));
            }
        }

        private void UpdateScriptsValidity(XContainer scriptsElement)
        {
            // from each like <id type="script" idStatus="..."> regular, special, deprecated, private, unknown
            var scripts = scriptsElement.Elements("id").Where(x => x.Attribute("type").Value == "script");

            foreach (var scri in scripts)
            {
                // slit Value by Space
                string[] updateLn = GenerateCodeArray(scri);
                // Update each ScriptsList by Code set IdStatus from Attribute idStatus
                //ScriptsList
                string status = scri.Attribute("idStatus").Value;
                var containList = ScriptsList.Where(u => updateLn.Contains(u.Code));
                if (containList.Any())
                {
                    foreach (var sc in containList)
                    {
                        sc.IdStatus = status;
                    }
                }
                // Status is not available in AddScriptEntries(..)
                // Now remove "deprecated", "private_use" scripts
                int removeCountDepr = ScriptsList.RemoveAll(u => u.IdStatus == "deprecated");
                int removeCountPriv = ScriptsList.RemoveAll(u => u.IdStatus == "private_use");
                // Remove all from ScriptNamesList not in ScriptsList
                int removeCountNames = ScriptNamesList.RemoveAll(item => !ScriptsList.Select(s => s.Code).Contains(item.Code));
            }
        }

        private void UpdateTerritoriesValidity(XContainer territoriesElement)
        {
            // from each like <id type="region" idStatus="..."> regular, macroregion, deprecated, private, unknown
            var territories = territoriesElement.Elements("id").Where(x => x.Attribute("type").Value == "region");

            foreach (var terr in territories)
            {
                // slit Value by Space
                string[] updateLn = GenerateCodeArray(terr);
                // Update each TerritoriesList by Code set IdStatus from Attribute idStatus
                //TerritoriesList
                string status = terr.Attribute("idStatus").Value;
                var containList = TerritoriesList.Where(u => updateLn.Contains(u.Code));
                if (containList.Any())
                {
                    foreach (var te in containList)
                    {
                        te.IdStatus = status;
                    }
                }
                // Status is not available in AddTerritoryEntries(..)
                // Now remove "deprecated", "private_use" territories
                int removeCountDepr = TerritoriesList.RemoveAll(u => u.IdStatus == "deprecated");
                int removeCountPriv = TerritoriesList.RemoveAll(u => u.IdStatus == "private_use");
                // Remove all from TerritoryNamesList not in TerritoriesList
                int removeCountNames = TerritoryNamesList.RemoveAll(item => !TerritoriesList.Select(s => s.Code).Contains(item.Code));
            }
        }

        private void UpdateCurrenciesValidity(XContainer currenciesElement)
        {
            // from each like <id type="currency" idStatus="..."> regular, deprecated, unknown
            var currencies = currenciesElement.Elements("id").Where(x => x.Attribute("type").Value == "currency");

            foreach (var curr in currencies)
            {
                // slit Value by Space
                string[] updateLn = GenerateCodeArray(curr);
                // Update each CurrenciesList by Code set IdStatus from Attribute idStatus
                //CurrenciesList
                string status = curr.Attribute("idStatus").Value;
                var containList = CurrenciesList.Where(u => updateLn.Contains(u.Code));
                if (containList.Any())
                {
                    foreach (var cu in containList)
                    {
                        cu.IdStatus = status;
                    }
                }
                // Status is not available in AddCurrencyEntries(..)
                // Now remove "deprecated" currencies
                int removeCount = CurrenciesList.RemoveAll(u => u.IdStatus == "deprecated");
                // Remove all from CurrencyNamesList not in CurrenciesList
                int removeCountNames = CurrencyNamesList.RemoveAll(item => !CurrenciesList.Select(s => s.Code).Contains(item.Code));
            }
        }

        private void UpdateSubdivisionsValidity(XContainer subdivisionsElement)
        {
            // from each like <id type="subdivision" idStatus="..."> regular, deprecated, unknown
            var subdivitions = subdivisionsElement.Elements("id").Where(x => x.Attribute("type").Value == "subdivision");

            foreach (var subd in subdivitions)
            {
                // slit Value by Space
                string[] updateLn = GenerateCodeArray(subd);
                // Update each TerritoriesList by Code set IdStatus from Attribute idStatus
                //TerritoriesList
                string status = subd.Attribute("idStatus").Value;
                var containList = TerritoriesList.Where(u => updateLn.Contains(u.Code));
                if (containList.Any())
                {
                    foreach (var te in containList)
                    {
                        te.IdStatus = status;
                    }
                }
                // Status is not available in AddTerritorySubdivisionEntries(..)
                // Now remove "deprecated" subdivitions
                int removeCount = TerritoriesList.RemoveAll(u => u.IdStatus == "deprecated");
                // Remove all from TerritoryNamesList not in TerritoriesList
                int removeCountNames = TerritoryNamesList.RemoveAll(item => !TerritoriesList.Select(s => s.Code).Contains(item.Code));
            }
        }

        private string[] GenerateCodeArray(XElement code)
        {
            var dataValueRead = code.Value;
            string[] updateLn = SplitDataToArray(dataValueRead);
            return updateLn;
        }

        private string[] SplitDataToArray(string dataValueRead)
        {
            // first trim and replace \t to "", \n to " "
            string dataValue = dataValueRead.Replace("\t", string.Empty).Replace("\n", " ").Trim();
            List<string> inList = dataValue.Split(' ').ToList();
            // part has "~" (e.g. qaa~z) is to be used as each qaa to qaz or (e.g. Brah~i) each Brah to Brai or (e.g. AC~G) each AC to AG or (e.g. BGL~M) each BGL to BGM
            List<string> outList = new List<string>();
            foreach (var upd in inList)
            {
                if (upd.Contains("~"))
                {
                    string[] d = upd.Split('~');
                    List<string> inserts = new List<string>();
                    char start = d[0].Last();
                    char end = d[1].First();
                    for (char c = start; c <= end; c++)
                    {
                        inserts.Add(d[0].Remove(d[0].Length - 1) + c);
                    }
                    outList.AddRange(inserts);
                }
                else
                    outList.Add(upd);
            }
            string[] updateLn = outList.ToArray();
            return updateLn;
        }

        private void UpdateFromSupplementalData()
        {
            // Update Territories, Currencies, Scripts, Languages, Subdivisions, WindowsZones, TimeZoneNames supplementalData

            using (var stream = File.OpenRead(_CldrPath + @"common\supplemental\supplementalData.xml"))
            {
                var doc = XDocument.Load(stream);

                // Update Territories

                // Territories: "/supplementalData/currencyData"
                var territoriesCurrencyElement = doc.XPathSelectElement("/supplementalData/currencyData");
                if (territoriesCurrencyElement != null)
                {
                    UpdateTerritoriesCurrencySD(territoriesCurrencyElement);
                }
                // Territories: "/supplementalData/territoryContainment"
                var territoriesParentElement = doc.XPathSelectElement("/supplementalData/territoryContainment");
                if (territoriesParentElement != null)
                {
                    UpdateTerritoriesParentSD(territoriesParentElement);
                }

                // Territories: "/supplementalData/languageData"
                var territoriesLanguagesElement = doc.XPathSelectElement("/supplementalData/languageData");
                if (territoriesLanguagesElement != null)
                {
                    UpdateTerritoriesLanguagesSD(territoriesLanguagesElement);
                }

                // Territories: "/supplementalData/codeMappings"
                var territoriesCodeMappingsElement = doc.XPathSelectElement("/supplementalData/codeMappings");
                if (territoriesCodeMappingsElement != null)
                {
                    UpdateTerritoriesCodeMappingsSD(territoriesCodeMappingsElement);
                }

                // Update Currencies

                // Currencies: "/supplementalData/codeMappings"
                var currenciesCodeMappingsElement = doc.XPathSelectElement("/supplementalData/codeMappings");
                if (currenciesCodeMappingsElement != null)
                {
                    UpdateCurrenciesCodeMappingsSD(currenciesCodeMappingsElement);
                }

                // Update Scripts

                // Scripts: "/supplementalData/languageData"
                var scriptsLanguagesElement = doc.XPathSelectElement("/supplementalData/languageData");
                if (scriptsLanguagesElement != null)
                {
                    UpdateScriptsLanguagesSD(scriptsLanguagesElement);
                }

                // Update Languages

                // Languages: "/supplementalData/parentLocales"
                var languagesParentLocaleElement = doc.XPathSelectElement("/supplementalData/parentLocales");
                if (languagesParentLocaleElement != null)
                {
                    UpdateLanguagesParentSD(languagesParentLocaleElement);
                }
            }
            using (var stream = File.OpenRead(_CldrPath + @"common\supplemental\subdivisions.xml"))
            {
                var doc = XDocument.Load(stream);

                // Update Subdivisions

                // Subdivisions: "/supplementalData/subdivisionContainment"
                var subdivisionsParentElement = doc.XPathSelectElement("/supplementalData/subdivisionContainment");
                if (subdivisionsParentElement != null)
                {
                    UpdateSubdivisionsParentSD(subdivisionsParentElement);
                }
            }
            using (var stream = File.OpenRead(_CldrPath + @"common\supplemental\metaZones.xml"))
            {
                var doc = XDocument.Load(stream);

                // Update TimeZones

                // TimeZones: "/supplementalData/metaZones/metazoneInfo" set MetaZoneCode
                var metaZonesElement = doc.XPathSelectElement("/supplementalData/metaZones/metazoneInfo");
                if (metaZonesElement != null)
                {
                    UpdateTimeZoneMetaZoneSD(metaZonesElement);
                }
                // TimeZones: "/supplementalData/metaZones/mapTimezones" set TerritoryCode
                var mapZonesElement = doc.XPathSelectElement("/supplementalData/metaZones/mapTimezones");
                if (mapZonesElement != null)
                {
                    UpdateTimeZoneTerritorySD(mapZonesElement);
                }
            }
        }

        private void UpdateTerritoriesCurrencySD(XElement territoriesCurrencyElement)
        {
            // from each like <region iso3166="AC"> with <currency iso4217="SHP" from="1976-01-01"/>
            // max attribute 'from' and attribute 'to' is null
            var territories = territoriesCurrencyElement.Elements("region");

            foreach (var terr in territories)
            {
                string terrCode = terr.Attribute("iso3166").Value;
                var elements = terr.Elements("currency");
                var elem = (from e in elements
                            where e.Attribute("to") == null && e.Attribute("from") != null
                            orderby e.Attribute("from").Value descending
                            select e);
                var currElement = elem.Any() ? elem.First() : null;

                if (currElement != null)
                {
                    string currCode = currElement.Attribute("iso4217").Value;

                    // Is currCode still in CurrenciesList.Code
                    if (CurrenciesList.Any(cur => currCode.Equals(cur.Code)))
                    {
                        // Update each TerritoriesList by Code set Currency from Attribute iso4217
                        //TerritoriesList
                        var containList = TerritoriesList.Where(u => terrCode.Equals(u.Code));
                        if (containList.Any())
                        {
                            foreach (var te in containList)
                            {
                                te.CurrencyCode = currCode;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTerritoriesParentSD(XElement territoriesParentElement)
        {
            // from each like <group type="001" contains="019 002 150 142 009"/>
            // attribute status="deprecated", status="grouping", grouping="true"
            // not existing status="deprecated"
            var territories = territoriesParentElement.Elements("group");

            foreach (var terr in territories)
            {
                string parrentCode = terr.Attribute("type").Value;
                string parrentContains = terr.Attribute("contains").Value;

                // Is parrentCode still in TerritoriesList.Code
                if (TerritoriesList.Any(ter => parrentCode.Equals(ter.Code)))
                {
                    // If has status="deprecated" then ignore.

                    // If has status="grouping" or grouping="true" then isGrouping.
                    bool isGrouping = false;
                    var status = terr.Attribute("status");
                    if (status != null)
                    {
                        if (status.Value == "deprecated")
                            continue;
                        else if (status.Value == "grouping")
                            isGrouping = true;
                    }
                    var grouping = terr.Attribute("grouping");
                    if (grouping != null && grouping.Value == "true")
                        isGrouping = true;

                    // slit Value by Space
                    string[] updateLn = SplitDataToArray(parrentContains);
                    // Update each TerritoriesList by Code set ParentCode or ParentGroupCode from Attribute type
                    //TerritoriesList
                    var containList = TerritoriesList.Where(u => updateLn.Contains(u.Code));
                    if (containList.Any())
                    {
                        foreach (var te in containList)
                        {
                            // grouping can countain multiple parents
                            if (isGrouping)
                                te.ParentGroupCodes = string.IsNullOrWhiteSpace(te.ParentGroupCodes) ? parrentCode : te.ParentGroupCodes + "," + parrentCode;
                            else
                                te.ParentCode = parrentCode;
                        }
                    }
                }
            }
        }

        private void UpdateTerritoriesLanguagesSD(XElement territoriesLanguagesElement)
        {
            // from each like <language type="aa" territories="DJ ET"/>
            // attribute alt="secondary" add to LanguageAltCodes
            var territories = territoriesLanguagesElement.Elements("language").Where(x => x.Attribute("territories") != null);

            foreach (var terr in territories)
            {
                string languageCode = terr.Attribute("type").Value.Replace("_", "-");
                string territoryCodes = terr.Attribute("territories").Value;

                // Is languageCode still in LanguagesList.Code
                if (LanguagesList.Any(lng => languageCode.Equals(lng.Code)))
                {
                    // If has alt="secondary" then add to LanguageAltCodes.
                    bool isSecondary = false;
                    var secondary = terr.Attribute("alt");
                    if (secondary != null && secondary.Value == "secondary")
                        isSecondary = true;

                    // slit Value by Space
                    string[] updateLn = SplitDataToArray(territoryCodes);
                    // Update each TerritoriesList by Code set LanguageCodes or LanguageAltCodes from Attribute type
                    //TerritoriesList
                    var containList = TerritoriesList.Where(u => updateLn.Contains(u.Code));
                    if (containList.Any())
                    {
                        foreach (var te in containList)
                        {
                            // each can countain multiple Languages
                            if (isSecondary)
                                te.LanguageAltCodes = string.IsNullOrWhiteSpace(te.LanguageAltCodes) ? languageCode : te.LanguageAltCodes + "," + languageCode;
                            else
                                te.LanguageCodes = string.IsNullOrWhiteSpace(te.LanguageCodes) ? languageCode : te.LanguageCodes + "," + languageCode;
                        }
                    }
                }
            }
        }

        private void UpdateTerritoriesCodeMappingsSD(XElement territoriesCodeMappingsElement)
        {
            // from each like <territoryCodes type="AD" numeric="020" alpha3="AND" fips10="AN"/>
            var territories = territoriesCodeMappingsElement.Elements("territoryCodes");

            foreach (var terr in territories)
            {
                // Code is alpha2
                string terrCode = terr.Attribute("type").Value;

                var hasNumeric = terr.Attribute("numeric");
                var hasAlpha3 = terr.Attribute("alpha3");
                var hasFips10 = terr.Attribute("fips10");

                // Update each TerritoriesList by Code set NumericCode, Alpha3Code and Fips10Code from each existing Attribute
                //TerritoriesList
                var containList = TerritoriesList.Where(u => terrCode.Equals(u.Code));
                if (containList.Any())
                {
                    foreach (var te in containList)
                    {
                        if (hasNumeric != null)
                            te.NumericCode = hasNumeric.Value;
                        if (hasAlpha3 != null)
                            te.Alpha3Code = hasAlpha3.Value;
                        if (hasFips10 != null)
                            te.Fips10Code = hasFips10.Value;
                    }
                }
            }
        }

        private void UpdateCurrenciesCodeMappingsSD(XElement currenciesCodeMappingsElement)
        {
            // from each like <currencyCodes type="AED" numeric="784"/>
            var currencies = currenciesCodeMappingsElement.Elements("currencyCodes");

            foreach (var curr in currencies)
            {
                string currCode = curr.Attribute("type").Value;

                // Update each CurrenciesList by Code set NumericCode from Attribute numeric
                //CurrenciesList
                var containList = CurrenciesList.Where(u => currCode.Equals(u.Code));
                if (containList.Any())
                {
                    foreach (var cu in containList)
                    {
                        cu.NumericCode = curr.Attribute("numeric").Value;
                    }
                }
            }
        }

        private void UpdateScriptsLanguagesSD(XElement scriptsLanguagesElement)
        {
            // from each like <language type="aa" scripts="Latn"/>
            // attribute alt="secondary" add to LanguageAltCodes
            var scripts = scriptsLanguagesElement.Elements("language").Where(x => x.Attribute("scripts") != null);

            foreach (var scri in scripts)
            {
                string languageCode = scri.Attribute("type").Value.Replace("_", "-");
                string scriptCodes = scri.Attribute("scripts").Value;

                // Is languageCode still in LanguagesList.Code
                if (LanguagesList.Any(lng => languageCode.Equals(lng.Code)))
                {
                    // If has alt="secondary" then add to LanguageAltCodes.
                    bool isSecondary = false;
                    var secondary = scri.Attribute("alt");
                    if (secondary != null && secondary.Value == "secondary")
                        isSecondary = true;

                    // slit Value by Space
                    string[] updateLn = SplitDataToArray(scriptCodes);
                    // Update each ScriptsList by Code set LanguageCodes or LanguageAltCodes from Attribute type
                    //ScriptsList
                    var containList = ScriptsList.Where(u => updateLn.Contains(u.Code));
                    if (containList.Any())
                    {
                        foreach (var sc in containList)
                        {
                            // each can countain multiple Languages
                            if (isSecondary)
                                sc.LanguageAltCodes = string.IsNullOrWhiteSpace(sc.LanguageAltCodes) ? languageCode : sc.LanguageAltCodes + "," + languageCode;
                            else
                                sc.LanguageCodes = string.IsNullOrWhiteSpace(sc.LanguageCodes) ? languageCode : sc.LanguageCodes + "," + languageCode;
                        }
                    }
                }
            }
        }

        private void UpdateLanguagesParentSD(XElement languagesParentLocaleElement)
        {
            // from each like <parentLocale parent="root" locales="..."/>
            var languages = languagesParentLocaleElement.Elements("parentLocale");

            foreach (var lang in languages)
            {
                string parrentCode = lang.Attribute("parent").Value.Replace("_", "-");
                string parrentContains = lang.Attribute("locales").Value.Replace("_", "-");

                // Is parrentCode still in LanguagesList.Code
                if (LanguagesList.Any(lng => parrentCode.Equals(lng.Code)))
                {
                    // slit Value by Space
                    string[] updateLn = SplitDataToArray(parrentContains);
                    // Update each LanguagesList by Code set ParentCode from Attribute parent
                    //LanguagesList
                    var containList = LanguagesList.Where(u => updateLn.Contains(u.Code));
                    if (containList.Any())
                    {
                        foreach (var ln in containList)
                        {
                            ln.ParentCode = parrentCode;
                        }
                    }
                }
            }
        }

        private void UpdateSubdivisionsParentSD(XElement subdivisionsParentElement)
        {
            // from each like <subgroup type="AD" contains="ad02 ad03 ad04 ad05 ad06 ad07 ad08"/>
            var subdivisions = subdivisionsParentElement.Elements("subgroup");

            foreach (var subd in subdivisions)
            {
                string parrentCode = subd.Attribute("type").Value;
                string parrentContains = subd.Attribute("contains").Value;

                // Is parrentCode still in TerritoriesList.Code
                if (TerritoriesList.Any(ter => parrentCode.Equals(ter.Code)))
                {
                    // slit Value by Space
                    string[] updateLn = SplitDataToArray(parrentContains);
                    // Update each TerritoriesList by Code set ParentCode from Attribute type
                    //TerritoriesList
                    var containList = TerritoriesList.Where(u => updateLn.Contains(u.Code));
                    if (containList.Any())
                    {
                        foreach (var te in containList)
                        {
                            te.ParentCode = parrentCode;
                        }
                    }
                }
            }
        }

        private void UpdateTimeZoneMetaZoneSD(XElement metaZonesElement)
        {
            // from each like <timezone type="Africa/Algiers"> with <usesMetazone from="1981-05-01 00:00" mzone="Europe_Central"/>
            // max attribute 'from' and attribute 'to' is null
            var metaZones = metaZonesElement.Elements("timezone");

            foreach (var meta in metaZones)
            {
                string timeCode = meta.Attribute("type").Value;
                var elements = meta.Elements("usesMetazone");
                var elem = (from e in elements
                            where e.Attribute("to") == null && e.Attribute("from") != null
                            orderby e.Attribute("from").Value descending
                            select e);
                // if no 'to' and no 'from' Attribute exists, use first element.
                var timeElement = elem.Any() ? elem.First() : elements.First();

                if (timeElement != null)
                {
                    string mzoneCode = timeElement.Attribute("mzone").Value;

                    // Is mzoneCode still in MetaZonesList.Code
                    if (MetaZonesList.Any(ter => mzoneCode.Equals(ter.Code)))
                    {
                        // Update each TimeZonesList by Code set MetaZoneCode from Attribute mzone
                        //TimeZonesList
                        var containList = TimeZonesList.Where(u => timeCode.Equals(u.Code));
                        if (containList.Any())
                        {
                            foreach (var ti in containList)
                            {
                                ti.MetaZoneCode = mzoneCode;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTimeZoneTerritorySD(XElement mapZonesElement)
        {
            // from each like <mapZone other="Acre" territory="001" type="America/Rio_Branco"/>
            var mapZones = mapZonesElement.Elements("mapZone");

            foreach (var map in mapZones)
            {
                string timeCode = map.Attribute("type").Value;
                var territory = map.Attribute("territory");

                if (territory != null)
                {
                    // Is territory still in TerritoriesList.Code
                    if (TerritoriesList.Any(ter => territory.Value.Equals(ter.Code)))
                    {
                        // Update each TimeZonesList by Code set TerritoryCode from Attribute territory
                        //TimeZonesList
                        var containList = TimeZonesList.Where(u => timeCode.Equals(u.Code));
                        if (containList.Any())
                        {
                            foreach (var ti in containList)
                            {
                                ti.TerritoryCode = territory.Value;
                            }
                        }
                    }
                }
            }
        }
    }
}