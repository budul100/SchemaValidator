using System.IO;
using System.Reflection;
using SchemaValidator;
using SchemaValidator.Exceptions;
using Xunit;

namespace SchemaValidatorTests
{
    public class Tests
    {
        #region Private Fields

        private const string GraphMLSamples = @"..\..\..\Samples\GraphML\";
        private const string GraphMLSchema = @"Example.xsd";
        private const string RailMLSamples = @"..\..\..\Samples\railML\";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public void TestFromDirectory()
        {
            var schemaPath = GetPath(RailMLSamples);

            var validator = new Validator(schemaPath);

            var xmlPath = GetPath(
                folder: RailMLSamples,
                file: "_Example.railML");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestFromFile()
        {
            var schemaPath = GetPath(
                folder: GraphMLSamples,
                file: GraphMLSchema);

            var validator = new Validator(schemaPath);

            var xmlPath = GetPath(
                folder: GraphMLSamples,
                file: "Example_negative.graphml");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestNegative()
        {
            var schemaPath = GetPath(
                folder: GraphMLSamples,
                file: "Example.xsd");

            var validator = new Validator([schemaPath]);

            var xmlPath = GetPath(
                folder: GraphMLSamples,
                file: "Example_negative.graphml");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestPositive()
        {
            var schemaPath = GetPath(
                folder: GraphMLSamples,
                file: "Example.xsd");

            var validator = new Validator([schemaPath]);

            var xmlPath = GetPath(
                folder: GraphMLSamples,
                file: "Example_positive.graphml");

            validator.Validate(xmlPath);
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetPath(string folder, string file = "")
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var result = Path.Combine(
                assemblyFolder,
                folder,
                file);

            return result;
        }

        #endregion Private Methods
    }
}