using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FileAnalysis
{

    public static class HTMLAnalyzer
    {
        public static Stats Analyze(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return AnalyzeContent(content);
        }

        private static Stats AnalyzeContent(string content)
        {
            var doughHandling_weights = new Dictionary<string, int>()
            {
                { "div", 3 },
                { "section", 3 },
                { "article", 3 },
                { "form", 5 },
                { "fieldset", 3 },
                { "textarea", 4 },
                { "template", 7 },
                { "input", 3 }
            };

            var topping_weights = new Dictionary<string, int>()
            {
                { "ul", 3 },
                { "ol", 3 },
                { "li", 2 },
                { "span", 1 },
                { "img", 5 },
                { "button", 2 },
                { "label", 1 },
                { "details", 2 },
                { "summary", 2 }
            };

            var cooking_weights = new Dictionary<string, int>()
            {
                { "progress", 10 },
                { "meter", 4 },
                { "script", 5 },
                { "canvas", 4 },
                { "style", 3 },
                { "time", 3 },
                { "p", 1},
                { "h1", 1 },
                { "h2", 2 },
                { "h3", 3 },
                { "h4", 4 },
                { "h5", 5 },
                { "h6", 6 }
            };

            var cutting_weights = new Dictionary<string, int>()
            {
                { "br", 8 },
                { "hr", 5 },
                { "nav", 3 },
                { "table", 4 },
                { "tr", 2 },
                { "td", 1 },
                { "dl", 3 }
            };

            var speed_weights = new Dictionary<string, int>()
            {
                { "marquee", 5 },
                { "range", 4 },
                { "video", 4 },
                { "audio", 3 },
                { "script", 2 },
                { "link", 3 },
                { "iframe", 3 },
                { "p", 1},
                { "h1", 1 },
                { "h2", 2 },
                { "h3", 3 },
                { "h4", 4 },
                { "h5", 5 },
                { "h6", 6 }
            };

            var stamina_weights = new Dictionary<string, int>()
            {
                { "body", 5 },
                { "main", 4 },
                { "html", 4 },
                { "footer", 3 },
                { "header", 3 }
            };

            return new Stats
            {
                DoughHandling = CalculateStat(content, doughHandling_weights),
                Toppings = CalculateStat(content, topping_weights),
                Cooking = CalculateStat(content, cooking_weights),
                Cutting = CalculateStat(content, cutting_weights),
                Speed = CalculateStat(content, speed_weights),
                Stamina = CalculateStat(content, stamina_weights),
            };
        }

        static int CalculateStat(string content, Dictionary<string, int> tagWeights)
        {
            int score = 0;
            foreach (var tag in tagWeights)
            {
                int count = Regex.Matches(content, $@"<\s*{tag.Key}(\s|>)", RegexOptions.IgnoreCase).Count;
                score += count * tag.Value;
            }

            int scaledScore = (int)(score * 3.5);
            return Math.Clamp(scaledScore, 0, 100);
        }
    }
}