namespace InvoiceGenerator.Core.Models
{
    /// <summary>
    /// Represents an individual line item on an invoice.
    /// </summary>
    public class InvoiceItem
    {
        /// <summary>
        /// Gets or sets the description of the item or service.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity of items.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price for a single item.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets the calculated line total (Quantity × UnitPrice).
        /// </summary>
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
