# Invoice Generator CLI — Design Document

## Overview
A cross-platform .NET console application for generating itemized invoices with configurable currency and cultural formatting.

## Architecture

### Solution Structure (Multi-Assembly Design)
```
InvoiceGenerator.sln
├── Core/
│   ├── InvoiceGenerator.Core/              # Core business logic
│   │   ├── Models/
│   │   │   ├── Invoice.cs
│   │   │   ├── InvoiceItem.cs
│   │   │   ├── Client.cs
│   │   │   └── Enums/
│   │   ├── Interfaces/
│   │   │   ├── IInvoiceCalculationService.cs
│   │   │   ├── ICurrencyFormattingService.cs
│   │   │   ├── IInputValidationService.cs
│   │   │   └── IDateTimeProvider.cs
│   │   ├── Services/
│   │   │   ├── InvoiceCalculationService.cs
│   │   │   ├── CurrencyFormattingService.cs
│   │   │   └── InputValidationService.cs
│   │   ├── Extensions/
│   │   │   └── CultureExtensions.cs
│   │   └── Exceptions/
│   │       ├── InvoiceValidationException.cs
│   │       └── CurrencyFormatException.cs
│   │
│   └── InvoiceGenerator.Infrastructure/     # Data access & external concerns
│       ├── Configuration/
│       │   ├── AppConfiguration.cs
│       │   ├── IConfigurationService.cs
│       │   └── ConfigurationService.cs
│       ├── Data/
│       │   ├── Interfaces/
│       │   │   ├── IInvoiceRepository.cs
│       │   │   ├── IClientRepository.cs
│       │   │   └── IDatabaseService.cs
│       │   ├── Repositories/
│       │   │   ├── InvoiceRepository.cs
│       │   │   └── ClientRepository.cs
│       │   ├── Services/
│       │   │   ├── DatabaseService.cs
│       │   │   ├── InvoiceStorageService.cs
│       │   │   └── InvoiceRetrievalService.cs
│       │   └── Migrations/
│       │       └── InitialSchema.sql
│       └── FileSystem/
│           ├── IFileSystemService.cs
│           └── FileSystemService.cs
│
├── Applications/
│   ├── InvoiceGenerator.Console/           # CLI application
│   │   ├── Handlers/
│   │   │   ├── ConsoleInputHandler.cs
│   │   │   ├── FileOutputHandler.cs
│   │   │   └── MarkdownOutputHandler.cs
│   │   ├── Services/
│   │   │   ├── IConsoleWrapper.cs
│   │   │   └── ConsoleWrapper.cs
│   │   ├── Commands/
│   │   │   ├── CreateInvoiceCommand.cs
│   │   │   ├── ListInvoicesCommand.cs
│   │   │   └── SearchInvoicesCommand.cs
│   │   ├── DependencyInjection/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Program.cs
│   │
│   ├── InvoiceGenerator.Api/               # Future: Web API
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   └── Program.cs
│   │
│   └── InvoiceGenerator.Desktop/           # Future: WPF/MAUI app
│       ├── ViewModels/
│       ├── Views/
│       └── Services/
│
└── Tests/
    ├── InvoiceGenerator.Core.Tests/
    ├── InvoiceGenerator.Infrastructure.Tests/
    ├── InvoiceGenerator.Console.Tests/
    └── InvoiceGenerator.Integration.Tests/
```

### Assembly Separation Principles

#### InvoiceGenerator.Core
**Purpose:** Pure business logic with no external dependencies
**Contains:**
- Domain models (Invoice, InvoiceItem, Client)
- Business logic services (calculations, validation, formatting)
- Domain interfaces
- Business exceptions

**Dependencies:** Only .NET Standard libraries, no external packages
**Benefits:**
- Can be reused in any .NET application
- Fast to test (no I/O operations)
- Clear business rules and logic

#### InvoiceGenerator.Infrastructure
**Purpose:** Data access, configuration, and external system integration
**Contains:**
- Database repositories and services
- Configuration management
- File system operations
- External service integrations

