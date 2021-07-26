using SchemaValidator.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace SchemaValidator
{
    public class Validator
    {
        #region Private Fields

        private readonly XmlReaderSettings settings;

        #endregion Private Fields

        #region Public Constructors

        public Validator(string schemaPath = default)
        {
            settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
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

            if (!string.IsNullOrWhiteSpace(schemaPath))
            {
                settings.Schemas.Add(
                    targetNamespace: default,
                    schemaUri: schemaPath);
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

        private static void ValidationCallBack(ValidationEventArgs args, string xmlPath)
        {
            throw new ValidationException(
                args: args,
                xmlPath: xmlPath);
        }

        #endregion Private Methods
    }
}