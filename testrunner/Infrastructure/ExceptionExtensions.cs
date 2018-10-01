using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace TestRunner.Infrastructure
{

    public static class ExceptionExtensions
    {

        public static string FormatException(Exception ex)
        {
            if (ex == null) return "";
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine("Type: " + ex.GetType().FullName);
            if (ex.Data != null)
            {
                foreach (DictionaryEntry de in ex.Data)
                {
                    sb.AppendLine($"Data.{de.Key}: {de.Value}");
                }
            }
            if (!string.IsNullOrWhiteSpace(ex.Source))
            {
                sb.AppendLine("Source: " + ex.Source);
            }
            if (!string.IsNullOrWhiteSpace(ex.HelpLink))
            {
                sb.AppendLine("HelpLink: " + ex.HelpLink);
            }
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                sb.AppendLine("StackTrace:");
                sb.AppendLine(StringExtensions.Indent(FormatStackTrace(ex.StackTrace)));
            }
            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException:");
                sb.AppendLine(StringExtensions.Indent(FormatException(ex.InnerException)));
            }
            return sb.ToString();
        }


        static string FormatStackTrace(string stackTrace)
        {
            return string.Join(
                Environment.NewLine,
                StringExtensions.SplitLines(stackTrace)
                    .Select(line => line.Trim())
                    .SelectMany(line => {
                        var i = line.IndexOf(" in ", StringComparison.Ordinal);
                        if (i <= 0) return new[] {line};
                        var inPart = line.Substring(i + 1);
                        var atPart = line.Substring(0, i);
                        return new[] {atPart, StringExtensions.Indent(inPart)};
                        }));
        }

    }
}
