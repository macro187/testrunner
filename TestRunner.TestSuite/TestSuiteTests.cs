using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.TestSuite
{

    [TestClass]
    public class TestSuiteTests
    {

        public static readonly string TestCleanupMessage = "[TestCleanup] is running";
        public static readonly string ClassCleanupMessage = "[ClassCleanup] is running";
        public static readonly string AssemblyCleanupMessage = "[AssemblyCleanup] is running";
        public static readonly string IgnoredTestMessage = "[Ignore]d test method is running";
        public static readonly string TraceTestMessage = "Trace test message";


        static bool assemblyInitializeRan = false;
        static bool classInitializeRan = false;
        bool testInitializeRan = false;
        bool isInstanceNew  = true;
        object isInstanceNewLock = new object();


        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            assemblyInitializeRan = true;
        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            classInitializeRan = true;
        }


        [TestInitialize]
        public void TestInitialize()
        {
            testInitializeRan = true;
        }


        [TestMethod]
        public void AssemblyInitialize_Runs()
        {
            Assert.IsTrue(assemblyInitializeRan, "[AssemblyInitialize] method did not run");
        }


        [TestMethod]
        public void ClassInitialize_Runs()
        {
            Assert.IsTrue(classInitializeRan, "[ClassInitialize] method did not run");
        }


        [TestMethod]
        public void TestInitialize_Runs()
        {
            Assert.IsTrue(testInitializeRan, "[TestInitialize] method did not run");
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


        #if __MonoCS__
        //
        // Config file switching doesn't work on Mono
        // See https://bugzilla.xamarin.com/show_bug.cgi?id=15741
        //
        [Ignore]
        #endif
        [TestMethod]
        public void TestAssembly_Config_File_Is_Used()
        {
            Assert.AreEqual(
                "ConfigFileValue",
                ConfigurationManager.AppSettings["ConfigFileKey"]);
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


        [TestCleanup]
        public void TestCleanup()
        {
            //
            // No way to directly test that [TestCleanup] runs, so print a message so it can be confirmed by examining
            // the output
            //
            Console.WriteLine(TestCleanupMessage);
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
