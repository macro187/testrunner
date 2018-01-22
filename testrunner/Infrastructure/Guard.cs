using System;

namespace TestRunner.Infrastructure
{

    public static class Guard
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "param",
            Justification = "Consistency with ArgumentException classes in BCL")]
        public static void NotNull(object value, string paramName)
        {
            if (paramName == null) throw new ArgumentNullException("paramName");
            if (value == null) throw new ArgumentNullException(paramName);
        }

    }

}
