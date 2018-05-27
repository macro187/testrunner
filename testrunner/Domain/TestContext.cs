#if NETCOREAPP2_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Data;
using System.Data.Common;

namespace TestRunner.Domain
{

    /// <summary>
    /// Property values and method implementations for the
    /// <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> provided to test methods
    /// </summary>
    ///
    public static class TestContext
    {
        
        static TestContext()
        {
            Clear();
        }

        public static UnitTestOutcome CurrentTestOutcome { get; set; }

        public static DbConnection DataConnection { get; set; }

        public static DataRow DataRow { get; set; }

        public static string DeploymentDirectory { get; set; }

        public static string FullyQualifiedTestClassName { get; set; }

        #if NETCOREAPP2_0
        public static IDictionary<string, object> Properties { get; set; }
        #else
        public static IDictionary Properties { get; set; }
        #endif

        public static string ResultsDirectory { get; set; }

        public static string TestDeploymentDir { get; set; }

        public static string TestDir { get; set; }

        public static string TestLogsDir { get; set; }

        public static string TestName { get; set; }

        public static string TestResultsDirectory { get; set; }

        public static string TestRunDirectory { get; set; }

        public static string TestRunResultsDirectory { get; set; }

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

        public static void Clear()
        {
            CurrentTestOutcome = UnitTestOutcome.Unknown;
            DataConnection = null;
            DataRow = null;
            DeploymentDirectory = null;
            FullyQualifiedTestClassName = null;
            Properties = null;
            ResultsDirectory = null;
            TestDeploymentDir = null;
            TestDir = null;
            TestLogsDir = null;
            TestName = null;
            TestResultsDirectory = null;
            TestRunDirectory = null;
            TestRunResultsDirectory = null;
        }

    }
}
