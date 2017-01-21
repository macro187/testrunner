using System;

namespace TestRunner.Infrastructure
{

    /// <summary>
    /// A user-facing error
    /// </summary>
    ///
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2237:MarkISerializableTypesWithSerializable",
        Justification = "Doesn't need to be serializable for this application")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "At least a message is required")]
    public class UserException : Exception
    {

        public UserException(string message)
            : this(message, null)
        {
        }


        public UserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }

}
