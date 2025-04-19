# GM Database Project

## Overview
The GM Database project is a .NET-based solution designed to manage and test database functionality. It includes the following components:
- **gmdb**: Implements the core database functionality, allowing most tables to be read and enabling rows to be written into the tables while considering the current index.
- **gbmdb.tests**: Provides a comprehensive test suite for validating the database features.

## Project Structure
```
src/
├── gm.sln                  # Solution file
├── gmdb/                   # Core database project
│   ├── Core/               # Core logic and utilities
│   ├── Doc/                # Project documentation
│   ├── Models/             # Data model definitions
│   ├── Resources/          # Embedded resources
│   └── gmdb.csproj         # Project configuration
└── gbmdb.tests/            # Test project
    ├── GmDbTestsBase.cs    # Base classes for tests
    ├── _GmIndexReaderTests.cs
    ├── Additional test files
    └── gmdb.tests.csproj   # Test project configuration
```

## Getting Started
1. Open the `gm.sln` file in Visual Studio.
2. Restore the required NuGet packages.
3. Build the solution to ensure all dependencies are resolved.

## Running Tests
To execute the tests, use the Test Explorer in Visual Studio. All tests are located in the `gbmdb.tests` project.

## Dependencies
- .NET Framework or .NET Core (refer to the project files for the specific version).

## License

This project is licensed under the **Business Source License 1.1 (BSL 1.1)**. Under this license:

- The source code is available for personal and non-commercial use.
- Commercial use by companies requires a separate commercial license.
- After a specified change date, the code will be made available under the Apache 2.0 License.

For more details, refer to the [LICENSE](LICENSE) file.