using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eClaims.Core.Entities
{
    public class Claim : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        public string PolicyNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "DRAFT"; // DRAFT, SUBMITTED, IN_REVIEW, APPROVED, REJECTED, PAID
        
        public IncidentDetails Incident { get; set; } = new();
        
        public List<WorkOrder> WorkOrders { get; set; } = new();
        public List<AuditLogEntry> AuditLog { get; set; } = new();
        public List<ClaimDocument> Documents { get; set; } = new();
    }

    public class ClaimDocument
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public class IncidentDetails
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class WorkOrder
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProviderId { get; set; } = string.Empty; // Reference to a User (Partner)
        
        public decimal EstimateAmount { get; set; }
        public string Status { get; set; } = "PENDING";
        public string Notes { get; set; } = string.Empty;
    }

    public class AuditLogEntry
    {
        public string Action { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string ByUser { get; set; } = string.Empty;
    }
}
