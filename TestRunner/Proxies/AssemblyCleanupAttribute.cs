using System;

namespace TestRunner.Proxies
{

    class AssemblyCleanupAttribute : AttributeBase
    {

        public static AssemblyCleanupAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyCleanupAttribute",
                a => new AssemblyCleanupAttribute(a));
        }


        AssemblyCleanupAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
