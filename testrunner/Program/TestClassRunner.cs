using TestRunner.Domain;
using TestRunner.Infrastructure;
using static TestRunner.Infrastructure.ConsoleExtensions;

namespace TestRunner.Program
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
            Guard.NotNull(testClass, "testClass");

            WriteLine();
            WriteHeading(testClass.FullName);

            bool classInitializeSucceeded = false;
            int ran = 0;
            int passed = 0;
            int failed = 0;
            int ignored = 0;
            bool classCleanupSucceeded = false;

            if (testClass.IsIgnored)
            {
                WriteLine();
                WriteLine("Ignoring all tests because class is decorated with [Ignore]");
                ignored = testClass.TestMethods.Count;
            }
            else
            {
                //
                // Run [ClassInitialize] method
                //
                classInitializeSucceeded =
                    MethodRunner.Run(
                        testClass.ClassInitializeMethod, null,
                        true,
                        null, false,
                        "[ClassInitialize]");

                if (classInitializeSucceeded)
                {
                    //
                    // Run [TestMethod]s
                    //
                    foreach (var testMethod in testClass.TestMethods)
                    {
                        switch(
                            TestMethodRunner.Run(
                                testMethod,
                                testClass.TestInitializeMethod,
                                testClass.TestCleanupMethod,
                                testClass))
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
                    classCleanupSucceeded =
                        MethodRunner.Run(
                            testClass.ClassCleanupMethod, null,
                            false,
                            null, false,
                            "[ClassCleanup]");
                }
            }

            //
            // Print results
            //
            WriteSubheading("Summary");
            WriteLine();
            WriteLine("ClassInitialize: {0}",
                testClass.ClassInitializeMethod == null
                    ? "Not present"
                    : classInitializeSucceeded
                        ? "Succeeded"
                        : "Failed");
            WriteLine("Total:           {0} tests", testClass.TestMethods.Count);
            WriteLine("Ran:             {0} tests", ran);
            WriteLine("Ignored:         {0} tests", ignored);
            WriteLine("Passed:          {0} tests", passed);
            WriteLine("Failed:          {0} tests", failed);
            WriteLine("ClassCleanup:    {0}",
                testClass.ClassCleanupMethod == null
                    ? "Not present"
                    : classCleanupSucceeded
                        ? "Succeeded"
                        : "Failed");

            return
                classInitializeSucceeded &&
                failed == 0 &&
                classCleanupSucceeded;
        }
        
    }
}
