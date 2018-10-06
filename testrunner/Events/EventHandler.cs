using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.Events
{
    public class EventHandler
    {

        public EventHandler Next { get; set; }


        public void Handle(TestRunnerEvent e)
        {
            Guard.NotNull(e, nameof(e));

            var method = GetType().GetMethod(
                "Handle",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[]{ e.GetType() },
                null);

            method.Invoke(this, new[]{ e });

            Next?.Handle(e);
        }


        protected virtual void Handle(ProgramBannerEvent e) {}
        protected virtual void Handle(ProgramUsageEvent e) {}
        protected virtual void Handle(ProgramUserErrorEvent e) {}
        protected virtual void Handle(ProgramInternalErrorEvent e) {}
        protected virtual void Handle(TestAssemblyBeginEvent e) {}
        protected virtual void Handle(TestAssemblyNotFoundEvent e) {}
        protected virtual void Handle(TestAssemblyNotDotNetEvent e) {}
        protected virtual void Handle(TestAssemblyNotTestEvent e) {}
        protected virtual void Handle(TestAssemblyConfigFileSwitchedEvent e) {}
        protected virtual void Handle(TestAssemblyEndEvent e) {}
        protected virtual void Handle(TestClassBeginEvent e) {}
        protected virtual void Handle(TestClassEndEvent e) {}
        protected virtual void Handle(TestBeginEvent e) {}
        protected virtual void Handle(TestIgnoredEvent e) {}
        protected virtual void Handle(TestEndEvent e) {}
        protected virtual void Handle(AssemblyInitializeMethodBeginEvent e) {}
        protected virtual void Handle(AssemblyInitializeMethodEndEvent e) {}
        protected virtual void Handle(AssemblyCleanupMethodBeginEvent e) {}
        protected virtual void Handle(AssemblyCleanupMethodEndEvent e) {}
        protected virtual void Handle(ClassInitializeMethodBeginEvent e) {}
        protected virtual void Handle(ClassInitializeMethodEndEvent e) {}
        protected virtual void Handle(ClassCleanupMethodBeginEvent e) {}
        protected virtual void Handle(ClassCleanupMethodEndEvent e) {}
        protected virtual void Handle(TestContextSetterBeginEvent e) {}
        protected virtual void Handle(TestContextSetterEndEvent e) {}
        protected virtual void Handle(TestInitializeMethodBeginEvent e) {}
        protected virtual void Handle(TestInitializeMethodEndEvent e) {}
        protected virtual void Handle(TestMethodBeginEvent e) {}
        protected virtual void Handle(TestMethodEndEvent e) {}
        protected virtual void Handle(TestCleanupMethodBeginEvent e) {}
        protected virtual void Handle(TestCleanupMethodEndEvent e) {}
        protected virtual void Handle(MethodExpectedExceptionEvent e) {}
        protected virtual void Handle(MethodUnexpectedExceptionEvent e) {}
        protected virtual void Handle(OutputTraceEvent e) {}

    }
}
