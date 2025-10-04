using System.Collections.Generic;
using InvoiceGenerator.Core.Models;

namespace InvoiceGenerator.Core.Interfaces
{
    /// <summary>
    /// Provides invoice calculation services.
    /// </summary>
    public interface IInvoiceCalculationService
    {
        /// <summary>
        /// Calculates the subtotal from a collection of invoice items.
        /// </summary>
        /// <param name="items">The invoice items to calculate the subtotal for.</param>
        /// <returns>The calculated subtotal.</returns>
        decimal CalculateSubtotal(IEnumerable<InvoiceItem> items);

        /// <summary>
        /// Calculates the tax amount based on subtotal and tax rate.
        /// </summary>
        /// <param name="subtotal">The subtotal amount.</param>
        /// <param name="taxRate">The tax rate as a percentage (e.g., 7.0 for 7%).</param>
        /// <returns>The calculated tax amount.</returns>
        decimal CalculateTax(decimal subtotal, decimal taxRate);

        /// <summary>
        /// Calculates the total amount by adding subtotal and tax amount.
        /// </summary>
        /// <param name="subtotal">The subtotal amount.</param>
        /// <param name="taxAmount">The tax amount.</param>
        /// <returns>The calculated total amount.</returns>
        decimal CalculateTotal(decimal subtotal, decimal taxAmount);

        /// <summary>
        /// Calculates and updates all totals for an invoice.
        /// </summary>
        /// <param name="invoice">The invoice to calculate totals for.</param>
        void CalculateInvoiceTotals(Invoice invoice);
    }
}
