# SchemaValidator

A .NET library for validating XML files against XSD schemas.

## Description

SchemaValidator is a simple and efficient library for validating XML documents against XSD schema definitions. The library supports both individual schema files and complete directories containing multiple XSD files.

## Features

- Validation of XML files against XSD schemas

- Support for single schema files

- Support for schema directories with multiple XSD files

- Built-in envelope schema for extended validations

- Detailed error output for validation failures

- .NET Standard 2.0 compatibility

---

## Installation

**NuGet CLI**

```bash
dotnet add package budul.SchemaValidator
```

**Package Manager Console**

```powershell
Install-Package budul.SchemaValidator
```

---

## Usage

### Validation with a Single Schema File

```cs
using SchemaValidator;
using SchemaValidator.Exceptions;

// Create validator with path to schema file
var validator = new Validator("path/to/schema.xsd");

try 
{
    validator.Validate("path/to/file.xml");
    Console.WriteLine("Validation successful");
} 
catch (ValidationException ex) 
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
```

### Validation with a Schema Directory

```cs
using SchemaValidator;

// All .xsd files in the directory will be loaded
var validator = new Validator("path/to/schema/directory");
validator.Validate("path/to/file.xml");
```

### Validation with Multiple Explicit Schema Files

```cs
var schemaFiles = new[] { "schema1.xsd", "schema2.xsd", "schema3.xsd" };
var validator = new Validator(schemaFiles);
validator.Validate("path/to/file.xml");
```

### Validation with Built-in Envelope Only

```cs
var validator = new Validator();
validator.Validate("path/to/file.xml");
```

---

## Exception Handling

The library throws a `ValidationException` when the XML file does not conform to the schema. This exception contains detailed information about the validation error and the file path of the XML file being validated.

## Requirements

- .NET Standard 2.0 or higher

- .NET Framework 4.6.1 or higher

- .NET Core 2.0 or higher

- .NET 5.0 or higher

---

## Project Info

- **Author:** budul

- **Version:** 1.3.1

- **License:** MIT

- **Repository:** [GitHub - budul100/SchemaValidator](https://github.com/budul100/SchemaValidator)

## Tags

XSD, Schema validation, XML, Validation, .NET Standard
