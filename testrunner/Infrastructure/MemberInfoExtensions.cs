using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestRunner.Infrastructure
{

    public static class MemberInfoExtensions
    {

        public static bool HasCustomAttribute<TAttribute>(
            this MemberInfo memberInfo,
            Func<Attribute, TAttribute> tryCreate)
            where TAttribute : class
        {
            return memberInfo.GetCustomAttribute(tryCreate) != null;
        }


        public static TAttribute GetCustomAttribute<TAttribute>(
            this MemberInfo memberInfo,
            Func<Attribute, TAttribute> tryCreate)
            where TAttribute : class
        {
            var atts = memberInfo.GetCustomAttributes(tryCreate).ToList();

            if (atts.Count > 1)
            {
                var memberName = $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";
                if (memberInfo is MethodInfo) memberName += "()";

                var attributeName = typeof(TAttribute).Name;
                   
                throw new UserException(
                    $"{memberName}(): Encountered multiple [{attributeName}] where a maximum of one was expected");
            }

            if (atts.Count == 0)
                return null;

            return atts[0];
        }


        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(
            this MemberInfo memberInfo,
            Func<Attribute, TAttribute> tryCreate)
        {
            Guard.NotNull(memberInfo, nameof(memberInfo));
            Guard.NotNull(tryCreate, nameof(tryCreate));

            return
                memberInfo.GetCustomAttributes(false)
                    .Cast<Attribute>()
                    .Select(a => tryCreate(a))
                    .Where(a => a != null);
        }      

    }

}
