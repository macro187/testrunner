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
                throw new UserException(
                    StringExtensions.FormatInvariant(
                        "{0}(): Encountered more than one [{1}] where only a maximum of one was expected",
                        memberInfo is MethodInfo
                            ? memberInfo.DeclaringType.FullName + "." + memberInfo.Name + "()"
                            : memberInfo.Name,
                        typeof(TAttribute).Name));

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
