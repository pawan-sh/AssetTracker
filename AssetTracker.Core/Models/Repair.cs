public class Repair
{
    public int RepairId { get; set; }

    public int IssueId { get; set; }

    public string TechnicianName { get; set; } = "";
    public string TechnicianPhone { get; set; } = "";

    public string InvoiceNumber { get; set; } = "";
    public decimal RepairCost { get; set; }

    public DateTime RepairDate { get; set; }

    public string? RepairNotes { get; set; }
}
