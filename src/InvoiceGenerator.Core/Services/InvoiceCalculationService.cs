using System;
using System.Collections.Generic;
using System.Linq;
using InvoiceGenerator.Core.Interfaces;
using InvoiceGenerator.Core.Models;

namespace InvoiceGenerator.Core.Services
{
    /// <summary>
    /// Provides invoice calculation services.
    /// </summary>
    public class InvoiceCalculationService : IInvoiceCalculationService
    {
        /// <inheritdoc />
        public decimal CalculateSubtotal(IEnumerable<InvoiceItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.Sum(item => item.LineTotal);
        }

        /// <inheritdoc />
        public decimal CalculateTax(decimal subtotal, decimal taxRate)
        {
            if (subtotal < 0)
                throw new ArgumentException("Subtotal cannot be negative", nameof(subtotal));

            if (taxRate < 0 || taxRate > 100)
                throw new ArgumentException("Tax rate must be between 0 and 100", nameof(taxRate));

            return Math.Round(subtotal * (taxRate / 100), 2, MidpointRounding.AwayFromZero);
        }

        /// <inheritdoc />
        public decimal CalculateTotal(decimal subtotal, decimal taxAmount)
        {
            if (subtotal < 0)
                throw new ArgumentException("Subtotal cannot be negative", nameof(subtotal));

            if (taxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative", nameof(taxAmount));

            return subtotal + taxAmount;
        }

        /// <inheritdoc />
        public void CalculateInvoiceTotals(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            invoice.Subtotal = CalculateSubtotal(invoice.Items);
            invoice.TaxAmount = CalculateTax(invoice.Subtotal, invoice.TaxRate);
            invoice.Total = CalculateTotal(invoice.Subtotal, invoice.TaxAmount);
        }
    }
}
