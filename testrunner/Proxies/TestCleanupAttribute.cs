using System;

namespace TestRunner.Proxies
{

    class TestCleanupAttribute : AttributeBase
    {

        public static TestCleanupAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute",
                a => new TestCleanupAttribute(a));
        }


        TestCleanupAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
