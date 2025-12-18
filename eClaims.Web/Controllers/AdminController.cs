using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using eClaims.Web.Models;

namespace eClaims.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IRepository<Claim> _claimRepository;

        public AdminController(IRepository<Claim> claimRepository)
        {
            _claimRepository = claimRepository;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _claimRepository.GetAllAsync();
            
            // Basic Counts
            var total = claims.Count;
            var pending = claims.Count(c => c.Status == "SUBMITTED" || c.Status == "REVIEW");
            var approved = claims.Count(c => c.Status == "APPROVED");

            // Processing Time (Simulated logic: CreatedAt vs UpdatedAt for closed claims)
            var closedClaims = claims.Where(c => c.Status == "APPROVED" || c.Status == "REJECTED").ToList();
            double avgTime = 0;
            if (closedClaims.Any())
            {
                avgTime = closedClaims.Average(c => (c.UpdatedAt.GetValueOrDefault(DateTime.Now) - c.CreatedAt).TotalDays);
            }

            // Geo Stats
            var byLocation = claims
                .GroupBy(c => c.Incident.Location ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            // Status Stats
            var byStatus = claims
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            // Ageing Matrix (For active claims)
            var activeClaims = claims.Where(c => c.Status != "APPROVED" && c.Status != "REJECTED");
            var ageing = new Dictionary<string, int>
            {
                { "0-7 Days", activeClaims.Count(c => (DateTime.UtcNow - c.CreatedAt).TotalDays <= 7) },
                { "8-30 Days", activeClaims.Count(c => (DateTime.UtcNow - c.CreatedAt).TotalDays > 7 && (DateTime.UtcNow - c.CreatedAt).TotalDays <= 30) },
                { "30+ Days", activeClaims.Count(c => (DateTime.UtcNow - c.CreatedAt).TotalDays > 30) }
            };

            var stats = new ReportingViewModel
            {
                TotalClaims = total,
                PendingReview = pending,
                Approved = approved,
                AverageProcessingTimeDays = Math.Round(avgTime, 1),
                ClaimsByLocation = byLocation,
                ClaimsByStatus = byStatus,
                AgeingMatrix = ageing
            };
            return View(stats);
        }
    }
}
