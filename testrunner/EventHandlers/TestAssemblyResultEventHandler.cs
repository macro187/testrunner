using System.Collections.Generic;
using System.Linq;
using TestRunner.Events;
using TestRunner.Results;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that accumulates test assembly execution information and populates
    /// <see cref="TestAssemblyEndEvent"/> results
    /// </summary>
    ///
    public class TestAssemblyResultEventHandler : ContextTrackingEventHandler
    {

        bool assemblyNotFound;
        bool assemblyNotDotNet;
        bool assemblyNotTest;
        string configFilePath;
        MethodResult assemblyInitializeResult;
        IList<TestClassResult> testClassResults;
        MethodResult assemblyCleanupResult;


        protected override void Handle(TestAssemblyBeginEvent e)
        {
            base.Handle(e);
            assemblyNotFound = false;
            assemblyNotDotNet = false;
            assemblyNotTest = false;
            configFilePath = null;
            assemblyInitializeResult = null;
            testClassResults = new List<TestClassResult>();
            assemblyCleanupResult = null;
        }


        protected override void Handle(TestAssemblyNotFoundEvent e)
        {
            base.Handle(e);
            assemblyNotFound = true;
        }


        protected override void Handle(TestAssemblyNotDotNetEvent e)
        {
            base.Handle(e);
            assemblyNotDotNet = true;
        }


        protected override void Handle(TestAssemblyNotTestEvent e)
        {
            base.Handle(e);
            assemblyNotTest = true;
        }


        protected override void Handle(TestAssemblyConfigFileSwitchedEvent e)
        {
            base.Handle(e);
            configFilePath = e.Path;
        }


        protected override void Handle(AssemblyInitializeMethodEndEvent e)
        {
            base.Handle(e);
            assemblyInitializeResult = e.Result;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            base.Handle(e);
            testClassResults.Add(e.Result);
        }


        protected override void Handle(AssemblyCleanupMethodEndEvent e)
        {
            base.Handle(e);
            assemblyCleanupResult = e.Result;
        }


        protected override void Handle(TestAssemblyEndEvent e)
        {
            base.Handle(e);
            e.Result.TestAssemblyPath = CurrentTestAssemblyPath;
            e.Result.Success = GetSuccess();
        }


        bool GetSuccess()
        {
            if (assemblyNotFound) return false;
            if (assemblyNotDotNet) return true;
            if (assemblyNotTest) return true;
            if (assemblyInitializeResult?.Success == false) return false;
            if (testClassResults.Any(r => !r.Success)) return false;
            if (assemblyCleanupResult?.Success == false) return false;
            return true;
        }

    }
}
