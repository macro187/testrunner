using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.Fail
{

    [TestClass]
    public class FailTests
    {
        
        public TestContext TestContext { get; set; }


        [TestCleanup]
        public void TestCleanup()
        {
            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
            {
                Console.Out.WriteLine("Failed UnitTestOutcome");
            }
        }


        [TestMethod]
        public void FailTest()
        {
            throw new Exception("Fail");
        }

    }
}
