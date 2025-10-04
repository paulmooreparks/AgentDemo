# Invoice Generator CLI — Requirements

## Purpose
Create a command-line tool for freelancers and small businesses to generate clean, itemized invoices quickly and reliably.

## Functional Requirements
- Prompt user for:
  - Client name
  - List of items (description, quantity, unit price)
  - Tax rate (percentage) - with configurable default
- Calculate:
  - Subtotal (in configured currency)
  - Tax amount (in configured currency)
  - Total (in configured currency)
- Output:
  - Formatted invoice in console with culture-appropriate currency formatting
  - Optionally save to file (Markdown or plain text)
- Configuration:
  - Store application settings in dot directory in user's home directory (`~/.invoice-generator/`)
  - Configuration file should support:
    - Language/Culture settings (e.g., en-US, ja-JP, en-SG)
    - Currency code (e.g., USD, JPY, SGD)
    - Default tax rate
    - Currency formatting adapts to selected currency (decimals for SGD/USD, no decimals for JPY)

## Non-Functional Requirements
- Must run as a .NET console application
- Must be cross-platform compatible (Windows, Linux, macOS)
- Should be easy to extend (e.g., discount codes, currency formatting)
- Should handle invalid input gracefully
- Should be readable and maintainable
- Error handling: Throw exceptions immediately (do not swallow exceptions)

## Stretch Goals (if time allows)
- Support for discount codes
- Date stamping and invoice numbering
- Configurable output format
