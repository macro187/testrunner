using System;
using System.Reflection;
using TestRunner.Domain;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler pipeline
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This class serves as a "no-op" handler implementation and as the base class for other handlers.
    /// </para>
    ///
    /// <para>
    /// The static members of this class implement the handler pipeline, to which handlers can be
    /// <see cref="Prepend(EventHandler)"/>ed or <see cref="Append(EventHandler)"/>ed.
    /// </para>
    ///
    /// <para>
    /// The TestRunner program invokes event methods on the <see cref="First"/> handler as it runs.  Those methods
    /// have the opportunity to perform actions before propagating to the next handler in the pipeline.
    /// </para>
    /// </remarks>
    ///
    public class EventHandler
    {

        static EventHandler()
        {
            Append(new EventHandler());
        }


        /// <summary>
        /// The first event handler in the pipeline
        /// </summary>
        ///
        public static EventHandler First { get; private set; }


        private static EventHandler Last;


        /// <summary>
        /// Add an event handler to the beginning of the pipeline
        /// </summary>
        ///
        public static void Prepend(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            handler.Next = First;
            First = handler;
            if (Last == null) Last = handler;
        }


        /// <summary>
        /// Add an event handler to the end of the pipeline
        /// </summary>
        ///
        public static void Append(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            if (First == null) First = handler;
            if (Last != null) Last.Next = handler;
            Last = handler;
        }


        private EventHandler Next = null;


        public virtual void ProgramBannerEvent(params string[] lines)
        {
            Next?.ProgramBannerEvent(lines);
        }


        public virtual void ProgramUsageEvent(string[] lines)
        {
            Next?.ProgramUsageEvent(lines);
        }


        public virtual void ProgramUserErrorEvent(UserException exception)
        {
            Next?.ProgramUserErrorEvent(exception);
        }


        public virtual void ProgramInternalErrorEvent(Exception exception)
        {
            Next?.ProgramInternalErrorEvent(exception);
        }


        public virtual void TestAssemblyBeginEvent(string path)
        {
            Next?.TestAssemblyBeginEvent(path);
        }


        public virtual void TestAssemblyNotFoundEvent(string path)
        {
            Next?.TestAssemblyNotFoundEvent(path);
        }


        public virtual void TestAssemblyNotDotNetEvent(string path)
        {
            Next?.TestAssemblyNotDotNetEvent(path);
        }


        public virtual void TestAssemblyNotTestEvent(string path)
        {
            Next?.TestAssemblyNotTestEvent(path);
        }


        public virtual void TestAssemblyConfigFileSwitchedEvent(string path)
        {
            Next?.TestAssemblyConfigFileSwitchedEvent(path);
        }


        public virtual void TestAssemblyEndEvent(bool success)
        {
            Next?.TestAssemblyEndEvent(success);
        }


        public virtual void TestClassBeginEvent(string fullName)
        {
            Next?.TestClassBeginEvent(fullName);
        }


        public virtual void TestClassEndEvent(
            bool success,
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
            Next?.TestClassEndEvent(
                success,
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


        public virtual void TestBeginEvent(string name)
        {
            Next?.TestBeginEvent(name);
        }


        public virtual void TestIgnoredEvent()
        {
            Next?.TestIgnoredEvent();
        }


        public virtual void TestEndEvent(UnitTestOutcome outcome)
        {
            Next?.TestEndEvent(outcome);
        }


        public virtual void AssemblyInitializeMethodBeginEvent(TestAssembly testAssembly)
        {
            Next?.AssemblyInitializeMethodBeginEvent(testAssembly);
        }


        public virtual void AssemblyInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.AssemblyInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void AssemblyCleanupMethodBeginEvent(MethodInfo method)
        {
            Next?.AssemblyCleanupMethodBeginEvent(method);
        }


        public virtual void AssemblyCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.AssemblyCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void ClassInitializeMethodBeginEvent(TestClass testClass)
        {
            Next?.ClassInitializeMethodBeginEvent(testClass);
        }


        public virtual void ClassInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.ClassInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void ClassCleanupMethodBeginEvent(MethodInfo method)
        {
            Next?.ClassCleanupMethodBeginEvent(method);
        }


        public virtual void ClassCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.ClassCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void TestContextSetterBeginEvent(MethodInfo method)
        {
            Next?.TestContextSetterBeginEvent(method);
        }


        public virtual void TestContextSetterEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.TestContextSetterEndEvent(success, elapsedMilliseconds);
        }


        public virtual void TestInitializeMethodBeginEvent(MethodInfo method)
        {
            Next?.TestInitializeMethodBeginEvent(method);
        }


        public virtual void TestInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.TestInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void TestMethodBeginEvent(MethodInfo method)
        {
            Next?.TestMethodBeginEvent(method);
        }


        public virtual void TestMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.TestMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void TestCleanupMethodBeginEvent(MethodInfo method)
        {
            Next?.TestCleanupMethodBeginEvent(method);
        }


        public virtual void TestCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            Next?.TestCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public virtual void MethodExpectedExceptionEvent(Type expected, Exception exception)
        {
            Next?.MethodExpectedExceptionEvent(expected, exception);
        }


        public virtual void MethodUnexpectedExceptionEvent(Exception exception)
        {
            Next?.MethodUnexpectedExceptionEvent(exception);
        }


        public virtual void OutputTraceEvent(string message = "")
        {
            Next?.OutputTraceEvent(message);
        }

    }
}
