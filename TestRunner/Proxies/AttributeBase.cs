using System;
using TestRunner.Infrastructure;

namespace TestRunner.Proxies
{
    public abstract class AttributeBase
    {
        
        protected static TAttribute TryCreate<TAttribute>(
            Attribute attribute,
            string typeName,
            Func<Attribute, TAttribute> constructor)
            where TAttribute : class
        {
            Guard.NotNull(attribute, "attribute");
            Guard.NotNull(typeName, "typeName");
            Guard.NotNull(constructor, "constructor");

            if (attribute.GetType().FullName != typeName)
                return null;

            return constructor(attribute);
        }


        protected AttributeBase(Attribute attribute)
        {
            Attribute = attribute;
        }


        public Attribute Attribute
        {
            get;
            private set;
        }

    }
}
