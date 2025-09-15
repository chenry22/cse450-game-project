using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

/*
JSON Statistics:
- Dough Handling    = total # of lines
- Toppings          = total # of elements (keys + array elements)
- Cutting           = total # of unique keys
- Cooking           = max nesting depth
- Speed             = avg length of string values, or longest number if no strings
- Stamina           = number of keys
*/

namespace FileAnalysis
{
    public static class JSONAnalyzer
    {
        public static Stats Analyze(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return GenerateStats(content);
        }

        private static Stats GenerateStats(string jsonContent)
        {
            // dough handling
            int lineCount = jsonContent.Split('\n').Length;

            // key/element counting
            int keyCount = Regex.Matches(jsonContent, "\"[^\"]+\"\\s*:").Count;
            int arrayBracketCount = Regex.Matches(jsonContent, "\\[").Count + Regex.Matches(jsonContent, "\\]").Count;
            int totalElements = keyCount + arrayBracketCount;

            // unique keys
            var keyMatches = Regex.Matches(jsonContent, "\"([^\"]+)\"\\s*:");
            HashSet<string> uniqueKeys = new HashSet<string>();
            foreach (Match match in keyMatches)
                uniqueKeys.Add(match.Groups[1].Value);

            // stamina
            int totalKeys = keyCount;

            // max nesting depth
            int maxNestingDepth = 0;
            int currentDepth = 0;
            foreach (char c in jsonContent)
            {
                if (c == '{' || c == '[')
                {
                    currentDepth++;
                    if (currentDepth > maxNestingDepth) maxNestingDepth = currentDepth;
                }
                else if (c == '}' || c == ']')
                {
                    currentDepth--;
                }
            }

            // speed
            var stringValueMatches = Regex.Matches(jsonContent, ":\\s*\"([^\"]+)\"");
            List<int> stringValueLengths = new List<int>();
            foreach (Match match in stringValueMatches)
                stringValueLengths.Add(match.Groups[1].Value.Length);

            int speedValue = 0;
            if (stringValueLengths.Count > 0)
            {
                speedValue = (int)Math.Round(stringValueLengths.Average());
            }
            else
            {
                var numberValueMatches = Regex.Matches(jsonContent, ":\\s*([0-9\\.]+)");
                int maxNumberLength = 0;
                foreach (Match match in numberValueMatches)
                    maxNumberLength = Math.Max(maxNumberLength, match.Groups[1].Value.Length);
                speedValue = maxNumberLength;
            }

            int doughHandling = Math.Clamp(lineCount * 5, 0, 100);
            int toppings     = Math.Clamp(totalElements * 3, 0, 100);
            int cutting      = Math.Clamp(uniqueKeys.Count * 7, 0, 100);
            int cooking      = Math.Clamp(maxNestingDepth * 10, 0, 100);
            int speed        = Math.Clamp(speedValue * 2, 0, 100);
            int stamina      = Math.Clamp(totalKeys * 5, 0, 100);

            return new Stats
            {
                DoughHandling = doughHandling,
                Toppings      = toppings,
                Cooking       = cooking,
                Cutting       = cutting,
                Speed         = speed,
                Stamina       = stamina
            };
        }
    }
}