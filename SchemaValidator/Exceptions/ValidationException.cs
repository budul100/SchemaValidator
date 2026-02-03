using System;
using System.Runtime.Serialization;
using System.Xml.Schema;

namespace SchemaValidator.Exceptions
{
    /// <summary>
    /// Represents errors that occur during XML schema validation.
    /// </summary>
    [Serializable]
    public class ValidationException 
        : Exception
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
        { }

        /// <summary>
        /// Initializes a new instance from <see cref="ValidationEventArgs"/> (used by callbacks).
        /// </summary>
        /// <param name="args">The validation event arguments containing error details.</param>
        /// <param name="xmlPath">The path of the XML file that failed validation.</param>
        public ValidationException(ValidationEventArgs args, string xmlPath)
            : base(message: FormatMessage(message: args.Message, severity: args.Severity,
                xmlPath: xmlPath, line: args.Exception?.LineNumber ?? 0,
                position: args.Exception?.LinePosition ?? 0))
        {
            Severity = args.Severity;
            XmlPath = xmlPath;
            LineNumber = args.Exception?.LineNumber ?? 0;
            LinePosition = args.Exception?.LinePosition ?? 0;
        }

        /// <summary>
        /// Initializes a new instance from a direct <see cref="XmlSchemaValidationException"/>.
        /// </summary>
        /// <param name="innerException">The inner exception containing validation error details.</param>
        /// <param name="xmlPath">The path of the XML file that failed validation.</param>
        public ValidationException(XmlSchemaValidationException innerException, string xmlPath)
            : base(message: FormatMessage(message: innerException.Message, severity: XmlSeverityType.Error,
                xmlPath: xmlPath, line: innerException.LineNumber, position: innerException.LinePosition),
                  innerException: innerException)
        {
            XmlPath = xmlPath;
            LineNumber = innerException.LineNumber;
            LinePosition = innerException.LinePosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationException(string message)
            : base(message: message)
        { }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// Constructor for serialization (Obsolete in .NET 8+, but kept for compatibility).
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            XmlPath = info.GetString(nameof(XmlPath));
            Severity = (XmlSeverityType)info.GetInt32(nameof(Severity));
            LineNumber = info.GetInt32(nameof(LineNumber));
            LinePosition = info.GetInt32(nameof(LinePosition));
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the line number where the validation error occurred.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Gets the line position where the validation error occurred.
        /// </summary>
        public int LinePosition { get; }

        /// <summary>
        /// Gets the severity level of the validation event (Error or Warning).
        /// </summary>
        public XmlSeverityType Severity { get; } = XmlSeverityType.Error;

        /// <summary>
        /// Gets the path of the XML file that failed validation.
        /// </summary>
        public string XmlPath { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(XmlPath), XmlPath);
            info.AddValue(nameof(Severity), (int)Severity);
            info.AddValue(nameof(LineNumber), LineNumber);
            info.AddValue(nameof(LinePosition), LinePosition);
        }

        #endregion Public Methods

        #region Private Methods

        private static string FormatMessage(string message, XmlSeverityType severity, string xmlPath, int line,
            int position)
        {
            var prefix = severity == XmlSeverityType.Warning ? "Validation Warning" : "Validation Error";
            var location = line > 0 ? $" at Line {line}, Position {position}" : string.Empty;

            return $"{prefix} in {xmlPath}{location}:\n{message}";
        }

        #endregion Private Methods
    }
}