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
            bool classIgnored = false;
            bool classInitializeSucceeded = false;
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            bool classCleanupSucceeded = false;

            EventHandlers.Raise(new TestClassBeginEvent() { FullName = testClass.FullName });

            do
            {
                //
                // Handle [Ignored] [TestClass]
                //
                if (testClass.IsIgnored)
                {
                    classIgnored = true;
                    ignored = testClass.TestMethods.Count;
                    success = true;
                    break;
                }

                //
                // Run [ClassInitialize] method
                //
                classInitializeSucceeded = MethodRunner.RunClassInitializeMethod(testClass);
                if (!classInitializeSucceeded) break;

                //
                // Run [TestMethod]s
                //
                foreach (var testMethod in testClass.TestMethods)
                {
                    switch(TestMethodRunner.Run(testMethod))
                    {
                        case UnitTestOutcome.Passed:
                            passed++;
                            ran++;
                            break;
                        case UnitTestOutcome.Failed:
                            failed++;
                            ran++;
                            break;
                        case UnitTestOutcome.NotRunnable:
                            ignored++;
                            break;
                    }
                }

                //
                // Run [ClassCleanup] method
                //
                classCleanupSucceeded = MethodRunner.RunClassCleanupMethod(testClass);

                success = failed == 0 && classCleanupSucceeded;
            }
            while (false);

            EventHandlers.Raise(
                new TestClassEndEvent() {
                    Success = success,
                    ClassIgnored = classIgnored,
                    InitializePresent = testClass.ClassInitializeMethod != null,
                    InitializeSucceeded = classInitializeSucceeded,
                    TestsTotal = testClass.TestMethods.Count,
                    TestsRan = ran,
                    TestsIgnored = ignored,
                    TestsPassed = passed,
                    TestsFailed = failed,
                    CleanupPresent = testClass.ClassCleanupMethod != null,
                    CleanupSucceeded = classCleanupSucceeded
                });

            return success;
        }
        
    }
}
