namespace InvoiceGenerator.Core.Models
{
    /// <summary>
    /// Represents a client for invoicing purposes.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the client's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        // Future expansion: Address, contact information, payment terms, etc.
    }
}
