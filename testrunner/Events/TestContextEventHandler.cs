using System;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Domain;
using System.Linq;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that maintains <see cref="TestContext"/> state
    /// </summary>
    ///
    public class TestContextEventHandler : EventHandler
    {

        public override void ProgramBannerEvent(params string[] lines)
        {
            base.ProgramBannerEvent(lines);
        }


        public override void ProgramUsageEvent(string[] lines)
        {
            base.ProgramUsageEvent(lines);
        }


        public override void ProgramUserErrorEvent(UserException exception)
        {
            base.ProgramUserErrorEvent(exception);
        }


        public override void ProgramInternalErrorEvent(Exception exception)
        {
            base.ProgramInternalErrorEvent(exception);
        }


        public override void TestAssemblyBeginEvent(string path)
        {
            base.TestAssemblyBeginEvent(path);
        }


        public override void TestAssemblyNotFoundEvent(string path)
        {
            base.TestAssemblyNotFoundEvent(path);
        }


        public override void TestAssemblyNotDotNetEvent(string path)
        {
            base.TestAssemblyNotDotNetEvent(path);
        }


        public override void TestAssemblyNotTestEvent(string path)
        {
            base.TestAssemblyNotTestEvent(path);
        }


        public override void TestAssemblyConfigFileSwitchedEvent(string path)
        {
            base.TestAssemblyConfigFileSwitchedEvent(path);
        }


        public override void TestAssemblyEndEvent(bool success)
        {
            base.TestAssemblyEndEvent(success);
        }


        public override void TestClassBeginEvent(string fullName)
        {
            TestContext.FullyQualifiedTestClassName = fullName;
            base.TestClassBeginEvent(fullName);
        }


        public override void TestClassEndEvent(
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
            TestContext.FullyQualifiedTestClassName = null;
            base.TestClassEndEvent(
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


        public override void TestBeginEvent(string name)
        {
            TestContext.TestName = name;
            base.TestBeginEvent(name);
        }


        public override void TestIgnoredEvent()
        {
            base.TestIgnoredEvent();
        }


        public override void TestEndEvent(UnitTestOutcome outcome)
        {
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
            base.TestEndEvent(outcome);
        }


        public override void AssemblyInitializeMethodBeginEvent(TestAssembly testAssembly)
        {
            TestContext.FullyQualifiedTestClassName = testAssembly.TestClasses.First().FullName;
            TestContext.TestName = testAssembly.TestClasses.First().TestMethods.First().Name;
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
            base.AssemblyInitializeMethodBeginEvent(testAssembly);
        }


        public override void AssemblyInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            TestContext.FullyQualifiedTestClassName = null;
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
            base.AssemblyInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void AssemblyCleanupMethodBeginEvent(MethodInfo method)
        {
            base.AssemblyCleanupMethodBeginEvent(method);
        }


        public override void AssemblyCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            base.AssemblyCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassInitializeMethodBeginEvent(TestClass testClass)
        {
            TestContext.TestName = testClass.TestMethods.First().Name;
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
            base.ClassInitializeMethodBeginEvent(testClass);
        }


        public override void ClassInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
            base.ClassInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void ClassCleanupMethodBeginEvent(MethodInfo method)
        {
            base.ClassCleanupMethodBeginEvent(method);
        }


        public override void ClassCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            base.ClassCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestContextSetterBeginEvent(MethodInfo method)
        {
            base.TestContextSetterBeginEvent(method);
        }


        public override void TestContextSetterEndEvent(bool success, long elapsedMilliseconds)
        {
            base.TestContextSetterEndEvent(success, elapsedMilliseconds);
        }


        public override void TestInitializeMethodBeginEvent(MethodInfo method)
        {
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
            base.TestInitializeMethodBeginEvent(method);
        }


        public override void TestInitializeMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            base.TestInitializeMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestMethodBeginEvent(MethodInfo method)
        {
            base.TestMethodBeginEvent(method);
        }


        public override void TestMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            TestContext.CurrentTestOutcome = success ? UnitTestOutcome.Passed : UnitTestOutcome.Failed;
            base.TestMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void TestCleanupMethodBeginEvent(MethodInfo method)
        {
            base.TestCleanupMethodBeginEvent(method);
        }


        public override void TestCleanupMethodEndEvent(bool success, long elapsedMilliseconds)
        {
            base.TestCleanupMethodEndEvent(success, elapsedMilliseconds);
        }


        public override void MethodExpectedExceptionEvent(Type expected, Exception exception)
        {
            base.MethodExpectedExceptionEvent(expected, exception);
        }


        public override void MethodUnexpectedExceptionEvent(Exception exception)
        {
            base.MethodUnexpectedExceptionEvent(exception);
        }


        public override void OutputTraceEvent(string message = "")
        {
            base.OutputTraceEvent(message);
        }

    }
}
