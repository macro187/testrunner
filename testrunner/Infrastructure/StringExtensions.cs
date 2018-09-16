using System;
using System.Globalization;
using System.Linq;

namespace TestRunner.Infrastructure
{

    public static class StringExtensions
    {

        public static string FormatInvariant(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }


        public static string Indent(string value)
        {
            Guard.NotNull(value, nameof(value));
            var lines = SplitLines(value);
            var indentedLines = lines.Select(s => "  " + s);
            return string.Join(Environment.NewLine, indentedLines);
        }


        public static string[] SplitLines(string value)
        {
            Guard.NotNull(value, nameof(value));
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            if (value.EndsWith("\n", StringComparison.Ordinal))
            {
                value = value.Substring(0, value.Length-1);
            }
            return value.Split('\n');
        }


        public static string[] FormatHeading(char ruleCharacter, params string[] lines)
        {
            if (lines == null) return new string[0];
            if (lines.Length == 0) return new string [0];

            var longestLine = lines.Max(line => line.Length);
            var rule = new string(ruleCharacter, longestLine);

            return
                Enumerable.Empty<string>()
                    .Concat(new[]{ rule })
                    .Concat(lines)
                    .Concat(new[]{ rule })
                    .ToArray();
        }

    }

}
