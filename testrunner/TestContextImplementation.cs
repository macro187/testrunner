using System.Collections;
using System.Data;
using System.Data.Common;

namespace TestRunner
{
    static class TestContextImplementation
    {

        // public static Microsoft.VisualStudio.TestTools.UnitTesting.UnitTestOutcome GetCurrentTestOutcome()
        // {
        //     return null;
        // }

        public static DbConnection GetDataConnection()
        {
            return null;
        }

        public static DataRow GetDataRow()
        {
            return null;
        }

        public static string GetDeploymentDirectory()
        {
            return null;
        }

        public static string GetFullyQualifiedTestClassName()
        {
            return null;
        }

        public static IDictionary GetProperties()
        {
            return null;
        }

        public static string GetResultsDirectory()
        {
            return null;
        }

        public static string GetTestDeploymentDir()
        {
            return null;
        }

        public static string GetTestDir()
        {
            return null;
        }

        public static string GetTestLogsDir()
        {
            return null;
        }

        public static string GetTestName()
        {
            return null;
        }

        public static string GetTestResultsDirectory()
        {
            return null;
        }

        public static string GetTestRunDirectory()
        {
            return null;
        }

        public static string GetTestRunResultsDirectory()
        {
            return null;
        }

        public static void AddResultFile(string fileName)
        {
        }

        public static void BeginTimer(string timerName)
        {
        }

        public static void EndTimer(string timerName)
        {
        }

        public static void WriteLine(string message)
        {
        }

        public static void WriteLine(string format, params object[] args)
        {
        }

    }
}
