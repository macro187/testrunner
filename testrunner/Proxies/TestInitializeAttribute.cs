using System;

namespace TestRunner.Proxies
{

    class TestInitializeAttribute : AttributeBase
    {

        public static TestInitializeAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute",
                a => new TestInitializeAttribute(a));
        }


        TestInitializeAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
