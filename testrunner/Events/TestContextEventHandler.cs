using TestRunner.MSTest;
using System.Linq;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler that maintains <see cref="TestContext"/> state
    /// </summary>
    ///
    public class TestContextEventHandler : EventHandler
    {

        protected override void Handle(TestClassBeginEvent e)
        {
            TestContext.FullyQualifiedTestClassName = e.FullName;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            TestContext.FullyQualifiedTestClassName = null;
        }


        protected override void Handle(TestBeginEvent e)
        {
            TestContext.TestName = e.Name;
        }


        protected override void Handle(TestEndEvent e)
        {
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
        }


        protected override void Handle(AssemblyInitializeMethodBeginEvent e)
        {
            TestContext.FullyQualifiedTestClassName = e.FirstTestClassFullName;
            TestContext.TestName = e.FirstTestMethodName;
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
        }


        protected override void Handle(AssemblyInitializeMethodEndEvent e)
        {
            TestContext.FullyQualifiedTestClassName = null;
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
        }


        protected override void Handle(ClassInitializeMethodBeginEvent e)
        {
            TestContext.TestName = e.FirstTestMethodName;
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
        }


        protected override void Handle(ClassInitializeMethodEndEvent e)
        {
            TestContext.TestName = null;
            TestContext.CurrentTestOutcome = UnitTestOutcome.Unknown;
        }


        protected override void Handle(TestInitializeMethodBeginEvent e)
        {
            TestContext.CurrentTestOutcome = UnitTestOutcome.InProgress;
        }


        protected override void Handle(TestMethodEndEvent e)
        {
            TestContext.CurrentTestOutcome = e.Result.Success ? UnitTestOutcome.Passed : UnitTestOutcome.Failed;
        }

    }
}
