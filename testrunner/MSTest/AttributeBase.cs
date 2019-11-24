using System;
using TestRunner.Infrastructure;

namespace TestRunner.MSTest
{
    public abstract class AttributeBase
    {
        
        protected static TAttribute TryCreate<TAttribute>(
            Attribute attribute,
            string typeName,
            Func<Attribute, TAttribute> constructor)
            where TAttribute : class
        {
            Guard.NotNull(attribute, nameof(attribute));
            Guard.NotNull(typeName, nameof(typeName));
            Guard.NotNull(constructor, nameof(constructor));

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
