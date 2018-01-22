using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.DifferentConfigValue
{

    [TestClass]
    public class DifferentConfigValueTests
    {

        [TestMethod]
        public void TestAssembly_Config_File_Is_Used()
        {
            //
            // Config file switching doesn't work on Mono
            // See https://bugzilla.xamarin.com/show_bug.cgi?id=15741
            //
            if (Type.GetType("Mono.Runtime") != null) return;
            Assert.AreEqual(
                "DifferentConfigFileValue",
                ConfigurationManager.AppSettings["ConfigFileKey"]);
        }

    }
}
