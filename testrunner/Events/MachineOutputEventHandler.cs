using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using TestRunner.Domain;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that outputs events in a machine-readable JSON-based format
    /// </summary>
    ///
    public class MachineOutputEventHandler : EventHandler
    {

        static void WriteOut(string message = "")
        {
            message = message ?? "";
            foreach (var line in StringExtensions.SplitLines(message)) Console.Out.WriteLine(line);
        }


        public override void Handle(TestRunnerEvent e)
        {
            var serializer = new DataContractJsonSerializer(
                typeof(TestRunnerEvent),
                new DataContractJsonSerializerSettings() {
                    DateTimeFormat = new DateTimeFormat("o"), // ISO8601
                    EmitTypeInformation = EmitTypeInformation.Never,
                    KnownTypes = new[]{
                        typeof(UnitTestOutcome),
                        typeof(ExceptionInfo),
                        typeof(StackFrameInfo),
                        typeof(TestRunnerEvent),
                        typeof(ProgramBannerEvent),
                        typeof(ProgramUsageEvent),
                        typeof(ProgramUserErrorEvent),
                        typeof(ProgramInternalErrorEvent),
                        typeof(TestAssemblyBeginEvent),
                        typeof(TestAssemblyNotFoundEvent),
                        typeof(TestAssemblyNotDotNetEvent),
                        typeof(TestAssemblyNotTestEvent),
                        typeof(TestAssemblyConfigFileSwitchedEvent),
                        typeof(TestAssemblyEndEvent),
                        typeof(TestClassBeginEvent),
                        typeof(TestClassEndEvent),
                        typeof(TestBeginEvent),
                        typeof(TestIgnoredEvent),
                        typeof(TestEndEvent),
                        typeof(AssemblyInitializeMethodBeginEvent),
                        typeof(AssemblyInitializeMethodEndEvent),
                        typeof(AssemblyCleanupMethodBeginEvent),
                        typeof(AssemblyCleanupMethodEndEvent),
                        typeof(ClassInitializeMethodBeginEvent),
                        typeof(ClassInitializeMethodEndEvent),
                        typeof(ClassCleanupMethodBeginEvent),
                        typeof(ClassCleanupMethodEndEvent),
                        typeof(TestContextSetterBeginEvent),
                        typeof(TestContextSetterEndEvent),
                        typeof(TestInitializeMethodBeginEvent),
                        typeof(TestInitializeMethodEndEvent),
                        typeof(TestMethodBeginEvent),
                        typeof(TestMethodEndEvent),
                        typeof(TestCleanupMethodBeginEvent),
                        typeof(TestCleanupMethodEndEvent),
                        typeof(MethodExpectedExceptionEvent),
                        typeof(MethodUnexpectedExceptionEvent),
                        typeof(OutputTraceEvent),
                    },
                });

            string json;
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, e);
                json = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
            }

            var name = e.GetType().Name;
            var pid = Process.GetCurrentProcess().Id;
            var prefix = $"[{name}:{pid}]";
            WriteOut($"{prefix} {json}");
            
            base.Handle(e);
        }

    }
}
