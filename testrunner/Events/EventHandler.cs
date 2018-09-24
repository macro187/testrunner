using System;
using System.Reflection;
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


        public static void TestClassEndEvent(
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


        public static void TestBeginEvent(string name)
        {
            WriteOut();
            WriteSubheadingOut(name.Replace("_", " "));
        }


        public static void TestIgnoredEvent()
        {
            WriteOut();
            WriteOut("Ignored because [TestMethod] is decorated with [Ignore]");
        }


        public static void TestEndEvent(bool passed)
        {
            WriteOut();
            WriteOut(passed ? "Passed" : "FAILED");
        }


        public static void AssemblyInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[AssemblyInitialize]");
        }


        public static void AssemblyInitializeMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void AssemblyCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[AssemblyCleanup]");
        }


        public static void AssemblyCleanupMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void ClassInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[ClassInitialize]");
        }


        public static void ClassInitializeMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void ClassCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[ClassCleanup]");
        }


        public static void ClassCleanupMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void TestContextSetterBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "");
        }


        public static void TestContextSetterEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void TestInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestInitialize]");
        }


        public static void TestInitializeMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void TestMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestMethod]");
        }


        public static void TestMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void TestCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestCleanup]");
        }


        public static void TestCleanupMethodEndEvent(bool success)
        {
            WriteMethodEnd(success);
        }


        public static void MethodExpectedExceptionEvent(string name)
        {
            Guard.NotNull(name, nameof(name));
            WriteOut($"  [ExpectedException] {name} occurred:");
        }


        public static void MethodExceptionEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteOut(StringExtensions.Indent(ExceptionExtensions.FormatException(exception)));
        }


        public static void MethodTimingEvent(long milliseconds)
        {
            WriteOut($"  ({milliseconds:N0} ms)");
        }


        public static void OutputTraceEvent(string message = "")
        {
            WriteOut(message);
        }


        static void WriteMethodBegin(MethodInfo method, string prefix)
        {
            Guard.NotNull(method, nameof(method));
            Guard.NotNull(prefix, nameof(prefix));
            prefix = prefix != "" ? prefix + " " : prefix;
            WriteOut();
            WriteOut($"{prefix}{method.Name}()");
        }


        static void WriteMethodEnd(bool success)
        {
            WriteOut(success ? "  Succeeded" : "  Failed");
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
