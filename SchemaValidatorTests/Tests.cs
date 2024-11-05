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

        private const string PathSamplesGraphML = @"..\..\..\Samples\GraphML\";
        private const string PathSamplesRailML = @"..\..\..\Samples\railML\";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public void TestFromDirectory()
        {
            var schemaPath = GetPath(PathSamplesRailML);

            var validator = new Validator(schemaPath);

            var xmlPath = GetPath(
                folder: PathSamplesRailML,
                file: "_Example.railML");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestNegative()
        {
            var schemaPath = GetPath(
                folder: PathSamplesGraphML,
                file: "Example.xsd");

            var validator = new Validator([schemaPath]);

            var xmlPath = GetPath(
                folder: PathSamplesGraphML,
                file: "Example_negative.graphml");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestPositive()
        {
            var schemaPath = GetPath(
                folder: PathSamplesGraphML,
                file: "Example.xsd");

            var validator = new Validator([schemaPath]);

            var xmlPath = GetPath(
                folder: PathSamplesGraphML,
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