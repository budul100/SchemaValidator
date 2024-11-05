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

        private const string SchemaExtension = "*.xsd";

        private readonly XmlReaderSettings settings;

        #endregion Private Fields

        #region Public Constructors

        public Validator(string schemaDirectory)
            : this(GetSchemaPaths(schemaDirectory))
        { }

        public Validator(IEnumerable<string> schemaPaths = default)
        {
            settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                DtdProcessing = DtdProcessing.Ignore,
            };

            var assembly = this.GetType().GetTypeInfo().Assembly;

            using (var schemaStream = assembly.GetManifestResourceStream("SchemaValidator.Schemes.envelope.xsd"))
            {
                var schema = XmlSchema.Read(
                    stream: schemaStream,
                    validationEventHandler: default);

                settings.Schemas.Add(
                    schema: schema);
            }

            var relevantPaths = schemaPaths?
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (relevantPaths?.Any() == true)
            {
                foreach (var relevantPath in relevantPaths)
                {
                    settings.Schemas.Add(
                        targetNamespace: default,
                        schemaUri: relevantPath);
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

        private static IEnumerable<string> GetSchemaPaths(string schemaDirectory)
        {
            var result = default(IEnumerable<string>);

            if (Directory.Exists(schemaDirectory))
            {
                result = Directory.GetFiles(
                    path: schemaDirectory,
                    searchPattern: SchemaExtension);
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