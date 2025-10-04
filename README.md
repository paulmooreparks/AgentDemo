# Invoice Generator CLI

A cross-platform .NET console application for generating itemized invoices with configurable currency and cultural formatting.

## Project Structure

```
InvoiceGenerator.sln
├── src/
│   ├── InvoiceGenerator.Core/              # Core business logic (netstandard2.1)
│   ├── InvoiceGenerator.Infrastructure/    # Data access & external services (net8.0)
│   └── InvoiceGenerator.Console/           # CLI application (net8.0)
└── tests/
    ├── InvoiceGenerator.Core.Tests/
    ├── InvoiceGenerator.Infrastructure.Tests/
    ├── InvoiceGenerator.Console.Tests/
    └── InvoiceGenerator.Integration.Tests/
```

## Architecture

This solution follows a clean architecture with strict dependency rules:
- **Core**: Pure business logic with no external dependencies
- **Infrastructure**: Data access, configuration, and external service integration
- **Console**: CLI-specific UI and user interaction

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQLite (included with .NET)

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Running the Application
```bash
dotnet run --project src/InvoiceGenerator.Console
```

## Features (Planned)

- ✅ Multi-assembly architecture for reusability
- ✅ SQLite database for invoice storage
- ✅ Cross-platform compatibility (Windows, Linux, macOS)
- ✅ Comprehensive testing setup
- ⏳ Currency and culture-aware formatting
- ⏳ Configurable tax rates and defaults
- ⏳ Console-based invoice creation
- ⏳ File export (Markdown, Plain text)
- ⏳ Invoice search and retrieval

## Configuration

The application stores configuration in `~/.invoice-generator/`:
- `config.json` - Application settings
- `invoices.db` - SQLite database for invoices
- `exports/` - Exported invoice files

## Development

### Project Dependencies
- **Microsoft.Data.Sqlite** - Database operations
- **Dapper** - Lightweight ORM
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **NUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

### Design Principles
- Fail-fast error handling (exceptions bubble up)
- Interface-driven design for testability
- Culture-aware formatting throughout
- No circular dependencies between assemblies

## License

This project is licensed under the MIT License - see the LICENSE file for details.
