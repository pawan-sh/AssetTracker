namespace AssetTracker.Core.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string AssetType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string Status { get; set; } = "Available";
    }
}
