using System;

namespace TestRunner.Domain
{

    class TestMethodAttribute : AttributeBase
    {

        public static TestMethodAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute",
                a => new TestMethodAttribute(a));
        }


        TestMethodAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
