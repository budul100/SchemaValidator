using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace SchemaValidator
{
    public class Validator
    {
        #region Private Fields

        private const string EnvelopeFile = @"Schemes\envelope.xsd";

        private readonly XmlReaderSettings settings;

        #endregion Private Fields

        #region Public Constructors

        public Validator(string schemaPath = default)
        {
            settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };

            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var envelopePath = Path.Combine(
                    path1: assemblyFolder,
                    path2: EnvelopeFile);

            settings.Schemas.Add(
                targetNamespace: default,
                schemaUri: envelopePath);

            if (!string.IsNullOrWhiteSpace(schemaPath))
            {
                settings.Schemas.Add(
                    targetNamespace: default,
                    schemaUri: schemaPath);
            }

            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Validate(string xmlPath)
        {
            var reader = XmlReader.Create(
                inputUri: xmlPath,
                settings: settings);

            while (reader.Read()) { }
        }

        #endregion Public Methods

        #region Private Methods

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                throw new Exception($"Validation scheme not found: {args.Message}.");
            }
            else
            {
                throw new Exception($"Validation error: {args.Message}.");
            }
        }

        #endregion Private Methods
    }
}