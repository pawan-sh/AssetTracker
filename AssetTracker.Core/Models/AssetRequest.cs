namespace AssetTracker.Core.Models
{
    public class AssetRequest
    {
        public int AssetRequestId { get; set; }
        public string AssetName { get; set; } = "";
        public string AssetType { get; set; } = "";
        public string Purpose { get; set; } = "";
        public string RequestedBy { get; set; } = "";
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? AdminComments { get; set; }
    }
}
