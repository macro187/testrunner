using System;
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
    public class TestAssemblyResultEventHandler : EventHandler
    {

        bool isRunningTestAssembly;
        bool assemblyNotFound;
        bool assemblyNotDotNet;
        bool assemblyNotTest;
        string configFilePath;
        MethodResult assemblyInitializeResult;
        IList<TestClassResult> testClassResults;
        MethodResult assemblyCleanupResult;


        protected override void Handle(TestAssemblyBeginEvent e)
        {
            ExpectIsNotRunningTestAssembly();
            assemblyNotFound = false;
            assemblyNotDotNet = false;
            assemblyNotTest = false;
            configFilePath = null;
            assemblyInitializeResult = null;
            testClassResults = new List<TestClassResult>();
            assemblyCleanupResult = null;
            isRunningTestAssembly = true;
        }


        protected override void Handle(TestAssemblyNotFoundEvent e)
        {
            ExpectIsRunningTestAssembly();
            assemblyNotFound = true;
        }


        protected override void Handle(TestAssemblyNotDotNetEvent e)
        {
            ExpectIsRunningTestAssembly();
            assemblyNotDotNet = true;
        }


        protected override void Handle(TestAssemblyNotTestEvent e)
        {
            ExpectIsRunningTestAssembly();
            assemblyNotTest = true;
        }


        protected override void Handle(TestAssemblyConfigFileSwitchedEvent e)
        {
            ExpectIsRunningTestAssembly();
            configFilePath = e.Path;
        }


        protected override void Handle(AssemblyInitializeMethodEndEvent e)
        {
            ExpectIsRunningTestAssembly();
            assemblyInitializeResult = e.Result;
        }


        protected override void Handle(TestClassEndEvent e)
        {
            ExpectIsRunningTestAssembly();
            testClassResults.Add(e.Result);
        }


        protected override void Handle(AssemblyCleanupMethodEndEvent e)
        {
            ExpectIsRunningTestAssembly();
            assemblyCleanupResult = e.Result;
        }


        protected override void Handle(TestAssemblyEndEvent e)
        {
            ExpectIsRunningTestAssembly();
            e.Result.Success = GetSuccess();
            isRunningTestAssembly = false;
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


        void ExpectIsRunningTestAssembly()
        {
            if (!isRunningTestAssembly)
                throw new InvalidOperationException("Expected to be running a test assembly");
        }


        void ExpectIsNotRunningTestAssembly()
        {
            if (isRunningTestAssembly)
                throw new InvalidOperationException("Expected not to be running a test assembly");
        }

    }
}
