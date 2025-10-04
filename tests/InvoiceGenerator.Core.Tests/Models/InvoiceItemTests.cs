using FluentAssertions;
using InvoiceGenerator.Core.Models;

namespace InvoiceGenerator.Core.Tests.Models;

[TestFixture]
public class InvoiceItemTests
{
    [Test]
    public void LineTotal_CalculatesCorrectly()
    {
        // Arrange
        var item = new InvoiceItem
        {
            Quantity = 3,
            UnitPrice = 25.50m
        };

        // Act
        var lineTotal = item.LineTotal;

        // Assert
        lineTotal.Should().Be(76.50m);
    }

    [Test]
    public void LineTotal_WithZeroQuantity_ReturnsZero()
    {
        // Arrange
        var item = new InvoiceItem
        {
            Quantity = 0,
            UnitPrice = 100.00m
        };

        // Act
        var lineTotal = item.LineTotal;

        // Assert
        lineTotal.Should().Be(0.00m);
    }

    [Test]
    public void LineTotal_WithZeroPrice_ReturnsZero()
    {
        // Arrange
        var item = new InvoiceItem
        {
            Quantity = 5,
            UnitPrice = 0.00m
        };

        // Act
        var lineTotal = item.LineTotal;

        // Assert
        lineTotal.Should().Be(0.00m);
    }
}
