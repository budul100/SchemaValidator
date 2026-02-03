using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using SchemaValidator.Exceptions;

namespace SchemaValidator
{
    /// <summary>
    /// Provides thread-safe XML validation against XSD schemas.
    /// </summary>
    public class Validator
    {
        #region Private Fields

        private const string EnvelopeSchema = "SchemaValidator.Schemes.envelope.xsd";
        private const string SchemaExtension = "*.xsd";

        /// <summary>
        /// A thread-safe template containing the schema set and base configuration.
        /// </summary>
        private readonly XmlReaderSettings _templateSettings;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class using schemas from a directory or a single file.
        /// </summary>
        /// <param name="schemaPath">The path to a directory containing XSD files or a path to a specific XSD file.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if no schema files are found at the provided path.</exception>
        public Validator(string schemaPath)
            : this(GetSchemaFiles(schemaPath))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class using a collection of schema file paths.
        /// </summary>
        /// <param name="schemaFiles">An enumerable collection of paths to XSD files.</param>
        /// <exception cref="FileNotFoundException">Thrown if the internal envelope schema resource is missing.</exception>
        public Validator(IEnumerable<string> schemaFiles = default)
        {
            _templateSettings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                DtdProcessing = DtdProcessing.Ignore,
            };

            var assembly = this.GetType().GetTypeInfo().Assembly;

            // Load the internal base schema
            using (var schemaStream = assembly.GetManifestResourceStream(EnvelopeSchema))
            {
                if (schemaStream == null)
                {
                    throw new FileNotFoundException(
                        $"The embedded resource '{EnvelopeSchema}' was not found. " +
                        "Ensure the file is marked as 'Embedded Resource' in the project.");
                }

                var schema = XmlSchema.Read(schemaStream, null);
                _templateSettings.Schemas.Add(schema);
            }

            // Load external schemas provided by the user
            var relevantFiles = schemaFiles?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (relevantFiles?.Length > 0)
            {
                foreach (var relevantFile in relevantFiles)
                {
                    _templateSettings.Schemas.Add(null, relevantFile);
                }
            }

            _templateSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            _templateSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            _templateSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            // Compiling the SchemaSet makes it read-only and thread-safe for the Validate method.
            _templateSettings.Schemas.Compile();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Validates an XML file against the configured schemas.
        /// </summary>
        /// <param name="xmlPath">The full path to the XML file to be validated.</param>
        /// <exception cref="ValidationException">Thrown when a schema validation error or warning occurs.</exception>
        /// <remarks>
        /// This method is thread-safe. It clones the internal settings to ensure that
        /// concurrent validation calls do not interfere with each other's event handlers.
        /// </remarks>
        public void Validate(string xmlPath)
        {
            // Clone settings to isolate the ValidationEventHandler for this specific thread/call.
            var localSettings = _templateSettings.Clone();

            localSettings.ValidationEventHandler += (s, a) => ValidationCallBack(a, xmlPath);

            try
            {
                using (var reader = XmlReader.Create(xmlPath, localSettings))
                {
                    while (reader.Read())
                    {
                        // Validation happens here
                    }
                }
            }
            catch (XmlSchemaValidationException ex)
            {
                // Catch direct validation exceptions and wrap them in your custom exception
                throw new ValidationException(
                    innerException: ex,
                    xmlPath: xmlPath);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Resolves a path into a list of XSD file paths.
        /// </summary>
        /// <param name="schemaPath">The directory or file path to check.</param>
        /// <returns>A collection of file paths ending in .xsd.</returns>
        private static IEnumerable<string> GetSchemaFiles(string schemaPath)
        {
            if (string.IsNullOrWhiteSpace(schemaPath))
                return Enumerable.Empty<string>();

            if (Directory.Exists(schemaPath))
            {
                return Directory.GetFiles(schemaPath, SchemaExtension);
            }

            if (File.Exists(schemaPath))
            {
                return new[] { schemaPath };
            }

            throw new DirectoryNotFoundException($"There are no schema files at '{schemaPath}'.");
        }

        /// <summary>
        /// Callback method that handles validation events encountered by the <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="args">The event arguments containing error details.</param>
        /// <param name="xmlPath">The path of the XML file being validated (for logging purposes).</param>
        /// <exception cref="ValidationException">Always thrown when this method is triggered.</exception>
        private static void ValidationCallBack(ValidationEventArgs args, string xmlPath)
        {
            throw new ValidationException(args: args, xmlPath: xmlPath);
        }

        #endregion Private Methods
    }
}