**Dependencies:**
- References `InvoiceGenerator.Core`
- Database packages (Microsoft.Data.Sqlite, Dapper)
- Configuration packages

**Benefits:**
- Implements interfaces defined in Core
- Can be swapped for different storage implementations
- Handles all external concerns

#### InvoiceGenerator.Console
**Purpose:** Console-specific UI and user interaction
**Contains:**
- Console input/output handlers
- CLI command structure
- Program entry point
- Dependency injection setup

**Dependencies:**
- References `InvoiceGenerator.Core`
- References `InvoiceGenerator.Infrastructure`
- Console-specific packages

**Benefits:**
- Completely replaceable with different UI
- Focused on user interaction only
- Clean separation from business logic

### Dependency Flow
```
┌─────────────────────┐
│ InvoiceGenerator    │
│     .Console        │ ─┐
└─────────────────────┘  │
                         ▼
┌─────────────────────┐  ┌─────────────────────┐
│ InvoiceGenerator    │  │ InvoiceGenerator    │
│  .Infrastructure    │◄─┤      .Core          │
└─────────────────────┘  └─────────────────────┘
```

**Rules:**
- Core has no dependencies on other assemblies
- Infrastructure depends only on Core
- Applications depend on both Core and Infrastructure
- No circular dependencies allowed

### Benefits of Multi-Assembly Architecture

#### Reusability Across Different UIs
The separated assemblies enable building multiple applications:

**Web API Application:**
```csharp
// InvoiceGenerator.Api can reuse the same Core and Infrastructure
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceCalculationService _calculationService;
    private readonly IInvoiceRepository _repository;

    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        // Same business logic, different UI
        var invoice = new Invoice { /* ... */ };
        _calculationService.CalculateSubtotal(invoice.Items);
        await _repository.SaveAsync(invoice);
        return Ok(invoice);
    }
}
```

**Desktop Application (WPF/MAUI):**
```csharp
// InvoiceGenerator.Desktop can reuse the same services
public class InvoiceViewModel : INotifyPropertyChanged
{
    private readonly IInvoiceCalculationService _calculationService;
    private readonly IInvoiceRepository _repository;

    public async Task SaveInvoiceAsync()
    {
        // Same business logic, different UI
        _calculationService.CalculateTotal(Invoice.Subtotal, Invoice.TaxAmount);
        await _repository.SaveAsync(Invoice);
    }
}
```

**Mobile Application:**
```csharp
// InvoiceGenerator.Mobile (MAUI) - same services, mobile UI
public class InvoiceService
{
    private readonly IInvoiceCalculationService _calculationService;
    // Identical business logic, optimized mobile interface
}
```

#### Independent Testing and Development
- **Core**: Can be tested without any UI or database
- **Infrastructure**: Can be tested with in-memory databases
- **Applications**: Can be tested with mocked services

#### Package Distribution
- **NuGet Packages**: Core and Infrastructure can be distributed as NuGet packages
- **Versioning**: Each assembly can have independent versioning
- **Third-Party Integration**: Other developers can use just Core for their own UIs

#### Enterprise Scalability
- **Microservices**: Core can be wrapped in microservice APIs
- **Cloud Functions**: Individual services can become Azure Functions or AWS Lambdas
- **Plugin Architecture**: New features can be added without changing existing assemblies

## Core Components

### Models (InvoiceGenerator.Core)

#### `AppConfiguration` (moved to Infrastructure)
*Note: Configuration is infrastructure concern, moved to Infrastructure assembly*

#### `Invoice` (Core Domain Model)
```csharp
public class Invoice
{
    public string InvoiceId { get; set; } // e.g., "INV-2025-001"
    public Client Client { get; set; }
    public List<InvoiceItem> Items { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; }
    public string Culture { get; set; }
}
```

#### `InvoiceItem`
```csharp
public class InvoiceItem
{
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}
```

#### `Client`
```csharp
public class Client
{
    public string Name { get; set; }
    // Future: Address, contact info, etc.
}
```

### Core Services (InvoiceGenerator.Core)

