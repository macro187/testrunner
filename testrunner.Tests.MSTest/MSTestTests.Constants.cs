namespace TestRunner.Tests.MSTest
{

    public partial class MSTestTests
    {

        public static readonly string TestCleanupMessage = "[TestCleanup] is running";
        public static readonly string ClassCleanupMessage = "[ClassCleanup] is running";
        public static readonly string AssemblyCleanupMessage = "[AssemblyCleanup] is running";
        public static readonly string IgnoredTestMessage = "[Ignore]d test method is running";
        public static readonly string IgnoredClassInitializeMessage = "[ClassInitialize] on [Ignore]d [TestClass] is running";
        public static readonly string IgnoredClassTestMessage = "[Ignore]d test class method is running";
        public static readonly string IgnoredClassCleanupMessage = "[ClassCleanup] on [Ignore]d [TestClass] is running";
        public static readonly string EmptyClassInitializeMessage = "[ClassInitialize] on empty [TestClass] is running";
        public static readonly string EmptyClassCleanupMessage = "[ClassCleanup] on empty [TestClass] is running";
        public static readonly string TraceTestMessage = "Trace test message";

    }

}
