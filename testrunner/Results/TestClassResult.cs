namespace TestRunner.Results
{
    public class TestClassResult
    {
        public string TestAssemblyPath { get; set; }
        public string TestClassFullName { get; set; }
        public bool Success { get; set; }
        public bool ClassIgnored { get; set; }
        public bool InitializePresent { get; set; }
        public bool InitializeSucceeded { get; set; }
        public int TestsTotal { get; set; }
        public int TestsRan { get; set; }
        public int TestsIgnored { get; set; }
        public int TestsPassed { get; set; }
        public int TestsFailed { get; set; }
        public bool CleanupPresent { get; set; }
        public bool CleanupSucceeded { get; set; }
    }
}