#### `ICurrencyFormattingService` / `CurrencyFormattingService`
- **Assembly**: Core (pure business logic)
- **Responsibility**: Handle culture-specific currency formatting
- **Key Methods**:
  - `FormatAmount(decimal amount, string currency, string culture)`: Format monetary values
  - `GetCurrencySymbol(string currency)`: Get currency symbol
  - `GetDecimalPlaces(string currency)`: Get appropriate decimal places
- **Implementation**: Uses `CultureInfo` and `RegionInfo` for proper formatting

#### `IInvoiceCalculationService` / `InvoiceCalculationService`
- **Assembly**: Core (pure calculations)
- **Responsibility**: Calculate invoice totals
- **Key Methods**:
  - `CalculateSubtotal(List<InvoiceItem> items)`: Sum all line totals
  - `CalculateTax(decimal subtotal, decimal taxRate)`: Calculate tax amount
  - `CalculateTotal(decimal subtotal, decimal taxAmount)`: Calculate final total

#### `IInputValidationService` / `InputValidationService`
- **Assembly**: Core (business validation rules)
- **Responsibility**: Validate business data
- **Key Methods**:
  - `ValidateClientName(string name)`: Ensure non-empty client name
  - `ValidateItemDescription(string description)`: Ensure valid description
  - `ValidateQuantity(int quantity)`: Validate quantity rules
  - `ValidateUnitPrice(decimal price)`: Validate price rules
  - `ValidateTaxRate(decimal rate)`: Validate tax rate rules

### Infrastructure Services (InvoiceGenerator.Infrastructure)

#### `IConfigurationService` / `ConfigurationService`
- **Assembly**: Infrastructure (file system access)
- **Responsibility**: Manage application configuration
- **Key Methods**:
  - `LoadConfiguration()`: Load from `~/.invoice-generator/config.json`
  - `SaveConfiguration()`: Save configuration changes
  - `CreateDefaultConfiguration()`: Initialize on first run
  - `ValidateConfiguration()`: Ensure valid culture/currency codes

#### `IInvoiceRepository` / `InvoiceRepository`
- **Assembly**: Infrastructure (data access)
- **Responsibility**: Database operations for invoices
- **Key Methods**:
  - `SaveAsync(Invoice invoice)`: Save invoice to database
  - `GetByIdAsync(string invoiceId)`: Retrieve specific invoice
  - `SearchAsync(InvoiceSearchCriteria criteria)`: Search invoices
  - `GetRecentAsync(int count)`: Get most recent invoices

#### `IDatabaseService` / `DatabaseService`
- **Assembly**: Infrastructure (database management)
- **Responsibility**: Database initialization and management
- **Key Methods**:
  - `InitializeDatabaseAsync()`: Create database and tables if not exist
  - `GenerateInvoiceIdAsync()`: Generate unique invoice ID
  - `MigrateSchemaAsync()`: Handle database migrations

### Application Services (InvoiceGenerator.Console)

#### `IConsoleWrapper` / `ConsoleWrapper`
- **Assembly**: Console (UI abstraction)
- **Responsibility**: Console I/O abstraction for testing
- **Key Methods**:
  - `ReadLine()`: Read user input
  - `WriteLine(string message)`: Display message
  - `Clear()`: Clear console
  - `SearchInvoices(string clientName, DateTime? fromDate, DateTime? toDate)`: Search invoices
  - `GetInvoiceById(string invoiceId)`: Retrieve specific invoice with items
  - `GetRecentInvoices(int count)`: Get most recent invoices
  - `GetInvoicesByClient(string clientName)`: Get all invoices for a client
  - `GetInvoiceStatistics()`: Get summary statistics

### Input/Output Handlers

#### `ConsoleInputHandler`
- **Responsibility**: Handle all console input interactions
- **Key Methods**:
  - `GetClientName()`: Prompt for and collect client name
  - `GetInvoiceItems()`: Collect items in a loop until user is done
  - `GetTaxRate(decimal defaultRate)`: Get tax rate with default option
  - `ConfirmSaveToFile()`: Ask if user wants to save output

