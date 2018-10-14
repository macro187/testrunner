using System;

namespace TestRunner.MSTest
{

    class AssemblyInitializeAttribute : AttributeBase
    {

        public static AssemblyInitializeAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyInitializeAttribute",
                a => new AssemblyInitializeAttribute(a));
        }


        AssemblyInitializeAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
