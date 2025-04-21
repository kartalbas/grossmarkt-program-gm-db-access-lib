# GM Database Project

## Overview
The GM Database project is a .NET-based solution designed to manage and interact with the Grossmarkt database files. The library provides functionality to read and write most tables while supporting index-based operations.

## Project Structure
```
src/
├── gm.sln                  # Solution file
├── gmdb/                   # Core database library
│   ├── Core/               # Core functionality
│   │   ├── Files.cs        # File path management
│   │   ├── GmDb.cs         # Main database operations
│   │   └── ...
│   ├── Models/             # Data models
│   │   ├── GmBase.cs       # Base class for all models
│   │   ├── EkWare.cs       # Purchase items
│   │   ├── VkWammjj.cs     # Sales items (archived by month/year)
│   │   ├── Offen.cs        # Open items
│   │   └── ...
│   ├── Doc/                # Documentation
│   └── Resources/          # Project resources
└── gbmdb.tests/            # Test project
    ├── GmDbTestsBase.cs    # Base class for tests
    ├── _GmIndexReaderTests.cs
    ├── GmDbTestsEkBeleg.cs
    ├── GmDbTestsEkWare.cs
    ├── GmDbTestsFirma.cs
    └── ...
```

## Key Features
- Read and write operations for multiple table types
- Index-based data access and management
- Archive file handling for historical data
- Data import/export capabilities
- File backup and restoration utilities
- Comprehensive test coverage

## Technical Details
- Built with .NET Standard 2.0
- Uses custom binary file formats (.DAT and .NDX files)
- Implements data contracts for serialization
- Supports change tracking and notifications through INotifyPropertyChanged

## Getting Started
1. Open `gm.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution
4. Reference the `gmdb` library in your project

## Usage Examples

### Reading Data from a Table
```csharp
// Initialize the database instance using factory
var gmDb = GmDbFactory.Create(@"D:\GM", "USERDATA");

// Read data from the Waren (items) table
var warenTable = gmDb.Read(TableTypes.WAREN, Files.Waren, string.Empty, null);

// Process the data
foreach (DataRow row in warenTable.Rows)
{
    // Access data from columns
    var artikelNr = Convert.ToInt32(row["c1"]);
    var bezeichnung = row["c2"].ToString();
}
```

### Using Model Classes
```csharp
// Create an instance of a model
var program = new Program(@"D:\GM", "USERDATA");

// Authenticate a user
var user = program.Login("username", "password");

// Get all usernames
var usernames = program.Usernames;
```

## Running Tests
Use the Visual Studio Test Explorer to run the tests in the `gbmdb.tests` project. The tests require a valid GM database path and user data folder.

## Dependencies
- .NET Standard 2.0 or higher

## License
This project is licensed under the Business Source License 1.1 (BSL 1.1).