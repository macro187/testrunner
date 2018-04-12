using System;
using System.Reflection;
#if NET461
using System.Configuration;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestRunner.Tests.ReferencedAssembly;

namespace TestRunner.Tests.MSTest
{

    [TestClass]
    public partial class MSTestTests
    {

        static readonly string FullyQualifiedTestClassName = typeof(MSTestTests).FullName;

        static bool assemblyInitializeRan = false;
        static bool assemblyInitializeReceivedTestContext = false;
        static string assemblyInitializeTestName;
        static string assemblyInitializeFullyQualifiedTestClassName;
        static UnitTestOutcome? assemblyInitializeCurrentTestOutcome;

        static int classInitializeCount = 0;
        static bool classInitializeReceivedTestContext = false;
        static string classInitializeTestName;
        static string classInitializeFullyQualifiedTestClassName;
        static UnitTestOutcome? classInitializeCurrentTestOutcome;

        static int testInitializeCount = 0;
        bool testInitializeTestContextAvailable;
        string testInitializeTestName;
        string testInitializeFullyQualifiedTestClassName;
        UnitTestOutcome? testInitializeCurrentTestOutcome;

        static int testCleanupCount = 0;
        static UnitTestOutcome? testCleanupCurrentTestOutcome;

        bool isInstanceNew  = true;
        object isInstanceNewLock = new object();


        public TestContext TestContext { get; set; }


        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            assemblyInitializeReceivedTestContext = testContext != null;
            assemblyInitializeTestName = testContext?.TestName;
            assemblyInitializeFullyQualifiedTestClassName = testContext?.FullyQualifiedTestClassName;
            assemblyInitializeCurrentTestOutcome = testContext?.CurrentTestOutcome;
            assemblyInitializeRan = true;
        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            classInitializeReceivedTestContext = testContext != null;
            classInitializeTestName = testContext?.TestName;
            classInitializeFullyQualifiedTestClassName = testContext?.FullyQualifiedTestClassName;
            classInitializeCurrentTestOutcome = testContext?.CurrentTestOutcome;
            classInitializeCount++;
        }


        [TestInitialize]
        public void TestInitialize()
        {
            testInitializeTestContextAvailable = TestContext != null;
            testInitializeTestName = TestContext?.TestName;
            testInitializeFullyQualifiedTestClassName = TestContext?.FullyQualifiedTestClassName;
            testInitializeCurrentTestOutcome = TestContext?.CurrentTestOutcome;
            testInitializeCount++;
        }


        [TestMethod]
        public void AssemblyInitialize_Runs()
        {
            Assert.IsTrue(assemblyInitializeRan, "[AssemblyInitialize] method did not run");
        }


        [TestMethod]
        public void ClassInitialize_Runs_Once()
        {
            Assert.AreEqual(1, classInitializeCount);
        }
        [TestMethod]
        public void ClassInitialize_Runs_Once_2()
        {
            Assert.AreEqual(1, classInitializeCount);
        }


        [TestMethod]
        public void TestInitialize_Runs()
        {
            Assert.IsTrue(testInitializeCount > 0, "[TestInitialize] method did not run");
        }


        //
        // Run the same check twice to make sure we've completed at least one full [TestMethod]
        //
        [TestMethod]
        public void TestCleanup_Runs()
        {
            if (testInitializeCount < 2) return;
            Assert.IsTrue(testCleanupCount > 0, "[TestCleanup] did not run");
        }
        [TestMethod]
        public void TestCleanup_Runs_2()
        {
            if (testInitializeCount < 2) return;
            Assert.IsTrue(testCleanupCount > 0, "[TestCleanup] did not run");
        }


        [TestMethod]
        public void AssemblyInitialize_Receives_TestContext()
        {
            Assert.IsTrue(
                assemblyInitializeReceivedTestContext,
                "[AssemblyInitialize] method did not receive a TestContext instance");
        }


        [TestMethod]
        public void AssemblyInitialize_Receives_Random_TestName()
        {
            Assert.IsNotNull(assemblyInitializeTestName);
            Assert.AreNotEqual("", assemblyInitializeTestName);
        }


        [TestMethod]
        public void AssemblyInitialize_Receives_Random_FullyQualifiedTestClassName()
        {
            Assert.IsNotNull(assemblyInitializeFullyQualifiedTestClassName);
            Assert.AreNotEqual("", assemblyInitializeFullyQualifiedTestClassName);
        }


        [TestMethod]
        public void AssemblyInitialize_Receives_InProgress_CurrentTestOutcome()
        {
            Assert.AreEqual(UnitTestOutcome.InProgress, assemblyInitializeCurrentTestOutcome);
        }


        [TestMethod]
        public void ClassInitialize_Receives_TestContext()
        {
            Assert.IsTrue(
                classInitializeReceivedTestContext,
                "[ClassInitialize] method did not receive a TestContext instance");
        }


        [TestMethod]
        public void ClassInitialize_Receives_Random_TestName()
        {
            Assert.IsNotNull(classInitializeTestName);
            Assert.AreNotEqual("", classInitializeTestName);
        }


        [TestMethod]
        public void ClassInitialize_Receives_Correct_FullyQualifiedTestClassName()
        {
            Assert.AreEqual(FullyQualifiedTestClassName, classInitializeFullyQualifiedTestClassName);
        }


