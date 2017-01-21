using System;

namespace TestRunner.Proxies
{

    class IgnoreAttribute : AttributeBase
    {

        public static IgnoreAttribute TryCreate(Attribute attribute)
        {
            return TryCreate(
                attribute,
                "Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute",
                a => new IgnoreAttribute(a));
        }


        IgnoreAttribute(Attribute attribute)
            : base(attribute)
        {
        }

    }

}
