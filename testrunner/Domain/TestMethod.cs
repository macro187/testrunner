using System;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.Domain
{

    public class TestMethod
    {

        internal static TestMethod TryCreate(MethodInfo methodInfo)
        {
            Guard.NotNull(methodInfo, nameof(methodInfo));
            return methodInfo.HasCustomAttribute(TestMethodAttribute.TryCreate) ? new TestMethod(methodInfo) : null;
        }


        TestMethod(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            IsIgnored = MethodInfo.HasCustomAttribute(IgnoreAttribute.TryCreate);
            var eea = methodInfo.GetCustomAttribute(ExpectedExceptionAttribute.TryCreate);
            ExpectedException = eea != null ? eea.ExceptionType : null;
            AllowDerivedExpectedExceptionTypes = eea != null ? eea.AllowDerivedTypes : false;
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

    }

}
