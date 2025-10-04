using FluentAssertions;
using InvoiceGenerator.Core.Models;
using InvoiceGenerator.Core.Services;

namespace InvoiceGenerator.Core.Tests.Services;

[TestFixture]
public class InvoiceCalculationServiceTests
{
    private InvoiceCalculationService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new InvoiceCalculationService();
    }

    [TestFixture]
    public class CalculateSubtotal : InvoiceCalculationServiceTests
    {
        [Test]
        public void WithSingleItem_ReturnsItemLineTotal()
        {
            // Arrange
            var items = new List<InvoiceItem>
            {
                new() { Quantity = 2, UnitPrice = 100.00m }
            };

            // Act
            var result = _service.CalculateSubtotal(items);

            // Assert
            result.Should().Be(200.00m);
        }

        [Test]
        public void WithMultipleItems_ReturnsSumOfLineTotals()
        {
            // Arrange
            var items = new List<InvoiceItem>
            {
                new() { Quantity = 2, UnitPrice = 100.00m },
                new() { Quantity = 1, UnitPrice = 50.00m },
                new() { Quantity = 3, UnitPrice = 25.00m }
            };

            // Act
            var result = _service.CalculateSubtotal(items);

            // Assert
            result.Should().Be(325.00m); // 200 + 50 + 75
        }

        [Test]
        public void WithEmptyList_ReturnsZero()
        {
            // Arrange
            var items = new List<InvoiceItem>();

            // Act
            var result = _service.CalculateSubtotal(items);

            // Assert
            result.Should().Be(0.00m);
        }

        [Test]
        public void WithNullItems_ThrowsArgumentNullException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateSubtotal(null!))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("items");
        }
    }

    [TestFixture]
    public class CalculateTax : InvoiceCalculationServiceTests
    {
        [Test]
        public void WithValidSubtotalAndTaxRate_ReturnsCorrectTaxAmount()
        {
            // Arrange
            var subtotal = 100.00m;
            var taxRate = 7.0m;

            // Act
            var result = _service.CalculateTax(subtotal, taxRate);

            // Assert
            result.Should().Be(7.00m);
        }

        [Test]
        public void WithZeroTaxRate_ReturnsZero()
        {
            // Arrange
            var subtotal = 100.00m;
            var taxRate = 0.0m;

            // Act
            var result = _service.CalculateTax(subtotal, taxRate);

            // Assert
            result.Should().Be(0.00m);
        }

        [Test]
        public void WithDecimalResult_RoundsToTwoDecimalPlaces()
        {
            // Arrange
            var subtotal = 123.45m;
            var taxRate = 7.0m;

            // Act
            var result = _service.CalculateTax(subtotal, taxRate);

            // Assert
            result.Should().Be(8.64m); // 123.45 * 0.07 = 8.6415, rounded to 8.64
        }

        [Test]
        public void WithNegativeSubtotal_ThrowsArgumentException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateTax(-100.00m, 7.0m))
                .Should().Throw<ArgumentException>()
                .WithParameterName("subtotal")
                .WithMessage("Subtotal cannot be negative*");
        }

        [Test]
        public void WithNegativeTaxRate_ThrowsArgumentException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateTax(100.00m, -1.0m))
                .Should().Throw<ArgumentException>()
                .WithParameterName("taxRate")
                .WithMessage("Tax rate must be between 0 and 100*");
        }

        [Test]
        public void WithTaxRateAbove100_ThrowsArgumentException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateTax(100.00m, 101.0m))
                .Should().Throw<ArgumentException>()
                .WithParameterName("taxRate")
                .WithMessage("Tax rate must be between 0 and 100*");
        }
    }

    [TestFixture]
    public class CalculateTotal : InvoiceCalculationServiceTests
    {
        [Test]
        public void WithValidSubtotalAndTaxAmount_ReturnsSum()
        {
            // Arrange
            var subtotal = 100.00m;
            var taxAmount = 7.00m;

            // Act
            var result = _service.CalculateTotal(subtotal, taxAmount);

            // Assert
            result.Should().Be(107.00m);
        }

        [Test]
        public void WithZeroTaxAmount_ReturnsSubtotal()
        {
            // Arrange
            var subtotal = 100.00m;
            var taxAmount = 0.00m;

            // Act
            var result = _service.CalculateTotal(subtotal, taxAmount);

            // Assert
            result.Should().Be(100.00m);
        }

        [Test]
        public void WithNegativeSubtotal_ThrowsArgumentException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateTotal(-100.00m, 7.00m))
                .Should().Throw<ArgumentException>()
                .WithParameterName("subtotal")
                .WithMessage("Subtotal cannot be negative*");
        }

        [Test]
        public void WithNegativeTaxAmount_ThrowsArgumentException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateTotal(100.00m, -7.00m))
                .Should().Throw<ArgumentException>()
                .WithParameterName("taxAmount")
                .WithMessage("Tax amount cannot be negative*");
        }
    }

    [TestFixture]
    public class CalculateInvoiceTotals : InvoiceCalculationServiceTests
    {
        [Test]
        public void WithValidInvoice_CalculatesAllTotalsCorrectly()
        {
            // Arrange
            var invoice = new Invoice
            {
                TaxRate = 7.0m,
                Items = new List<InvoiceItem>
                {
                    new() { Quantity = 2, UnitPrice = 100.00m },
                    new() { Quantity = 1, UnitPrice = 50.00m }
                }
            };

            // Act
            _service.CalculateInvoiceTotals(invoice);

            // Assert
            invoice.Subtotal.Should().Be(250.00m);
            invoice.TaxAmount.Should().Be(17.50m);
            invoice.Total.Should().Be(267.50m);
        }

        [Test]
        public void WithNullInvoice_ThrowsArgumentNullException()
        {
            // Act & Assert
            _service.Invoking(s => s.CalculateInvoiceTotals(null!))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("invoice");
        }
    }
}
