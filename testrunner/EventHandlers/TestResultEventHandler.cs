using System;
using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that accumulates test execution information and populates <see cref="TestEndEvent"/> results
    /// </summary>
    ///
    public class TestResultEventHandler : EventHandler
    {

        bool isRunningTest;
        bool ignored;
        MethodResult testInitializeMethodResult;
        MethodResult testMethodResult;
        MethodResult testCleanupMethodResult;


        protected override void Handle(TestBeginEvent e)
        {
            ExpectIsNotRunningTest();
            ignored = false;
            testInitializeMethodResult = null;
            testMethodResult = null;
            testCleanupMethodResult = null;
            isRunningTest = true;
        }


        protected override void Handle(TestIgnoredEvent e)
        {
            ExpectIsRunningTest();
            ignored = true;
        }


        protected override void Handle(TestInitializeMethodEndEvent e)
        {
            ExpectIsRunningTest();
            testInitializeMethodResult = e.Result;
        }


        protected override void Handle(TestMethodEndEvent e)
        {
            ExpectIsRunningTest();
            testMethodResult = e.Result;
        }


        protected override void Handle(TestCleanupMethodEndEvent e)
        {
            ExpectIsRunningTest();
            testCleanupMethodResult = e.Result;
        }


        protected override void Handle(TestEndEvent e)
        {
            ExpectIsRunningTest();

            var success = false;
            if (ignored) success = true;
            if (testInitializeMethodResult?.Success == false) success = false;
            if (testMethodResult?.Success == true) success = true;
            if (testCleanupMethodResult?.Success == false) success = false;

            e.Result.Ignored = ignored;
            e.Result.Success = success;

            isRunningTest = false;
        }


        void ExpectIsRunningTest()
        {
            if (!isRunningTest) throw new InvalidOperationException("Expected to be running a test");
        }


        void ExpectIsNotRunningTest()
        {
            if (isRunningTest) throw new InvalidOperationException("Expected not to be running a test");
        }

    }
}
