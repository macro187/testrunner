using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using TestRunner.Domain;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that outputs events in a machine-readable JSON-based format
    /// </summary>
    ///
    public class MachineOutputEventHandler : EventHandler
    {

        readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(
            typeof(TestRunnerEvent),
            new DataContractJsonSerializerSettings() {
                EmitTypeInformation = EmitTypeInformation.Never,
                DateTimeFormat = new DateTimeFormat("o"), // ISO8601
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


        public override void Handle(TestRunnerEvent e)
        {
            var name = e.GetType().Name;
            var json = Serialize(e);
            Console.Out.WriteLine($"{name} {json}");
            base.Handle(e);
        }


        string Serialize(TestRunnerEvent e)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.WriteObject(stream, e);
                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
            }
        }

    }
}