#### `FileOutputHandler`
- **Responsibility**: Handle file output operations
- **Key Methods**:
  - `SaveToFile(string content, string filename, string format)`: Save formatted invoice
  - `GetOutputPath()`: Determine where to save file
  - `EnsureDirectoryExists(string path)`: Create output directory if needed

#### `MarkdownOutputHandler`
- **Responsibility**: Format invoice as Markdown
- **Key Methods**:
  - `FormatInvoice(Invoice invoice, AppConfiguration config)`: Generate Markdown output

## Application Flow

### Startup Sequence
1. Load configuration from `~/.invoice-generator/config.json`
2. If config doesn't exist, create default configuration
3. Validate configuration (culture codes, currency codes)
4. Set up culture for current session

### Main Workflow
1. **Collect Client Information**
   - Prompt for client name
   - Validate input

2. **Collect Invoice Items**
   - Loop: Get description, quantity, unit price
   - Validate each input
   - Add to items list
   - Ask if user wants to add more items

3. **Collect Tax Information**
   - Display default tax rate from config
   - Allow user to override or accept default
   - Validate tax rate input

4. **Calculate Totals**
   - Calculate subtotal from all items
   - Calculate tax amount
   - Calculate final total

5. **Display Invoice**
   - Format invoice using culture-specific formatting
   - Display in console with proper currency symbols

6. **Optional File Output**
   - Ask if user wants to save to file
   - If yes, prompt for format (Markdown/plain text)
   - Save formatted invoice

## Invoice Storage Strategy

### Storage Location
```
~/.invoice-generator/
├── config.json
├── invoices.db              # SQLite database
└── exports/                 # Exported files (MD, PDF, etc.)
    ├── 2025/
    │   └── 10/
    │       ├── INV-2025-001.md
    │       └── INV-2025-001.pdf
```

### SQLite Database Design

**Primary Storage:** SQLite database for all invoice data and metadata
**Export Files:** Optional formatted outputs (Markdown, PDF) stored separately

**Libraries:**
- `Microsoft.Data.Sqlite` - Official SQLite provider
- `Dapper` - Lightweight ORM for clean SQL queries
- `System.Text.Json` - For configuration and complex data serialization

**Database Schema:**
```sql
-- Main invoices table
CREATE TABLE Invoices (
    InvoiceId TEXT PRIMARY KEY,
    ClientName TEXT NOT NULL,
    InvoiceDate DATE NOT NULL,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    TaxRate DECIMAL(5,2) NOT NULL,
    TaxAmount DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    Currency TEXT NOT NULL,
    Culture TEXT NOT NULL,
    Notes TEXT
);

-- Invoice line items
CREATE TABLE InvoiceItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    InvoiceId TEXT NOT NULL,
    Description TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(10,2) NOT NULL,
    SortOrder INTEGER NOT NULL,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId) ON DELETE CASCADE
);

-- Client information for future use
CREATE TABLE Clients (
    ClientName TEXT PRIMARY KEY,
    ContactInfo TEXT, -- JSON for extensibility
    CreatedDate DATETIME NOT NULL,
    LastUsedDate DATETIME NOT NULL
);

-- Indexes for performance
CREATE INDEX IX_Invoices_ClientName ON Invoices(ClientName);
CREATE INDEX IX_Invoices_InvoiceDate ON Invoices(InvoiceDate);
CREATE INDEX IX_Invoices_CreatedDate ON Invoices(CreatedDate);
CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);
```

### Invoice ID Generation
Format: `INV-YYYY-NNN`
- YYYY: Year
- NNN: Sequential number (001, 002, etc.)
- Example: `INV-2025-001`
- Generated by querying the database for the highest number in the current year

### Migration Path to Enterprise Database
The SQLite design makes it easy to migrate to enterprise databases:

**SQL Server Migration:**
- Change connection string and provider
- Adjust data types (DECIMAL precision, DATETIME2)
- Add enterprise features (audit trails, security)

**PostgreSQL Migration:**
- Similar schema with minor syntax adjustments
- Enhanced JSON support for complex data
- Better concurrent access

