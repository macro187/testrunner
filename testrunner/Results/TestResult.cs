namespace TestRunner.Results
{
    public class TestResult
    {
        public string TestAssemblyPath { get; set; }
        public string TestClassFullName { get; set; }
        public string TestName { get; set; }
        public bool Success { get; set; }
        public bool Ignored { get; set; }
        public bool IgnoredFromCommandLine { get; set; }
    }
}
