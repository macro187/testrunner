using System;
using System.Collections.Generic;
using System.Linq;
using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that accumulates test class execution information and populates <see cref="TestClassEndEvent"/>
    /// results
    /// </summary>
    ///
    public class TestClassResultEventHandler : EventHandler
    {

        bool isRunningTestClass;
        bool ignored;
        MethodResult classInitializeMethodResult;
        IList<TestResult> testResults;
        MethodResult classCleanupMethodResult;


        protected override void Handle(TestClassBeginEvent e)
        {
            ExpectIsNotRunningTestClass();
            ignored = false;
            classInitializeMethodResult = null;
            testResults = new List<TestResult>();
            isRunningTestClass = true;
        }


        protected override void Handle(TestClassIgnoredEvent e)
        {
            ExpectIsRunningTestClass();
            ignored = true;
        }


        protected override void Handle(ClassInitializeMethodEndEvent e)
        {
            ExpectIsRunningTestClass();
            classInitializeMethodResult = e.Result;
        }


        protected override void Handle(TestEndEvent e)
        {
            ExpectIsRunningTestClass();
            testResults.Add(e.Result);
        }


        protected override void Handle(ClassCleanupMethodEndEvent e)
        {
            ExpectIsRunningTestClass();
            classCleanupMethodResult = e.Result;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            ExpectIsRunningTestClass();

            var success = false;
            if (ignored) success = true;
            if (classInitializeMethodResult?.Success == false) success = false;
            if (testResults.Any() && testResults.All(r => r.Success)) success = true;
            if (classCleanupMethodResult?.Success == false) success = false;

            e.Result.Success = success;
            e.Result.ClassIgnored = ignored;
            e.Result.InitializePresent = classInitializeMethodResult != null;
            e.Result.InitializeSucceeded = classInitializeMethodResult?.Success ?? false;
            e.Result.TestsTotal = testResults.Count;
            e.Result.TestsRan = testResults.Count - testResults.Count(r => r.Ignored);
            e.Result.TestsIgnored = testResults.Count(r => r.Ignored);
            e.Result.TestsPassed = testResults.Count(r => !r.Ignored && r.Success);
            e.Result.TestsFailed = testResults.Count(r => !r.Success);
            e.Result.CleanupPresent = classCleanupMethodResult != null;
            e.Result.CleanupSucceeded = classCleanupMethodResult?.Success ?? false;

            isRunningTestClass = false;
        }


        void ExpectIsRunningTestClass()
        {
            if (!isRunningTestClass) throw new InvalidOperationException("Expected to be running a test class");
        }


        void ExpectIsNotRunningTestClass()
        {
            if (isRunningTestClass) throw new InvalidOperationException("Expected not to be running a test class");
        }

    }
}
