using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace eClaims.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly IRepository<Claim> _claimRepository;
        private readonly IRepository<User> _userRepository;

        public SeedController(IRepository<Claim> claimRepository, IRepository<User> userRepository = null)
        {
            _claimRepository = claimRepository;
            _userRepository = userRepository; // Optional if not registered yet
        }

        [HttpPost]
        public async Task<IActionResult> Seed()
        {
            // Seed Claims
            var existingClaims = await _claimRepository.GetAllAsync();
            if (existingClaims.Count == 0)
            {
                var claims = new List<Claim>
                {
                    new Claim
                    {
                        PolicyNumber = "POL-1001",
                        Status = "SUBMITTED",
                        Incident = new IncidentDetails
                        {
                            Date = DateTime.UtcNow.AddDays(-10),
                            Description = "Minor collision with a pole.",
                            Location = "Downtown"
                        },
                        WorkOrders = new List<WorkOrder>
                        {
                            new WorkOrder { ProviderId = ObjectId.GenerateNewId().ToString(), EstimateAmount = 500, Status = "PENDING", Notes = "Bumper repair" }
                        }
                    },
                     new Claim
                    {
                        PolicyNumber = "POL-1002",
                        Status = "APPROVED",
                        Incident = new IncidentDetails
                        {
                            Date = DateTime.UtcNow.AddDays(-30),
                            Description = "Windshield crack.",
                            Location = "Highway 401"
                        }
                    }
                };

                foreach (var c in claims)
                {
                    await _claimRepository.AddAsync(c);
                }
            }

            return Ok(new { Message = "Database seeded successfully" });
        }
    }
}
