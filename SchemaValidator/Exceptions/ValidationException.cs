using System;
using System.Runtime.Serialization;
using System.Xml.Schema;

namespace SchemaValidator.Exceptions
{
    public class ValidationException
        : Exception
    {
        #region Public Constructors

        public ValidationException()
        { }

        public ValidationException(ValidationEventArgs args, string xmlPath)
            : this(GetMessage(args, xmlPath))
        { }

        public ValidationException(string message)
            : base(message)
        { }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        #endregion Public Constructors

        #region Protected Constructors

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion Protected Constructors

        #region Private Methods

        private static string GetMessage(ValidationEventArgs args, string xmlPath)
        {
            var result = args.Severity == XmlSeverityType.Warning
                ? $"Validation error in {xmlPath}: Validation scheme not found or namespace does not match.\r\n\r\n{args.Message}"
                : $"Validation error in {xmlPath}.\r\n\r\n{args.Message}";

            return result;
        }

        #endregion Private Methods
    }
}