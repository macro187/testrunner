using System;

namespace TestRunner.MSTest
{

    class ClassCleanupAttribute : AttributeBase
    {

        public static ClassCleanupAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute",
                a => new ClassCleanupAttribute(a));
        }


        ClassCleanupAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
