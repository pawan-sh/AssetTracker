namespace AssetTracker.Core.Models
{
    public class Issue
    {
        public int IssueId { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReportedBy { get; set; } = "";
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; } = "Pending";
        public Repair? Repair { get; set; }

        public string? CompletionMessage { get; set; }
        public string? Resolution { get; set; }
        public decimal? Cost { get; set; }
        public string? InvoiceReference { get; set; }
        public string? InvoicePath { get; set; }

    }
}
