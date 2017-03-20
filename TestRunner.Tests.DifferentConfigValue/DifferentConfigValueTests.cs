using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestRunner.Tests.DifferentConfigValue
{

    [TestClass]
    public class DifferentConfigValueTests
    {

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
                "DifferentConfigFileValue",
                ConfigurationManager.AppSettings["ConfigFileKey"]);
        }

    }
}
