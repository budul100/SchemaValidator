using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using SchemaValidator.Exceptions;

namespace SchemaValidator
{
    public class Validator
    {
        #region Private Fields

        private const string EnvelopeSchema = "SchemaValidator.Schemes.envelope.xsd";
        private const string SchemaExtension = "*.xsd";

        private readonly XmlReaderSettings settings;

        #endregion Private Fields

        #region Public Constructors

        public Validator(string schemaPath)
            : this(GetSchemaFiles(schemaPath))
        { }

        public Validator(IEnumerable<string> schemaFiles = default)
        {
            settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                DtdProcessing = DtdProcessing.Ignore,
            };

            var assembly = this.GetType().GetTypeInfo().Assembly;

            using (var schemaStream = assembly.GetManifestResourceStream(EnvelopeSchema))
            {
                if (schemaStream == null)
                {
                    throw new FileNotFoundException(
                        $"The embedded resource '{EnvelopeSchema}' was not found. " +
                        "Ensure the file is marked as 'Embedded Resource' in the project.");
                }

                var schema = XmlSchema.Read(
                    stream: schemaStream,
                    validationEventHandler: default);

                settings.Schemas.Add(
                    schema: schema);
            }

            var relevantFiles = schemaFiles?
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (relevantFiles?.Any() == true)
            {
                foreach (var relevantFile in relevantFiles)
                {
                    settings.Schemas.Add(
                        targetNamespace: default,
                        schemaUri: relevantFile);
                }
            }

            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Validate(string xmlPath)
        {
            settings.ValidationEventHandler += new ValidationEventHandler((s, a) => ValidationCallBack(a, xmlPath));

            var reader = XmlReader.Create(
                inputUri: xmlPath,
                settings: settings);

            while (reader.Read()) { }
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<string> GetSchemaFiles(string schemaPath)
        {
            var result = default(IEnumerable<string>);

            if (!string.IsNullOrWhiteSpace(schemaPath))
            {
                if (Directory.Exists(schemaPath))
                {
                    result = Directory.GetFiles(
                        path: schemaPath,
                        searchPattern: SchemaExtension);
                }
                else if (File.Exists(schemaPath))
                {
                    result = new[] { schemaPath };
                }

                if (result == default)
                {
                    throw new DirectoryNotFoundException(
                        message: $"There are no schema files at '{schemaPath}'.");
                }
            }

            return result;
        }

        private static void ValidationCallBack(ValidationEventArgs args, string xmlPath)
        {
            throw new ValidationException(
                args: args,
                xmlPath: xmlPath);
        }

        #endregion Private Methods
    }
}