using TestRunner.Events;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that tracks the currently-running assembly, class, and test
    /// </summary>
    ///
    public class ContextTrackingEventHandler : EventHandler
    {

        public string CurrentTestAssemblyPath { get; private set; }
        public string CurrentTestClassFullName { get; private set; }
        public string CurrentTestName { get; private set; }


        protected override void Handle(TestAssemblyBeginEvent e)
        {
            CurrentTestAssemblyPath = e.Path;
        }


        protected override void Handle(TestAssemblyEndEvent e)
        {
            CurrentTestAssemblyPath = null;
        }


        protected override void Handle(TestClassBeginEvent e)
        {
            CurrentTestClassFullName = e.FullName;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            CurrentTestClassFullName = null;
        }


        protected override void Handle(TestBeginEvent e)
        {
            CurrentTestName = e.Name;
        }


        protected override void Handle(TestEndEvent e)
        {
            CurrentTestName = null;
        }

    }
}
