using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/*
TXT Statistics:
- Dough Handling    = # of lines
- Toppings          = # of unique words
- Cooking           = avg sentence length (in words)
- Cutting           = # of punctuation marks (sentence being CUT)
- Speed             = avg word length (longer = slower read time, shorter = faster read time)
- Stamina           = Total character count (stamina to read)
*/

namespace FileAnalysis
{
    public static class TXTAnalyzer
    {
        public static Stats Analyze(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return GenerateStats(content);
        }

        private static Stats GenerateStats(string textContent)
        {
            // dough handling
            int lineCount = textContent.Split('\n').Length;

            // toppings
            var words = Regex.Matches(textContent.ToLower(), @"\b\w+\b").Cast<Match>().Select(m => m.Value).ToList();
            int numUniqueWords = Math.Max(1, words.Distinct().Count()); // at least 1...

            // cooking
            var sentences = Regex.Split(textContent, @"[.!?]+").Where(s => s.Trim().Length > 0).ToList();
            double avgSentenceLength = sentences.Count > 0
                ? sentences.Select(s => Regex.Matches(s, @"\b\w+\b").Count()).Average()
                : 1;

            // cutting
            int punctuationCount = Math.Max(1, Regex.Matches(textContent, @"[.,;:!?]").Count);

            // speed
            double avgWordLength = words.Count > 0
                ? words.Select(w => w.Length).Average()
                : 0;

            // stamina
            int charCount = textContent.Length;

            int doughHandling = lineCount % 100; // Math.Clamp(lineCount * 5, 0, 100);
            int toppings = numUniqueWords % 100; // Math.Clamp(numUniqueWords * 2, 0, 100);
            int cooking = (int)(avgSentenceLength * 10) % 100;// Math.Clamp((int)(avgSentenceLength * 10), 0, 100);
            int cutting = punctuationCount * 5 % 100; // Math.Clamp(punctuationCount * 5, 0, 100);
            int speed = (int)(avgWordLength * 10) % 100; // Math.Clamp((int)(avgWordLength * 10), 0, 100);
            int stamina = charCount % 100; // Math.Clamp(charCount / 10, 0, 100);

            return new Stats {
                DoughHandling = doughHandling,
                Toppings = toppings,
                Cooking = cooking,
                Cutting = cutting,
                Speed = speed,
                Stamina = stamina
            };
        }
    }
}