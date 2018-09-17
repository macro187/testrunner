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
            var formattedLines = StringExtensions.FormatHeading('=', lines);
            WriteError();
            foreach (var line in formattedLines) WriteError(line);
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
        public static void DiagnosticEvent(string message = "")
        {
            WriteError(message);
        }


        /// <summary>
        /// <see cref="System.Diagnostics.Trace"/> output
        /// </summary>
        ///
        public static void TraceEvent(string message = "")
        {
            WriteOut(message);
        }


        static void WriteOut(string message = "")
        {
            message = message ?? "";
            foreach (var line in StringExtensions.SplitLines(message)) Console.Out.WriteLine(line);
        }


        static void WriteError(string message = "")
        {
            message = message ?? "";
            foreach (var line in StringExtensions.SplitLines(message)) Console.Error.WriteLine(line);
        }

    }
}
