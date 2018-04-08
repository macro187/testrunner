using System;

namespace TestRunner.Domain
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
