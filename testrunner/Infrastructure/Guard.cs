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
            if (paramName == null) throw new ArgumentNullException(nameof(paramName));
            if (value == null) throw new ArgumentNullException(paramName);
        }


        public static void NotNullOrWhiteSpace(string value, string paramName)
        {
            NotNull(value, paramName);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Argument is empty or whitespace-only", paramName);
            }
        }

    }

}
