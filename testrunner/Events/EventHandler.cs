using System;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    public static class EventHandler
    {

        public static void ProgramBannerEvent(params string[] lines)
        {
            WriteError();
            WriteHeadingError(lines);
        }


        public static void ProgramUsageEvent(string message)
        {
            Guard.NotNull(message, nameof(message));
            WriteError();
            WriteError(message);
            WriteError();
        }


        public static void TestTraceOutputEvent(string message = "")
        {
            WriteOut(message);
        }


        public static void UserErrorEvent(UserException exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError(exception.Message);
        }


        public static void InternalErrorEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError("An internal error occurred:");
            WriteError(ExceptionExtensions.FormatException(exception));
        }


        static void WriteHeadingOut(params string[] lines)
        {
            lines = lines ?? new string[0];
            lines = StringExtensions.FormatHeading('=', lines);
            foreach (var line in lines) WriteOut(line);
        }


        static void WriteHeadingError(params string[] lines)
        {
            lines = lines ?? new string[0];
            lines = StringExtensions.FormatHeading('=', lines);
            foreach (var line in lines) WriteError(line);
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
