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
        /// Program usage information
        /// </summary>
        ///
        public static void UsageEvent(string message)
        {
            Guard.NotNull(message, nameof(message));
            WriteError();
            WriteError(message);
            WriteError();
        }


        /// <summary>
        /// <see cref="System.Diagnostics.Trace"/> output
        /// </summary>
        ///
        public static void TraceEvent(string message = "")
        {
            WriteOut(message);
        }


        /// <summary>
        /// A user-facing error occurred
        /// </summary>
        ///
        public static void UserErrorEvent(UserException exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError(exception.Message);
        }


        /// <summary>
        /// An internal error occurred
        /// </summary>
        ///
        public static void InternalErrorEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError("An internal error occurred:");
            WriteError(ExceptionExtensions.FormatException(exception));
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
