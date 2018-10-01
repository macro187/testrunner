using System;
using System.Linq;

namespace TestRunner.Infrastructure
{

    public static class StringExtensions
    {

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

    }

}
