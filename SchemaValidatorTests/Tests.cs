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

        private const string SamplesPath = @"..\..\..\Samples";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public void TestFromDirectory()
        {
            var schemaPath = GetPath(string.Empty);

            var validator = new Validator(schemaPath);

            var xmlPath = GetPath("Example_positive.graphml");

            validator.Validate(xmlPath);
        }

        [Fact]
        public void TestNegative()
        {
            var schemaPath = GetPath("Example.xsd");

            var validator = new Validator(new string[] { schemaPath });

            var xmlPath = GetPath("Example_negative.graphml");

            Assert.Throws<ValidationException>(() => validator.Validate(xmlPath));
        }

        [Fact]
        public void TestPositive()
        {
            var schemaPath = GetPath("Example.xsd");

            var validator = new Validator(new string[] { schemaPath });

            var xmlPath = GetPath("Example_positive.graphml");

            validator.Validate(xmlPath);
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetPath(string path)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var result = Path.Combine(
                assemblyFolder,
                SamplesPath,
                path);

            return result;
        }

        #endregion Private Methods
    }
}