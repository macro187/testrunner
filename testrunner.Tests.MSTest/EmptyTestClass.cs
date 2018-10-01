using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.MSTest
{

    [TestClass]
    public class EmptyTestClass
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Console.WriteLine(MSTestTests.EmptyClassInitializeMessage);
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(MSTestTests.EmptyClassCleanupMessage);
        }

    }

}