**Sample Migration Strategy:**
1. Export data from SQLite using standard SQL
2. Adjust schema for target database
3. Import data using database-specific tools
4. Update connection configuration

### Database Service Implementation
```csharp
public class DatabaseService
{
    private readonly string _connectionString;

    public async Task InitializeDatabaseAsync()
    {
        // Create database file if not exists
        // Run schema creation scripts
        // Create indexes
    }

    public async Task<string> GenerateInvoiceIdAsync()
    {
        // Query for max invoice number in current year
        // Increment and format as INV-YYYY-NNN
    }
}
```

### Retrieval Commands (Future CLI Enhancement)
```bash
# List recent invoices
invoice-gen list --recent 10

# Search by client
invoice-gen search --client "Acme Corp"

# Search by date range
invoice-gen search --from 2025-01-01 --to 2025-12-31

# Get specific invoice
invoice-gen get INV-2025-001

# Export invoice to PDF
invoice-gen export INV-2025-001 --format pdf
```

## Configuration Management

### Configuration File Location
- **Windows**: `%USERPROFILE%\.invoice-generator\config.json`
- **Linux/macOS**: `~/.invoice-generator/config.json`

### Default Configuration
```json
{
  "culture": "en-SG",
  "currency": "SGD",
  "defaultTaxRate": 7.0,
  "dateFormat": "dd/MM/yyyy",
  "defaultOutputFormat": "console"
}
```

### Supported Currencies (Initial)
- SGD (Singapore Dollar) - 2 decimal places
- USD (US Dollar) - 2 decimal places
- JPY (Japanese Yen) - 0 decimal places
- EUR (Euro) - 2 decimal places

## Cross-Platform Considerations

### Path Handling
- Use `Path.Combine()` for all path operations
- Use `Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)` for home directory

### Culture Handling
- Set `CultureInfo.CurrentCulture` based on configuration
- Use culture-specific number and currency formatting
- Handle right-to-left languages (future consideration)

### File System
- Handle different line endings appropriately
- Use UTF-8 encoding for all file operations

## Error Handling Strategy

### Input Validation
- Throw `ArgumentException` for invalid input with descriptive messages
- Don't catch exceptions - let them bubble up for immediate feedback

### File Operations
- Allow `IOException`, `UnauthorizedAccessException` to bubble up
- Don't swallow exceptions - fail fast for easier debugging

### Configuration Errors
- Throw `ConfigurationException` for invalid config values
- Validate culture and currency codes on startup

## Extensibility Points

### Future Enhancements
- **Discount Codes**: Add discount calculation service
- **Invoice Numbering**: Add invoice ID generation
- **Templates**: Support custom invoice templates
- **Multiple Clients**: Store client information
- **PDF Output**: Add PDF generation capability
- **Email Integration**: Send invoices via email

### Plugin Architecture (Future)
- Output format plugins
- Calculation plugins (for complex tax scenarios)
- Template plugins

## Testability and Testing Strategy

### Design for Testability

#### Dependency Injection
All services will use constructor-based dependency injection to enable easy mocking and testing:

```csharp
public interface IInvoiceStorageService
{
    Task<string> SaveInvoiceAsync(Invoice invoice);
    Task<Invoice> GetInvoiceByIdAsync(string invoiceId);
}

public interface ICurrencyFormattingService
{
    string FormatAmount(decimal amount, string currency, string culture);
    int GetDecimalPlaces(string currency);
}

public interface IConfigurationService
{
    AppConfiguration LoadConfiguration();
    void SaveConfiguration(AppConfiguration config);
}
```

#### Abstraction Layers
- **Database Access**: `IDbConnection` interface for SQLite abstraction
- **File System**: `IFileSystem` wrapper for file operations
- **Console I/O**: `IConsoleWrapper` for input/output operations
- **DateTime**: `IDateTimeProvider` for testable time operations

