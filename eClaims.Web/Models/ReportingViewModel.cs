namespace eClaims.Web.Models
{
    public class ReportingViewModel
    {
        public int TotalClaims { get; set; }
        public int PendingReview { get; set; }
        public int Approved { get; set; }
        public double AverageProcessingTimeDays { get; set; }
        
        // Data for Charts
        public Dictionary<string, int> ClaimsByStatus { get; set; } = new();
        public Dictionary<string, int> ClaimsByLocation { get; set; } = new();
        public Dictionary<string, int> AgeingMatrix { get; set; } = new(); // "0-7 Days", "8-30 Days", "30+ Days"
    }
}
