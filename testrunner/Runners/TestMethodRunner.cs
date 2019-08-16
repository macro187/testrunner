using System;
using TestRunner.MSTest;
using TestRunner.Events;
using TestRunner.EventHandlers;
using TestRunner.Program;

namespace TestRunner.Runners
{
    static class TestMethodRunner
    {

        /// <summary>
        /// Run a test method (plus its intialize and cleanup methods, if present)
        /// </summary>
        ///
        /// <remarks>
        /// If the test method is decorated with [Ignore], nothing is run
        /// </remarks>
        ///
        static public void Run(TestMethod testMethod)
        {
            EventHandlerPipeline.Raise(new TestBeginEvent() { Name = testMethod.Name });

            do
            {
                //
                // Handle exclusion from the command line
                //
                if (!ArgumentParser.MethodShouldRun(testMethod.FullName))
                {
                    EventHandlerPipeline.Raise(new TestIgnoredEvent() { IgnoredFromCommandLine = true });
                    break;
                }

                //
                // Handle [Ignored] [TestMethod]
                //
                if (testMethod.IsIgnored)
                {
                    EventHandlerPipeline.Raise(new TestIgnoredEvent());
                    break;
                }

                //
                // Create instance of [TestClass]
                //
                var instance = Activator.CreateInstance(testMethod.TestClass.Type);

                //
                // Set TestContext property (if present)
                //
                MethodRunner.RunTestContextSetter(testMethod.TestClass, instance);

                //
                // Run [TestInitialize] method
                //
                if (!MethodRunner.RunTestInitializeMethod(testMethod.TestClass, instance)) break;

                //
                // Run [TestMethod]
                //
                MethodRunner.RunTestMethod(testMethod, instance);

                //
                // Run [TestCleanup] method
                //
                MethodRunner.RunTestCleanupMethod(testMethod.TestClass, instance);
            }
            while (false);

            EventHandlerPipeline.Raise(new TestEndEvent());
        }
        
    }
}
