using System;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that produces default human-readable output to <see cref="Console.Out"/> and
    /// <see cref="Console.Error"/>
    /// </summary>
    ///
    public class DefaultOutputEventHandler : EventHandler
    {

        static void WriteMethodBegin(MethodInfo method, string prefix)
        {
            Guard.NotNull(method, nameof(method));
            Guard.NotNull(prefix, nameof(prefix));
            prefix = prefix != "" ? prefix + " " : prefix;
            WriteOut();
            WriteOut($"{prefix}{method.Name}()");
        }


        static void WriteMethodEnd(bool success, long elapsedMilliseconds)
        {
            var result = success ? "Succeeded" : "Failed";
            WriteOut($"  {result} ({elapsedMilliseconds:N0} ms)");
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


        public override void ProgramBannerEvent(params string[] lines)
        {
            WriteError();
            WriteHeadingError(lines);
            base.ProgramBannerEvent(lines);
        }


        public override void ProgramUsageEvent(string message)
        {
            Guard.NotNull(message, nameof(message));
            WriteError();
            WriteError(message);
            WriteError();
            base.ProgramUsageEvent(message);
        }


        public override void ProgramUserErrorEvent(UserException exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError(exception.Message);
            base.ProgramUserErrorEvent(exception);
        }


        public override void ProgramInternalErrorEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteError();
            WriteError("An internal error occurred:");

            if (exception is ReflectionTypeLoadException rtle)
            {
                foreach (var le in rtle.LoaderExceptions)
                {
                    WriteError(ExceptionExtensions.FormatException(le));
                }
            }

            WriteError(ExceptionExtensions.FormatException(exception));

            base.ProgramInternalErrorEvent(exception);
        }


        public override void TestAssemblyBeginEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut();
            WriteHeadingOut(path);
            base.TestAssemblyBeginEvent(path);
        }


        public override void TestAssemblyNotFoundEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Test assembly not found: {path}");
            base.TestAssemblyNotFoundEvent(path);
        }


        public override void TestAssemblyNotDotNetEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Not a .NET assembly: {path}");
            base.TestAssemblyNotDotNetEvent(path);
        }


        public override void TestAssemblyNotTestEvent(string path)
        {
            Guard.NotNull(path, nameof(path));
            WriteOut();
            WriteOut($"Not a test assembly: {path}");
            base.TestAssemblyNotTestEvent(path);
        }


        public override void TestAssemblyConfigFileSwitchedEvent(string path)
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

            base.TestAssemblyConfigFileSwitchedEvent(path);
        }


        public override void TestAssemblyEndEvent()
        {
            base.TestAssemblyEndEvent();
        }


        public override void TestClassBeginEvent(string fullName)
        {
            Guard.NotNull(fullName, nameof(fullName));
            WriteOut();
            WriteHeadingOut(fullName);
            base.TestClassBeginEvent(fullName);
        }


        public override void TestClassIgnoredEvent()
        {
            WriteOut();
            WriteOut("Ignoring all tests because class is decorated with [Ignore]");
            base.TestClassIgnoredEvent();
        }


        public override void TestClassEndEvent(
            bool classIgnored,
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
                    ? classIgnored
                        ? "Ignored"
                        : initializeSucceeded
                            ? "Succeeded"
                            : "Failed"
                    : "Not present";

            var cleanupResult =
                cleanupPresent
                    ? classIgnored
                        ? "Ignored"
                        : cleanupSucceeded
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

            base.TestClassEndEvent(
                classIgnored,
                initializePresent,
                initializeSucceeded,
                testsTotal,
                testsRan,
                testsIgnored,
                testsPassed,
                testsFailed,
                cleanupPresent,
                cleanupSucceeded);
        }


        public override void TestBeginEvent(string name)
        {
            WriteOut();
            WriteSubheadingOut(name.Replace("_", " "));
            base.TestBeginEvent(name);
        }


        public override void TestIgnoredEvent()
        {
            WriteOut();
            WriteOut("Ignored because [TestMethod] is decorated with [Ignore]");
            base.TestIgnoredEvent();
        }


        public override void TestEndEvent(bool passed)
        {
            WriteOut();
            WriteOut(passed ? "Passed" : "FAILED");
            base.TestEndEvent(passed);
        }


        public override void AssemblyInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[AssemblyInitialize]");
            base.AssemblyInitializeMethodBeginEvent(method);
        }


        public override void AssemblyInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.AssemblyInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void AssemblyCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[AssemblyCleanup]");
            base.AssemblyCleanupMethodBeginEvent(method);
        }


        public override void AssemblyCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.AssemblyCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[ClassInitialize]");
            base.ClassInitializeMethodBeginEvent(method);
        }


        public override void ClassInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.ClassInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[ClassCleanup]");
            base.ClassCleanupMethodBeginEvent(method);
        }


        public override void ClassCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.ClassCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestContextSetterBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "");
            base.TestContextSetterBeginEvent(method);
        }


        public override void TestContextSetterEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.TestContextSetterEndEvent(success, elapsedMilliseconds);
        }


        public override void TestInitializeMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestInitialize]");
            base.TestInitializeMethodBeginEvent(method);
        }


        public override void TestInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.TestInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestMethod]");
            base.TestMethodBeginEvent(method);
        }


        public override void TestMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.TestMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestCleanupMethodBeginEvent(MethodInfo method)
        {
            WriteMethodBegin(method, "[TestCleanup]");
            base.TestCleanupMethodBeginEvent(method);
        }


        public override void TestCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            WriteMethodEnd(success, elapsedMilliseconds);
            base.TestCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void MethodExpectedExceptionEvent(Type expected, Exception exception)
        {
            Guard.NotNull(expected, nameof(expected));
            Guard.NotNull(exception, nameof(exception));
            WriteOut($"  [ExpectedException] {expected.FullName} occurred:");
            WriteOut(StringExtensions.Indent(ExceptionExtensions.FormatException(exception)));
            base.MethodExpectedExceptionEvent(expected, exception);
        }


        public override void MethodUnexpectedExceptionEvent(Exception exception)
        {
            Guard.NotNull(exception, nameof(exception));
            WriteOut(StringExtensions.Indent(ExceptionExtensions.FormatException(exception)));
            base.MethodUnexpectedExceptionEvent(exception);
        }


        public override void OutputTraceEvent(string message = "")
        {
            WriteOut(message);
            base.OutputTraceEvent(message);
        }

    }
}
