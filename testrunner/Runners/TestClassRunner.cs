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
        /// <returns>
        /// Whether everything in <paramref name="testClass"/> succeeded
        /// </returns>
        ///
        static public bool Run(TestClass testClass)
        {
            Guard.NotNull(testClass, nameof(testClass));

            bool success = false;
            bool classCleanupSucceeded = false;

            EventHandlers.Raise(new TestClassBeginEvent() { FullName = testClass.FullName });

            do
            {
                //
                // Handle [Ignored] [TestClass]
                //
                if (testClass.IsIgnored)
                {
                    EventHandlers.Raise(new TestClassIgnoredEvent());
                    success = true;
                    break;
                }

                //
                // Run [ClassInitialize] method
                //
                if (!MethodRunner.RunClassInitializeMethod(testClass)) break;

                //
                // Run [TestMethod]s
                //
                var anyFailed = false;
                foreach (var testMethod in testClass.TestMethods)
                {
                    if (!TestMethodRunner.Run(testMethod)) anyFailed = true;
                }

                //
                // Run [ClassCleanup] method
                //
                classCleanupSucceeded = MethodRunner.RunClassCleanupMethod(testClass);

                success = !anyFailed && classCleanupSucceeded;
            }
            while (false);

            EventHandlers.Raise(new TestClassEndEvent());

            return success;
        }
        
    }
}
