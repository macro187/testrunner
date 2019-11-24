using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that accumulates test execution information and populates <see cref="TestEndEvent"/> results
    /// </summary>
    ///
    public class TestResultEventHandler : ContextTrackingEventHandler
    {

        bool ignored;
        bool ignoredFromCommandLine;
        MethodResult testInitializeMethodResult;
        MethodResult testMethodResult;
        MethodResult testCleanupMethodResult;


        protected override void Handle(TestBeginEvent e)
        {
            base.Handle(e);
            ignored = false;
            ignoredFromCommandLine = false;
            testInitializeMethodResult = null;
            testMethodResult = null;
            testCleanupMethodResult = null;
        }


        protected override void Handle(TestIgnoredEvent e)
        {
            base.Handle(e);
            ignored = true;
            ignoredFromCommandLine = e.IgnoredFromCommandLine;
        }


        protected override void Handle(TestInitializeMethodEndEvent e)
        {
            base.Handle(e);
            testInitializeMethodResult = e.Result;
        }


        protected override void Handle(TestMethodEndEvent e)
        {
            base.Handle(e);
            testMethodResult = e.Result;
        }


        protected override void Handle(TestCleanupMethodEndEvent e)
        {
            base.Handle(e);
            testCleanupMethodResult = e.Result;
        }


        protected override void Handle(TestEndEvent e)
        {
            base.Handle(e);
            e.Result.TestAssemblyPath = CurrentTestAssemblyPath;
            e.Result.TestClassFullName = CurrentTestClassFullName;
            e.Result.TestName = CurrentTestName;
            e.Result.Success = GetSuccess();
            e.Result.Ignored = ignored;
            e.Result.IgnoredFromCommandLine = ignoredFromCommandLine;
        }


        bool GetSuccess()
        {
            if (ignored) return true;
            if (testInitializeMethodResult?.Success == false) return false;
            if (testMethodResult?.Success == false) return false;
            if (testCleanupMethodResult?.Success == false) return false;
            return true;
        }

    }
}
