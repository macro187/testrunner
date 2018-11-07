using System.Collections.Generic;
using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that remembers results
    /// </summary>
    ///
    public class ResultAccumulatingEventHandler : EventHandler
    {

        public IList<TestResult> TestResults { get; private set; } = new List<TestResult>();
        public IList<TestClassResult> TestClassResults { get; private set; } = new List<TestClassResult>();
        public IList<TestAssemblyResult> TestAssemblyResults { get; private set; } = new List<TestAssemblyResult>();
        public IList<TestRunResult> TestRunResults { get; private set; } = new List<TestRunResult>();


        protected override void Handle(TestEndEvent e)
        {
            TestResults.Add(e.Result);
        }


        protected override void Handle(TestClassEndEvent e)
        {
            TestClassResults.Add(e.Result);
        }


        protected override void Handle(TestAssemblyEndEvent e)
        {
            TestAssemblyResults.Add(e.Result);
        }


        protected override void Handle(TestRunEndEvent e)
        {
            TestRunResults.Add(e.Result);
        }

    }
}
