# Invoice Generator CLI - Implementation Guidelines

Acknowledge that you have read and understood these detailed implementation guidelines before starting development.

## Project Overview
Building a cross-platform .NET console application for generating itemized invoices with configurable currency and cultural formatting.

## Critical Architecture Principles

- Use the `dotnet` cli to create and manage projects and solutions. DO NOT edit .sln or .csproj files manually.

### Multi-Assembly Design (NON-NEGOTIABLE)
```
InvoiceGenerator.Core/         # Pure business logic, no external dependencies
InvoiceGenerator.Infrastructure/  # Data access, configuration, external services
InvoiceGenerator.Console/      # CLI-specific UI and user interaction
```

**Dependency Rules:**
- Core has NO dependencies on other assemblies
- Infrastructure depends ONLY on Core
- Console depends on both Core and Infrastructure
- NO circular dependencies allowed

## Core Requirements to Always Remember

### Currency & Culture Handling
- **Configurable**: Support multiple currencies (SGD, USD, JPY, EUR)
- **Culture-Aware**: Format numbers/currency according to culture (en-SG, ja-JP, etc.)
- **Adaptive Formatting**: JPY has no decimals, SGD/USD have 2 decimals
- **Default**: Start with SGD and en-SG culture

### Data Storage
- **SQLite Database**: Primary storage (NOT file-based)
- **Location**: `~/.invoice-generator/` directory
- **Future-Proof**: Design for easy migration to enterprise databases
- **Schema**: Normalized tables with proper relationships and indexes

### Error Handling Strategy
- **Fail Fast**: Throw exceptions immediately
- **DO NOT swallow exceptions** - let them bubble up for visibility
- **Specific Exceptions**: Use appropriate exception types with clear messages

### Cross-Platform Requirements
- **Windows, Linux, macOS** compatibility required
- Use `Path.Combine()` for all path operations
- Use `Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)` for home directory
- UTF-8 encoding for all file operations

### Configuration Management
- **Location**: `~/.invoice-generator/config.json`
- **Settings**: Culture, Currency, Default Tax Rate, Date Format
- **Validation**: Ensure valid culture/currency codes on startup
- **Defaults**: Create default config on first run

## Business Logic Rules

### Invoice Calculation
1. **Subtotal**: Sum of all (quantity × unit price)
2. **Tax Amount**: Subtotal × tax rate
3. **Total**: Subtotal + tax amount

### Invoice ID Format
- **Pattern**: `INV-YYYY-NNN` (e.g., `INV-2025-001`)
- **YYYY**: Current year
- **NNN**: Sequential number starting from 001

### Validation Rules
- **Client Name**: Required, non-empty
- **Item Description**: Required, non-empty
- **Quantity**: Positive integers only
- **Unit Price**: Positive decimals only
- **Tax Rate**: 0-100 percentage

## Database Schema (SQLite)

### Tables
```sql
Invoices (InvoiceId, ClientName, InvoiceDate, CreatedDate, ModifiedDate,
          Subtotal, TaxRate, TaxAmount, Total, Currency, Culture, Notes)

InvoiceItems (Id, InvoiceId, Description, Quantity, UnitPrice, LineTotal, SortOrder)

Clients (ClientName, ContactInfo, CreatedDate, LastUsedDate)
```

### Required Indexes
- IX_Invoices_ClientName, IX_Invoices_InvoiceDate, IX_Invoices_CreatedDate
- IX_InvoiceItems_InvoiceId

## Testing Strategy

### Testability Requirements
- **Dependency Injection**: All services use constructor injection
- **Interface Abstraction**: Abstract all external dependencies
- **In-Memory Testing**: Use in-memory SQLite for database tests
- **Mock Services**: Console I/O, file system, datetime operations

### Test Structure
- **Unit Tests**: Fast tests for Core business logic
- **Integration Tests**: Database and configuration operations
- **Cross-Platform Tests**: Verify behavior on all platforms

## Key Interfaces to Implement

### Core Interfaces
```csharp
IInvoiceCalculationService  // Business calculations
ICurrencyFormattingService  // Culture-aware formatting
IInputValidationService     // Business validation rules
IDateTimeProvider          // Testable time operations
```

### Infrastructure Interfaces
```csharp
IConfigurationService      // Config file management
IInvoiceRepository        // Database operations
IDatabaseService          // Database initialization
IFileSystemService       // File operations abstraction
```

### Console Interfaces
```csharp
IConsoleWrapper           // Testable console I/O
```

## Output Requirements

### Console Display
- **Formatted Invoice**: Clean, readable layout with culture-appropriate currency formatting
- **Input Prompts**: Clear prompts with default values shown
- **Error Messages**: Descriptive error messages for validation failures

### File Export (Optional)
- **Formats**: Markdown, plain text
- **Location**: User-specified or default export directory

## Extension Points for Future

### Planned Extensions
- **Discount Codes**: Additional calculation service
- **PDF Export**: Output handler plugin
- **Email Integration**: External service integration
- **Multiple UIs**: Web API, desktop, mobile

### Plugin Architecture
- Output format plugins
- Calculation plugins for complex scenarios
- Template plugins for customization

## Libraries and Dependencies

### Core (Minimal Dependencies)
- System.Globalization (culture handling)
- System.Text.Json (configuration serialization)

### Infrastructure
- Microsoft.Data.Sqlite (database operations)
- Dapper (lightweight ORM)
- System.IO.Abstractions (testable file operations)

### Console
- Microsoft.Extensions.DependencyInjection (DI container)
- Microsoft.Extensions.Configuration (configuration management)

## Important Reminders for Implementation

1. **Always use interfaces** for service dependencies
2. **Validate configuration** on application startup
3. **Generate invoice IDs** from database queries, not file systems
4. **Use culture-specific formatting** throughout the application
5. **Test on multiple platforms** during development
6. **Keep Core assembly pure** - no external dependencies
7. **Design for multiple UIs** from the beginning
8. **Fail fast with clear error messages**
9. **Use proper decimal precision** for currency calculations
10. **Create database indexes** for performance

## Quick Reference

### Default Configuration
```json
{
  "culture": "en-SG",
  "currency": "SGD",
  "defaultTaxRate": 7.0,
  "dateFormat": "dd/MM/yyyy"
}
```

### Currency Formatting Examples
- SGD: S$1,234.56
- USD: $1,234.56
- JPY: ¥1,235 (no decimals)
- EUR: €1.234,56

### File Locations
- Config: `~/.invoice-generator/config.json`
- Database: `~/.invoice-generator/invoices.db`
- Exports: `~/.invoice-generator/exports/`
