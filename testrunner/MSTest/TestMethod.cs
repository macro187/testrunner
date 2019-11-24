using System;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.MSTest
{

    public class TestMethod
    {

        internal static TestMethod TryCreate(TestClass testClass, MethodInfo methodInfo)
        {
            Guard.NotNull(methodInfo, nameof(methodInfo));
            Guard.NotNull(testClass, nameof(testClass));
            return
                methodInfo.HasCustomAttribute(TestMethodAttribute.TryCreate)
                    ? new TestMethod(testClass, methodInfo)
                    : null;
        }


        TestMethod(TestClass testClass, MethodInfo methodInfo)
        {
            TestClass = testClass;
            MethodInfo = methodInfo;
            IsIgnored = MethodInfo.HasCustomAttribute(IgnoreAttribute.TryCreate);
            var eea = methodInfo.GetCustomAttribute(ExpectedExceptionAttribute.TryCreate);
            ExpectedException = eea != null ? eea.ExceptionType : null;
            AllowDerivedExpectedExceptionTypes = eea != null ? eea.AllowDerivedTypes : false;
        }


        public TestClass TestClass
        {
            get;
            private set;
        }


        public MethodInfo MethodInfo
        {
            get;
            private set;
        }


        public bool IsIgnored
        {
            get;
            private set;
        }


        public Type ExpectedException
        {
            get;
            private set;
        }


        public bool AllowDerivedExpectedExceptionTypes
        {
            get;
            private set;
        }


        public string Name
        {
            get
            {
                return MethodInfo.Name;
            }
        }


        public string FullName
        {
            get
            {
                return $"{TestClass.FullName}.{Name}";
            }
        }

    }

}
