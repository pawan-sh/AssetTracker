namespace AssetTracker.Core.Models
{
    public class AssetAssignment
    {
        public int AssetAssignmentId { get; set; }

        public int AssetId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }

        public Asset? Asset { get; set; }
        public Employee? Employee { get; set; }
    }
}
