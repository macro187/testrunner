namespace TestRunner.Results
{
    public class MethodResult
    {

        public bool Success { get; set; }
        public ExceptionInfo Exception { get; set; }
        public long ElapsedMilliseconds { get; set; } = -1;

    }
}
