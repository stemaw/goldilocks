using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DuoVia.FuzzyStrings;

namespace DataImporter
{
    public static class Fuzzy
    {
        public static string FuzzySearch(IEnumerable<string> potentials, string searchTerm)
        {
            var reducedPotentials = potentials.Where(p => GetStart(p) == GetStart(searchTerm));

            if (!reducedPotentials.Any()) reducedPotentials = potentials;

            var matches = new Dictionary<string, int>();

            foreach (var potential in reducedPotentials)
            {
                var fuzzyScore = Compute(potential, searchTerm);

                if (fuzzyScore != searchTerm.Length)
                {
                    if (matches.ContainsKey(potential)) continue;
                    matches.Add(potential, fuzzyScore);
                }
            }

            return !matches.Any() ? null : matches.OrderBy(r => r.Value).First().Key;
        }

        public static IEnumerable<string> FuzzyMatches(string[] potentials, string searchTerm, bool isSeriesSearch)
        {
            searchTerm = searchTerm.ToLower();

            var results = new List<string>();

            foreach (var potential in potentials)
            {
                if (SteStein(potential.ToLower(), searchTerm, false, isSeriesSearch))
                {
                    results.Add(potential);
                }
            }
            
            if (!results.Any())
            {
                foreach (var potential in potentials)
                {
                    if (SteStein(potential.ToLower(), searchTerm, true, isSeriesSearch))
                    {
                        results.Add(potential);
                    }
                }
            }
            return results;
        }

        private static bool SteStein(string potential, string searchTerm, bool lessStrict, bool isSeriesSearch)
        {
            var splitSearch = searchTerm.Split(' ');

            var alwaysIgnore = new string[] {"hatchback", "saloon"};
            var otherIgnoreWords = new string[]{"convertible", "cabriolet", "sportback", "roadster", "estate", "cabrio", "hardtop", "softtop", "hardback"};
            bool matched = true;

            if (!isSeriesSearch)
            {
                if (!potential.StartsWith("one") &&
                    string.Compare(potential.Split(' ')[0], splitSearch[0], CultureInfo.CurrentCulture,
                                   CompareOptions.IgnoreNonSpace) != 0)
                    return false;
            }

            foreach (var word in splitSearch)
            {
                matched = matched && CheckBodyType(potential, word) && (
                    alwaysIgnore.Contains(word) 
                    || string.Compare(word, "coupe", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0 
                    || (lessStrict && otherIgnoreWords.Contains(word)) 
                    || potential.Contains(word)
                    || ContainsSynonymOf(potential, word));
            }

            return matched;
        }

        private static bool CheckBodyType(string potential, string word)
        {
            if (ContainsSynonymOf(word, "estate"))
            {
                return ContainsSynonymOf(potential, "estate");
            }
                
            if(ContainsSynonymOf(word, "cabrio"))
            {
                 return ContainsSynonymOf(potential, "carbio");
            }

            return true;
        }

        private static bool ContainsSynonymOf(string potential, string word)
        {
            var matchingSynonyms = CarSynonyms().FirstOrDefault(s => s.Contains(word));

            if (matchingSynonyms == null) return false;

            return matchingSynonyms.Any(potential.Contains);
        }

        private static IEnumerable<List<string>> CarSynonyms()
        {
            return new List<List<string>>
                {
                    new List<string> {"sportswagon","s wagon", "sport wagon", "sports wagon", "swagon", "sportwagon", "s-wagon"},
                    new List<string> {"tourer", "estate", "touring", "avant", "aerodeck", "stationwagon", "station wagon"},
                    new List<string> {"cabrio", "cabriolet", "convertible", "softtop"},
                    
                };

        }
        private static string GetStart(string value)
        {
            return value.Substring(0, value.Length > 3 ? 3 : value.Length).ToLower();
        }

        public static int Compute(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        
    }
}