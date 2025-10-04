using System;
using System.Collections.Generic;

namespace InvoiceGenerator.Core.Models
{
    /// <summary>
    /// Represents a complete invoice with all associated data.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Gets or sets the unique invoice identifier (e.g., "INV-2025-001").
        /// </summary>
        public string InvoiceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client information.
        /// </summary>
        public Client Client { get; set; } = new Client();

        /// <summary>
        /// Gets or sets the list of invoice items.
        /// </summary>
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        /// <summary>
        /// Gets or sets the tax rate as a percentage (e.g., 7.0 for 7%).
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the date when the invoice was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the calculated subtotal before tax.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the calculated tax amount.
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets the calculated total amount including tax.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the currency code (e.g., "SGD", "USD", "JPY").
        /// </summary>
        public string Currency { get; set; } = "SGD";

        /// <summary>
        /// Gets or sets the culture code for formatting (e.g., "en-SG", "en-US").
        /// </summary>
        public string Culture { get; set; } = "en-SG";
    }
}
