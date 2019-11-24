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
    public class TestClassResultEventHandler : ContextTrackingEventHandler
    {

        bool ignored;
        bool ignoredFromCommandLine;
        MethodResult classInitializeMethodResult;
        IList<TestResult> testResults;
        MethodResult classCleanupMethodResult;


        protected override void Handle(TestClassBeginEvent e)
        {
            base.Handle(e);
            ignored = false;
            ignoredFromCommandLine = false;
            classInitializeMethodResult = null;
            testResults = new List<TestResult>();
            classCleanupMethodResult = null;
        }


        protected override void Handle(TestClassIgnoredEvent e)
        {
            base.Handle(e);
            ignored = true;
            ignoredFromCommandLine = e.IgnoredFromCommandLine;
        }


        protected override void Handle(ClassInitializeMethodEndEvent e)
        {
            base.Handle(e);
            classInitializeMethodResult = e.Result;
        }


        protected override void Handle(TestEndEvent e)
        {
            base.Handle(e);
            testResults.Add(e.Result);
        }


        protected override void Handle(ClassCleanupMethodEndEvent e)
        {
            base.Handle(e);
            classCleanupMethodResult = e.Result;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            base.Handle(e);
            e.Result.TestAssemblyPath = CurrentTestAssemblyPath;
            e.Result.TestClassFullName = CurrentTestClassFullName;
            e.Result.Success = GetSuccess();
            e.Result.ClassIgnored = ignored;
            e.Result.ClassIgnoredFromCommandLine = ignoredFromCommandLine;
            e.Result.InitializePresent = classInitializeMethodResult != null;
            e.Result.InitializeSucceeded = classInitializeMethodResult?.Success ?? false;
            e.Result.TestsTotal = testResults.Count;
            e.Result.TestsRan = testResults.Count - testResults.Count(r => r.Ignored);
            e.Result.TestsIgnored = testResults.Count(r => r.Ignored);
            e.Result.TestsPassed = testResults.Count(r => !r.Ignored && r.Success);
            e.Result.TestsFailed = testResults.Count(r => !r.Success);
            e.Result.CleanupPresent = classCleanupMethodResult != null;
            e.Result.CleanupSucceeded = classCleanupMethodResult?.Success ?? false;
        }


        bool GetSuccess()
        {
            if (ignored) return true;
            if (classInitializeMethodResult?.Success == false) return false;
            if (testResults.Any(r => !r.Success)) return false;
            if (classCleanupMethodResult?.Success == false) return false;
            return true;
        }

    }
}
