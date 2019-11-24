using System;

namespace TestRunner.MSTest
{

    class TestClassAttribute : AttributeBase
    {

        public static TestClassAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute",
                a => new TestClassAttribute(a));
        }


        TestClassAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
