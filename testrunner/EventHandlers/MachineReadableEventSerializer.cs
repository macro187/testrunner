using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using TestRunner.MSTest;
using TestRunner.Infrastructure;
using TestRunner.Results;
using TestRunner.Events;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Serialize events to and from a single-line machine-readable JSON-based text format
    /// </summary>
    ///
    public static class MachineReadableEventSerializer
    {

        const string Prefix = "[TestRunnerEvent] ";

        static readonly DataContractJsonSerializerSettings JsonSerializerSettings =
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
                    typeof(TestRunBeginEvent),
                    typeof(TestRunEndEvent),
                    typeof(TestAssemblyBeginEvent),
                    typeof(TestAssemblyNotFoundEvent),
                    typeof(TestAssemblyNotDotNetEvent),
                    typeof(TestAssemblyNotTestEvent),
                    typeof(TestAssemblyConfigFileSwitchedEvent),
                    typeof(TestAssemblyEndEvent),
                    typeof(TestClassBeginEvent),
                    typeof(TestClassIgnoredEvent),
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
                    typeof(StandardOutputEvent),
                    typeof(ErrorOutputEvent),
                    typeof(TraceOutputEvent),
                },
            };


        /// <summary>
        /// Serialize a <see cref="TestRunnerEvent"/> into a line of text
        /// </summary>
        ///
        public static string Serialize(TestRunnerEvent e)
        {
            Guard.NotNull(e, nameof(e));
            var name = e.GetType().Name;
            var json = SerializeJson(e);
            return $"{Prefix}{name} {json}";
        }


        /// <summary>
        /// Try to deserialize a line of text into a <see cref="TestRunnerEvent"/>
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="TestRunnerEvent"/>
        /// - OR -
        /// <c>null</c> if <paramref name="line"/> does not appear to be a serialized event
        /// </returns>
        ///
        /// <exception cref="Exception">
        /// <paramref name="line"/> appears to be a serialized event but deserialization fails
        /// </exception>
        ///
        public static TestRunnerEvent TryDeserialize(string line)
        {
            Guard.NotNull(line, nameof(line));
            if (!line.StartsWith(Prefix, StringComparison.Ordinal)) return null;
            line = line.Substring(Prefix.Length);
            var i = line.IndexOf(' ');
            if (i < 0) throw new FormatException("No space separator in serialized event");
            var name = line.Substring(0, i);
            if (string.IsNullOrWhiteSpace(name)) throw new FormatException("No event name in serialized event");
            var json = line.Substring(i + 1);
            if (string.IsNullOrWhiteSpace(json)) throw new FormatException("No event data in serialized event");
            var type = Type.GetType($"TestRunner.Events.{name}");
            if (type == null) throw new FormatException($"Unknown serialized event '{name}'");
            return DeserializeJson(type, json);
        }


        static string SerializeJson(TestRunnerEvent e)
        {
            using (var stream = new MemoryStream())
            {
                GetSerializer(typeof(TestRunnerEvent)).WriteObject(stream, e);
                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
            }
        }


        static TestRunnerEvent DeserializeJson(Type type, string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (TestRunnerEvent)GetSerializer(type).ReadObject(stream);
            }
        }


        static DataContractJsonSerializer GetSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, JsonSerializerSettings);
        }

    }
}