### Test Project Structure
```
InvoiceGenerator.Tests/
├── Unit/
│   ├── Services/
│   │   ├── InvoiceCalculationServiceTests.cs
│   │   ├── CurrencyFormattingServiceTests.cs
│   │   ├── InputValidationServiceTests.cs
│   │   ├── InvoiceStorageServiceTests.cs
│   │   └── InvoiceRetrievalServiceTests.cs
│   ├── Models/
│   │   ├── InvoiceTests.cs
│   │   └── InvoiceItemTests.cs
│   └── Extensions/
│       └── CultureExtensionsTests.cs
├── Integration/
│   ├── DatabaseIntegrationTests.cs
│   ├── ConfigurationIntegrationTests.cs
│   └── CrossPlatformTests.cs
├── Fixtures/
│   ├── TestData.cs
│   ├── MockServices.cs
│   └── TestDatabaseFixture.cs
└── Helpers/
    ├── TestConsole.cs
    └── TestFileSystem.cs
```

### Unit Testing Approach

#### Service Layer Testing
```csharp
[Test]
public void CalculateSubtotal_WithMultipleItems_ReturnsCorrectSum()
{
    // Arrange
    var service = new InvoiceCalculationService();
    var items = new List<InvoiceItem>
    {
        new() { Quantity = 2, UnitPrice = 100.00m },
        new() { Quantity = 1, UnitPrice = 50.00m }
    };

    // Act
    var result = service.CalculateSubtotal(items);

    // Assert
    Assert.AreEqual(250.00m, result);
}

[Test]
public void FormatAmount_WithSGD_ReturnsCorrectFormat()
{
    // Arrange
    var service = new CurrencyFormattingService();

    // Act
    var result = service.FormatAmount(1234.56m, "SGD", "en-SG");

    // Assert
    Assert.AreEqual("S$1,234.56", result);
}
```

#### Database Testing with In-Memory SQLite
```csharp
public class InvoiceStorageServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InvoiceStorageService _service;

    public InvoiceStorageServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Initialize schema
        var service = new DatabaseService(_connection);
        service.InitializeDatabaseAsync().Wait();

        _service = new InvoiceStorageService(_connection);
    }

    [Test]
    public async Task SaveInvoice_ValidInvoice_ReturnsInvoiceId()
    {
        // Arrange
        var invoice = CreateTestInvoice();

        // Act
        var invoiceId = await _service.SaveInvoiceAsync(invoice);

        // Assert
        Assert.IsNotNull(invoiceId);
        Assert.IsTrue(invoiceId.StartsWith("INV-"));
    }

    public void Dispose() => _connection?.Dispose();
}
```

#### Input Validation Testing
```csharp
[TestCase("", typeof(ArgumentException))]
[TestCase("  ", typeof(ArgumentException))]
[TestCase(null, typeof(ArgumentNullException))]
[TestCase("Valid Client", null)]
public void ValidateClientName_WithVariousInputs_HandlesCorrectly(
    string input, Type expectedExceptionType)
{
    // Arrange
    var service = new InputValidationService();

    // Act & Assert
    if (expectedExceptionType != null)
    {
        Assert.Throws(expectedExceptionType,
            () => service.ValidateClientName(input));
    }
    else
    {
        Assert.DoesNotThrow(() => service.ValidateClientName(input));
    }
}
```

### Integration Testing

#### Database Integration Tests
```csharp
[TestFixture]
public class DatabaseIntegrationTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly SqliteConnection _connection;

    public DatabaseIntegrationTests()
    {
        _testDbPath = Path.GetTempFileName();
        _connection = new SqliteConnection($"Data Source={_testDbPath}");
    }

    [Test]
    public async Task CompleteInvoiceWorkflow_SaveAndRetrieve_WorksCorrectly()
    {
        // Arrange
        var storageService = new InvoiceStorageService(_connection);
        var retrievalService = new InvoiceRetrievalService(_connection);
        var testInvoice = CreateCompleteTestInvoice();

        // Act
        var invoiceId = await storageService.SaveInvoiceAsync(testInvoice);
        var retrievedInvoice = await retrievalService.GetInvoiceByIdAsync(invoiceId);

        // Assert
        Assert.AreEqual(testInvoice.Client.Name, retrievedInvoice.Client.Name);
        Assert.AreEqual(testInvoice.Items.Count, retrievedInvoice.Items.Count);
        Assert.AreEqual(testInvoice.Total, retrievedInvoice.Total);
    }
}
```

