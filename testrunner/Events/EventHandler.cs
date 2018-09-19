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


        public static void ProgramUserErrorEvent(UserException exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError(exception.Message);
        }


        public static void ProgramInternalErrorEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError("An internal error occurred:");
            WriteError(ExceptionExtensions.FormatException(exception));
        }


        public static void TestAssemblyBeginEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut();
            WriteHeadingOut(path);
        }


        public static void TestAssemblyNotFoundEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Test assembly not found: {path}");
        }


        public static void TestAssemblyNotDotNetEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Not a .NET assembly: {path}");
        }


        public static void TestAssemblyNotTestEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Not a test assembly: {path}");
        }


        public static void TestAssemblyConfigFileSwitchedEvent(string path)
        {
            WriteOut();
            WriteOut("Configuration File:");
            WriteOut(path);

            if (Type.GetType("Mono.Runtime") != null)
            {
                WriteOut();
                WriteOut("WARNING: Running on Mono, configuration file will probably not take effect");
                WriteOut("See https://bugzilla.xamarin.com/show_bug.cgi?id=15741");
            }
        }


        public static void TestAssemblyEndEvent()
        {
        }


        public static void TestClassBeginEvent(string fullName)
        {
            Guard.NotNull(fullName, nameof(fullName));
            WriteOut();
            WriteHeadingOut(fullName);
        }


        public static void TestClassIgnoredEvent()
        {
            WriteOut();
            WriteOut("Ignoring all tests because class is decorated with [Ignore]");
        }


        public static void TestClassSummaryEvent(
            bool initializePresent,
            bool initializeSucceeded,
            int testsTotal,
            int testsRan,
            int testsIgnored,
            int testsPassed,
            int testsFailed,
            bool cleanupPresent,
            bool cleanupSucceeded
        )
        {
            var initializeResult =
                initializePresent
                    ? initializeSucceeded
                        ? "Succeeded"
                        : "Failed"
                    : "Not present";

            var cleanupResult =
                cleanupPresent
                    ? cleanupSucceeded
                        ? "Succeeded"
                        : "Failed"
                    : "Not present";

            WriteOut();
            WriteSubheadingOut("Summary");
            WriteOut();
            WriteOut($"ClassInitialize: {initializeResult}");
            WriteOut($"Total:           {testsTotal} tests");
            WriteOut($"Ran:             {testsRan} tests");
            WriteOut($"Ignored:         {testsIgnored} tests");
            WriteOut($"Passed:          {testsPassed} tests");
            WriteOut($"Failed:          {testsFailed} tests");
            WriteOut($"ClassCleanup:    {cleanupResult}");
        }


        public static void TestClassEndEvent()
        {
        }


        public static void TestMethodBeginEvent(string name)
        {
            WriteOut();
            WriteSubheadingOut(name.Replace("_", " "));
        }


        public static void TestMethodIgnoredEvent()
        {
            WriteOut();
            WriteOut("Ignored because method is decorated with [Ignore]");
        }


        public static void TestMethodEndEvent(bool passed)
        {
            WriteOut();
            WriteOut(passed ? "Passed" : "FAILED");
        }


        public static void OutputTraceEvent(string message = "")
        {
            WriteOut(message);
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


        static void WriteSubheadingOut(params string[] lines)
        {
            lines = lines ?? new string[0];
            lines = StringExtensions.FormatHeading('-', lines);
            foreach (var line in lines) WriteOut(line);
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
