using eClaims.Core.Entities;
using eClaims.Core.Interfaces;

namespace eClaims.Web.Services
{
    public static class DataSeeder
    {
        public static async Task SeedData(IRepository<Claim> claimRepo, IRepository<User> userRepo)
        {
            var existing = await claimRepo.GetAllAsync();
            if (existing.Count > 0) return; // Already seeded

            // Seed Admin User
            var admin = new User
            {
                Email = "admin@ycompany.com",
                PasswordHash = "Password123!",
                Role = "Admin",
                Profile = new UserProfile { FirstName = "Site", LastName = "Admin" }
            };
            await userRepo.AddAsync(admin);

            // Seed Claims for Reports
            var claims = new List<Claim>
            {
                // New York - Recent
                new Claim { 
                    PolicyNumber = "POL-NY-001", 
                    Status = "SUBMITTED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-2), Location = "New York", Description = "Fender bender on 5th Ave" } 
                },
                new Claim { 
                    PolicyNumber = "POL-NY-002", 
                    Status = "APPROVED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-5), Location = "New York", Description = "Broken windshield" },
                    CreatedAt = DateTime.Now.AddDays(-4),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                 // California - Older (Ageing)
                new Claim { 
                    PolicyNumber = "POL-CA-045", 
                    Status = "SUBMITTED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-35), Location = "California", Description = "Earthquake damage" },
                    CreatedAt = DateTime.Now.AddDays(-34)
                },
                new Claim { 
                    PolicyNumber = "POL-CA-088", 
                    Status = "REJECTED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-10), Location = "California", Description = "Suspicious water damage" },
                    CreatedAt = DateTime.Now.AddDays(-9),
                    UpdatedAt = DateTime.Now.AddDays(-8)
                },
                // Texas - Mid-term
                new Claim { 
                    PolicyNumber = "POL-TX-102", 
                    Status = "APPROVED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-15), Location = "Texas", Description = "Hail storm damage" },
                    CreatedAt = DateTime.Now.AddDays(-14),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                },
                new Claim { 
                    PolicyNumber = "POL-TX-103", 
                    Status = "REVIEW", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-12), Location = "Texas", Description = "Flood in basement" },
                    CreatedAt = DateTime.Now.AddDays(-11)
                },
                // Florida - Recent
                new Claim { 
                    PolicyNumber = "POL-FL-221", 
                    Status = "SUBMITTED", 
                    Incident = new IncidentDetails { Date = DateTime.Now.AddDays(-1), Location = "Florida", Description = "Wind damage to roof" } 
                }
            };

            // Seed Partner User
            var partner = new User
            {
                Email = "shop@partner.com",
                PasswordHash = "Password123!",
                Role = "Partner",
                Profile = new UserProfile { FirstName = "Bob", LastName = "Builder" }
            };
            await userRepo.AddAsync(partner);

            foreach (var c in claims)
            {
                c.UserId = admin.Id;
                
                // Assign a work order to the Partner for some claims
                if (c.Status == "APPROVED")
                {
                    c.WorkOrders.Add(new WorkOrder 
                    { 
                        ProviderId = partner.Id, 
                        Status = "PENDING",
                        EstimateAmount = 0,
                        Notes = "Awaiting estimate"
                    });
                }

                await claimRepo.AddAsync(c);
            }
        }
    }
}
