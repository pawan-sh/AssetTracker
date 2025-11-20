namespace AssetTracker.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int TotalEmployees { get; set; }
        public int PendingIssues { get; set; }
        public int CompletedRepairs { get; set; }
    }
}
