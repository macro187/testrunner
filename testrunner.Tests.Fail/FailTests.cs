using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.Fail
{

    [TestClass]
    public class FailTests
    {

        [TestMethod]
        public void FailTest()
        {
            throw new Exception("Fail");
        }

    }
}
