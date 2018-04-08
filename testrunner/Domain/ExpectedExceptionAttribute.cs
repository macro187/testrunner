using System;

namespace TestRunner.Domain
{

    class ExpectedExceptionAttribute : AttributeBase
    {

        public static ExpectedExceptionAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedExceptionAttribute",
                a => new ExpectedExceptionAttribute(a));
        }


        ExpectedExceptionAttribute(Attribute attribute)
            : base(attribute)
        {
            ExceptionType = (Type)attribute.GetType().GetProperty("ExceptionType").GetValue(attribute, null);
            AllowDerivedTypes = (bool)attribute.GetType().GetProperty("AllowDerivedTypes").GetValue(attribute, null);
        }


        public Type ExceptionType
        {
            get;
            private set;
        }


        public bool AllowDerivedTypes
        {
            get;
            private set;
        }

    }

}
