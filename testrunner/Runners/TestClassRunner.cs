using TestRunner.MSTest;
using TestRunner.Events;
using TestRunner.Infrastructure;
using TestRunner.EventHandlers;
using TestRunner.Program;

namespace TestRunner.Runners
{
    static class TestClassRunner
    {

        /// <summary>
        /// Run tests in a [TestClass]
        /// </summary>
        ///
        static public void Run(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));

            EventHandlerPipeline.Raise(new TestClassBeginEvent() { FullName = testClass.FullName });

            do
            {
                //
                // Handle exclusion from the command line
                //
                if (!ArgumentParser.ClassShouldRun(testClass.FullName))
                {
                    EventHandlerPipeline.Raise(new TestClassIgnoredEvent() { IgnoredFromCommandLine = true });
                    break;
                }

                //
                // Handle [Ignored] [TestClass]
                //
                if (testClass.IsIgnored)
                {
                    EventHandlerPipeline.Raise(new TestClassIgnoredEvent());
                    break;
                }

                //
                // Run [ClassInitialize] method
                //
                if (!MethodRunner.RunClassInitializeMethod(testClass)) break;

                //
                // Run [TestMethod]s
                //
                foreach (var testMethod in testClass.TestMethods)
                {
                    TestMethodRunner.Run(testMethod);
                }

                //
                // Run [ClassCleanup] method
                //
                MethodRunner.RunClassCleanupMethod(testClass);
            }
            while (false);

            EventHandlerPipeline.Raise(new TestClassEndEvent());
        }
        
    }
}
