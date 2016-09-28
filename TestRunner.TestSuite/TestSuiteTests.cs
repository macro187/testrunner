using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.TestSuite
{

    [TestClass]
    public class TestSuiteTests
    {

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
        public void TestAssembly_Config_File_Gets_Activated()
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
            Console.WriteLine("[TestCleanup] is running");
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            //
            // No way to directly test that [ClassCleanup] runs, so print a message so it can be confirmed by examining
            // the output
            //
            Console.WriteLine("[ClassCleanup] is running");
        }


        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            //
            // No way to directly test that [AssemblyCleanup] runs, so print a message so it can be confirmed by
            // examining the output
            //
            Console.WriteLine("[AssemblyCleanup] is running");
        }

    }

}
