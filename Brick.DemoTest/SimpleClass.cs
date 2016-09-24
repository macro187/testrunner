using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Brick.DemoTest
{
    /// <summary>
    /// Simple class to create a unit test for.
    /// Is used to test the MSTestRunner with.
    /// </summary>
    public class SimpleClass
    {
        /// <summary>
        /// Returns static text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return "MSTestRunner";
        }

        /// <summary>
        /// Returns text from app.config.
        /// </summary>
        /// <returns></returns>
        public string GetTextFromAppConfig()
        {
            return ConfigurationManager.AppSettings["SomeSetting"];
        }
    }
}
