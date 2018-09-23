using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.MSTest
{

    [Ignore]
    [TestClass]
    public class IgnoredMSTestTests
    {

        [TestMethod]
        public void IgnoredTestClassMethod()
        {
            Console.WriteLine(MSTestTests.IgnoredClassTestMessage);
        }

    }

}
