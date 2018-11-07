using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that remembers the result of the last test assembly that ran
    /// </summary>
    ///
    public class LastTestAssemblyResultEventHandler : EventHandler
    {

        public TestAssemblyResult LastTestAssemblyResult { get; private set; }


        protected override void Handle(TestAssemblyEndEvent e)
        {
            LastTestAssemblyResult = e.Result;
        }

    }
}
