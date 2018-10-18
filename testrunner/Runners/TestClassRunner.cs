using TestRunner.MSTest;
using TestRunner.Events;
using TestRunner.Infrastructure;

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

            EventHandlers.Raise(new TestClassBeginEvent() { FullName = testClass.FullName });

            do
            {
                //
                // Handle [Ignored] [TestClass]
                //
                if (testClass.IsIgnored)
                {
                    EventHandlers.Raise(new TestClassIgnoredEvent());
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

            EventHandlers.Raise(new TestClassEndEvent());
        }
        
    }
}
