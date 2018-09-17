using System;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    public static class EventHandler
    {

        /// <summary>
        /// Program banner
        /// </summary>
        ///
        public static void BannerEvent(params string[] lines)
        {
            lines = lines ?? new string[0];
            var formattedBanner = StringExtensions.FormatHeading('=', lines);
            Console.Error.WriteLine();
            foreach (var line in formattedBanner) Console.Error.WriteLine(line);
        }


        /// <summary>
        /// Diagnostic messaging to do with the operation of TestRunner itself
        /// </summary>
        ///
        public static void DiagnosticEvent(string[] values)
        {
            if (values == null) return;
            foreach (var value in values) DiagnosticEvent(value);
        }


        /// <summary>
        /// Diagnostic messaging to do with the operation of TestRunner itself
        /// </summary>
        ///
        public static void DiagnosticEvent(string value = "")
        {
            var lines = StringExtensions.SplitLines(value);
            foreach (var line in lines) Console.Error.WriteLine(line);
        }

    }
}