        [TestMethod]
        public void ClassInitialize_Receives_InProgress_CurrentTestOutcome()
        {
            Assert.AreEqual(UnitTestOutcome.InProgress, classInitializeCurrentTestOutcome);
        }


        [TestMethod]
        public void TestContext_Available_During_TestMethod()
        {
            Assert.IsNotNull(TestContext, "TestContext not available during [TestMethod]");
        }


        [TestMethod]
        public void TestContext_Available_During_TestInitialize()
        {
            Assert.IsTrue(testInitializeTestContextAvailable);
        }


        [TestMethod]
        public void TestContext_CurrentTestOutcome_InProgress_During_TestInitialize()
        {
            Assert.AreEqual(
                UnitTestOutcome.InProgress,
                testInitializeCurrentTestOutcome);
        }


        [TestMethod]
        public void TestContext_CurrentTestOutcome_InProgress_During_TestMethod()
        {
            Assert.AreEqual(
                UnitTestOutcome.InProgress,
                TestContext.CurrentTestOutcome);
        }


        //
        // Run the same check twice to make sure we've completed at least one full [TestMethod]
        //
        [TestMethod]
        public void TestContext_CurrentTestOutcome_Passed_During_TestCleanup_After_Passed_TestMethod()
        {
            if (testInitializeCount < 2) return;
            Assert.AreEqual(
                UnitTestOutcome.Passed,
                testCleanupCurrentTestOutcome);
        }
        [TestMethod]
        public void TestContext_CurrentTestOutcome_Passed_During_TestCleanup_After_Passed_TestMethod_2()
        {
            if (testInitializeCount < 2) return;
            Assert.AreEqual(
                UnitTestOutcome.Passed,
                testCleanupCurrentTestOutcome);
        }


        [TestMethod]
        public void TestContext_FullyQualifiedTestClassName_Correct_During_TestInitialize()
        {
            Assert.AreEqual(FullyQualifiedTestClassName, testInitializeFullyQualifiedTestClassName);
        }


        [TestMethod]
        public void TestContext_FullyQualifiedTestClassName_Correct_During_TestMethod()
        {
            Assert.AreEqual(FullyQualifiedTestClassName, TestContext?.FullyQualifiedTestClassName);
        }


        [TestMethod]
        public void TestContext_TestName_Correct_During_TestInitialize()
        {
            var thisTestName = MethodBase.GetCurrentMethod().Name;
            Assert.AreEqual(thisTestName, testInitializeTestName);
        }


        [TestMethod]
        public void TestContext_TestName_Correct_During_TestMethod()
        {
            var thisTestName = MethodBase.GetCurrentMethod().Name;
            Assert.AreEqual(thisTestName, TestContext?.TestName);
        }


        [TestMethod]
        public void Print_Trace_Test_Message()
        {
            System.Diagnostics.Trace.WriteLine(TraceTestMessage);
        }


        [Ignore]
        [TestMethod]
        public void IgnoredTestMethod()
        {
            Console.WriteLine(IgnoredTestMessage);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void ExpectedException_Works()
        {
            throw new ArgumentException();
        }
        

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void ExpectedException_AllowDerivedTypes_Works()
        {
            throw new ArgumentNullException();
        }


        [TestMethod]
        public void TestAssembly_Config_File_Is_Used()
        {
            #if NET461
            //
            // Config file switching doesn't work on Mono
            // See https://bugzilla.xamarin.com/show_bug.cgi?id=15741
            //
            if (Type.GetType("Mono.Runtime") != null) return;

            Assert.AreEqual(
                "ConfigFileValue",
                ConfigurationManager.AppSettings["ConfigFileKey"]);
            #endif
        }


        //
        // If each [TestMethod] doesn't get its own [TestClass] instance, the second of these two tests to run will fail
        //
        [TestMethod]
        public void Each_TestMethod_Gets_New_TestClass_Instance_Part1()
        {
            lock (isInstanceNewLock)
            {
                Assert.IsTrue(isInstanceNew, "Not a new [TestClass] instance");
                isInstanceNew = false;
            }
        }
        [TestMethod]
        public void Each_TestMethod_Gets_New_TestClass_Instance_Part2()
        {
            lock (isInstanceNewLock)
            {
                Assert.IsTrue(isInstanceNew, "Not a new [TestClass] instance");
                isInstanceNew = false;
            }
        }


        [TestMethod]
        public void Use_Referenced_Assembly()
        {
            Console.WriteLine(TestReferencedClass.TestReferencedMethod());
        }


        [TestCleanup]
        public void TestCleanup()
        {
            //
            // No way to directly test that [TestCleanup] runs, so print a message so it can be confirmed by examining
            // the output
            //
            Console.WriteLine(TestCleanupMessage);
            testCleanupCurrentTestOutcome = TestContext?.CurrentTestOutcome;
            testCleanupCount++;
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            //
            // No way to directly test that [ClassCleanup] runs, so print a message so it can be confirmed by examining
            // the output
            //
            Console.WriteLine(ClassCleanupMessage);
        }


        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            //
            // No way to directly test that [AssemblyCleanup] runs, so print a message so it can be confirmed by
            // examining the output
            //
            Console.WriteLine(AssemblyCleanupMessage);
        }

    }

}