#### Configuration Integration Tests
```csharp
[Test]
public void ConfigurationService_SaveAndLoad_PreservesAllSettings()
{
    // Arrange
    var tempConfigPath = Path.GetTempFileName();
    var service = new ConfigurationService(tempConfigPath);
    var originalConfig = new AppConfiguration
    {
        Currency = "USD",
        Culture = "en-US",
        DefaultTaxRate = 8.5m
    };

    // Act
    service.SaveConfiguration(originalConfig);
    var loadedConfig = service.LoadConfiguration();

    // Assert
    Assert.AreEqual(originalConfig.Currency, loadedConfig.Currency);
    Assert.AreEqual(originalConfig.Culture, loadedConfig.Culture);
    Assert.AreEqual(originalConfig.DefaultTaxRate, loadedConfig.DefaultTaxRate);
}
```

### Test Data and Fixtures

#### Test Data Factory
```csharp
public static class TestDataFactory
{
    public static Invoice CreateBasicInvoice(string clientName = "Test Client")
    {
        return new Invoice
        {
            Client = new Client { Name = clientName },
            Items = new List<InvoiceItem>
            {
                new() { Description = "Test Service", Quantity = 1, UnitPrice = 100.00m }
            },
            TaxRate = 7.0m,
            InvoiceDate = DateTime.Today
        };
    }

    public static List<InvoiceItem> CreateTestItems(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new InvoiceItem
            {
                Description = $"Test Item {i}",
                Quantity = i,
                UnitPrice = i * 10.00m
            })
            .ToList();
    }
}
```

#### Mock Services
```csharp
public class MockConfigurationService : IConfigurationService
{
    private AppConfiguration _config = new();

    public AppConfiguration LoadConfiguration() => _config;
    public void SaveConfiguration(AppConfiguration config) => _config = config;
}

public class MockConsole : IConsoleWrapper
{
    private readonly Queue<string> _inputs = new();
    private readonly List<string> _outputs = new();

    public void AddInput(string input) => _inputs.Enqueue(input);
    public string ReadLine() => _inputs.Dequeue();
    public void WriteLine(string line) => _outputs.Add(line);
    public List<string> GetOutputs() => _outputs;
}
```

### Cross-Platform Testing

#### Platform-Specific Test Categories
```csharp
[Test]
[Platform("Win")]
public void WindowsSpecific_PathHandling_WorksCorrectly()
{
    // Windows-specific path tests
}

[Test]
[Platform("Linux,MacOSX")]
public void UnixSpecific_PathHandling_WorksCorrectly()
{
    // Unix-specific path tests
}
```

#### Culture-Specific Testing
```csharp
[TestCase("en-US", "USD", "$1,234.56")]
[TestCase("en-SG", "SGD", "S$1,234.56")]
[TestCase("ja-JP", "JPY", "¥1,235")]
public void CurrencyFormatting_VariousCultures_FormatsCorrectly(
    string culture, string currency, string expected)
{
    // Test culture-specific formatting
}
```

### Continuous Integration Testing

#### Test Categories for CI/CD
```csharp
[Category("Unit")]     // Fast tests, no external dependencies
[Category("Integration")]  // Database tests with SQLite
[Category("CrossPlatform")]  // Platform-specific tests
[Category("Performance")]    // Performance benchmarks
```

### Testing Tools and Libraries
- **NUnit** - Primary testing framework
- **Moq** - Mocking framework for interfaces
- **FluentAssertions** - More readable assertions
- **Microsoft.Data.Sqlite** - In-memory database for tests
- **System.IO.Abstractions.TestingHelpers** - File system mocking

This comprehensive testing strategy ensures that all components can be tested in isolation, integration points are verified, and the application works consistently across platforms.
