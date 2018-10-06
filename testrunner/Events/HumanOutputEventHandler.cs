using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using TestRunner.Domain;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that produces output in human-readable format
    /// </summary>
    ///
    public class HumanOutputEventHandler : EventHandler
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
            lines = FormatHeading('=', lines);
            foreach (var line in lines) WriteOut(line);
        }


        static void WriteHeadingError(params string[] lines)
        {
            lines = lines ?? new string[0];
            lines = FormatHeading('=', lines);
            foreach (var line in lines) WriteError(line);
        }


        static void WriteSubheadingOut(params string[] lines)
        {
            lines = lines ?? new string[0];
            lines = FormatHeading('-', lines);
            foreach (var line in lines) WriteOut(line);
        }


        static void WriteOut(string message = "")
        {
            message = message ?? "";
            foreach (var line in StringExtensions.SplitLines(message)) Console.Out.WriteLine(line);
        }


        static void WriteError(string[] lines)
        {
            Guard.NotNull(lines, nameof(lines));
            foreach (var line in lines) WriteError(line);
        }


        static void WriteError(string message = "")
        {
            message = message ?? "";
            foreach (var line in StringExtensions.SplitLines(message)) Console.Error.WriteLine(line);
        }


        static string[] FormatHeading(char ruleCharacter, params string[] lines)
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


        static string FormatException(Exception ex)
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


        protected override void Handle(ProgramBannerEvent e)
        {
            WriteError();
            WriteError();
            WriteHeadingError(e.Lines);
        }


        protected override void Handle(ProgramUsageEvent e)
        {
            WriteError();
            WriteError(e.Lines);
            WriteError();
        }


        protected override void Handle(ProgramUserErrorEvent e)
        {
            WriteError();
            WriteError(e.Exception.Message);
        }


        protected override void Handle(ProgramInternalErrorEvent e)
        {
            WriteError();
            WriteError("An internal error occurred:");

            if (e.Exception is ReflectionTypeLoadException rtle)
            {
                foreach (var le in rtle.LoaderExceptions)
                {
                    WriteError(FormatException(le));
                }
            }

            WriteError(FormatException(e.Exception));

        }


        protected override void Handle(TestAssemblyBeginEvent e)
        {
            WriteOut();
            WriteHeadingOut(e.Path);
        }


        protected override void Handle(TestAssemblyNotFoundEvent e)
        {
            WriteOut();
            WriteOut($"Test assembly not found: {e.Path}");
        }


        protected override void Handle(TestAssemblyNotDotNetEvent e)
        {
            WriteOut();
            WriteOut($"Not a .NET assembly: {e.Path}");
        }


        protected override void Handle(TestAssemblyNotTestEvent e)
        {
            WriteOut();
            WriteOut($"Not a test assembly: {e.Path}");
        }


        protected override void Handle(TestAssemblyConfigFileSwitchedEvent e)
        {
            WriteOut();
            WriteOut("Configuration File:");
            WriteOut(e.Path);

            if (Type.GetType("Mono.Runtime") != null)
            {
                WriteOut();
                WriteOut("WARNING: Running on Mono, configuration file will probably not take effect");
                WriteOut("See https://bugzilla.xamarin.com/show_bug.cgi?id=15741");
            }

        }


        protected override void Handle(TestAssemblyEndEvent e)
        {
        }


        protected override void Handle(TestClassBeginEvent e)
        {
            WriteOut();
            WriteHeadingOut(e.FullName);
        }


        protected override void Handle(TestClassEndEvent e)
        {
            var initializeResult =
                e.InitializePresent
                    ? e.ClassIgnored
                        ? "Ignored"
                        : e.InitializeSucceeded
                            ? "Succeeded"
                            : "Failed"
                    : "Not present";

            var cleanupResult =
                e.CleanupPresent
                    ? e.ClassIgnored
                        ? "Ignored"
                        : e.CleanupSucceeded
                            ? "Succeeded"
                            : "Failed"
                    : "Not present";

            WriteOut();
            WriteSubheadingOut("Summary");

            if (e.ClassIgnored)
            {
                WriteOut();
                WriteOut("Ignored all tests because class is decorated with [Ignore]");
            }

            WriteOut();
            WriteOut($"ClassInitialize: {initializeResult}");
            WriteOut($"Total:           {e.TestsTotal} tests");
            WriteOut($"Ignored:         {e.TestsIgnored} tests");
            WriteOut($"Ran:             {e.TestsRan} tests");
            WriteOut($"Passed:          {e.TestsPassed} tests");
            WriteOut($"Failed:          {e.TestsFailed} tests");
            WriteOut($"ClassCleanup:    {cleanupResult}");
        }


        protected override void Handle(TestBeginEvent e)
        {
            WriteOut();
            WriteSubheadingOut(e.Name.Replace("_", " "));
        }


        protected override void Handle(TestIgnoredEvent e)
        {
            WriteOut();
            WriteOut("Ignored because [TestMethod] is decorated with [Ignore]");
        }


        protected override void Handle(TestEndEvent e)
        {
            WriteOut();
            switch (e.Outcome)
            {
                case UnitTestOutcome.NotRunnable:
                    WriteOut("Ignored");
                    break;
                case UnitTestOutcome.Passed:
                    WriteOut("Passed");
                    break;
                case UnitTestOutcome.Failed:
                    WriteOut("FAILED");
                    break;
                default:
                    throw new ArgumentException("Unexpected outcome", nameof(e));
            }
        }


        protected override void Handle(AssemblyInitializeMethodBeginEvent e)
        {
            WriteMethodBegin(e.TestAssembly.AssemblyInitializeMethod, "[AssemblyInitialize]");
        }


        protected override void Handle(AssemblyInitializeMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(AssemblyCleanupMethodBeginEvent e)
        {
            WriteMethodBegin(e.Method, "[AssemblyCleanup]");
        }


        protected override void Handle(AssemblyCleanupMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(ClassInitializeMethodBeginEvent e)
        {
            WriteMethodBegin(e.TestClass.ClassInitializeMethod, "[ClassInitialize]");
        }


        protected override void Handle(ClassInitializeMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(ClassCleanupMethodBeginEvent e)
        {
            WriteMethodBegin(e.Method, "[ClassCleanup]");
        }


        protected override void Handle(ClassCleanupMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(TestContextSetterBeginEvent e)
        {
            WriteMethodBegin(e.Method, "");
        }


        protected override void Handle(TestContextSetterEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(TestInitializeMethodBeginEvent e)
        {
            WriteMethodBegin(e.Method, "[TestInitialize]");
        }


        protected override void Handle(TestInitializeMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(TestMethodBeginEvent e)
        {
            WriteMethodBegin(e.Method, "[TestMethod]");
        }


        protected override void Handle(TestMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(TestCleanupMethodBeginEvent e)
        {
            WriteMethodBegin(e.Method, "[TestCleanup]");
        }


        protected override void Handle(TestCleanupMethodEndEvent e)
        {
            WriteMethodEnd(e.Success, e.ElapsedMilliseconds);
        }


        protected override void Handle(MethodExpectedExceptionEvent e)
        {
            WriteOut($"  [ExpectedException] {e.Expected.FullName} occurred:");
            WriteOut(StringExtensions.Indent(FormatException(e.Exception)));
        }


        protected override void Handle(MethodUnexpectedExceptionEvent e)
        {
            WriteOut(StringExtensions.Indent(FormatException(e.Exception)));
        }


        protected override void Handle(OutputTraceEvent e)
        {
            WriteOut(e.Message);
        }

    }
}
