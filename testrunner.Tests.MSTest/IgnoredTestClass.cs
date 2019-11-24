using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.MSTest
{

    [Ignore]
    [TestClass]
    public class IgnoredTestClass
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Console.WriteLine(MSTestTests.IgnoredClassInitializeMessage);
        }


        [TestMethod]
        public void IgnoredTestClassMethod()
        {
            Console.WriteLine(MSTestTests.IgnoredClassTestMessage);
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(MSTestTests.IgnoredClassCleanupMessage);
        }

    }

}
