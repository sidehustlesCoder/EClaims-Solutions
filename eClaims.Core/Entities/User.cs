using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eClaims.Core.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "CUSTOMER"; // CUSTOMER, PARTNER, ADMIN, ADJUSTOR
        public UserProfile Profile { get; set; } = new();
    }

    public class UserProfile
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
