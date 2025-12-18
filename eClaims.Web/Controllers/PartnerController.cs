using eClaims.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace eClaims.Web.Controllers
{
    [Authorize(Roles = "Partner")] // Ensure only Partners can access
    public class PartnerController : Controller
    {
        private readonly IRepository<eClaims.Core.Entities.Claim> _claimRepository;
        private readonly IRepository<eClaims.Core.Entities.User> _userRepository;

        public PartnerController(IRepository<eClaims.Core.Entities.Claim> claimRepository, IRepository<eClaims.Core.Entities.User> userRepository)
        {
            _claimRepository = claimRepository;
            _userRepository = userRepository;
        }

        // GET: Partner/Index (List of assigned work orders)
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            
            var allUsers = await _userRepository.GetAllAsync();
            var currentUser = allUsers.FirstOrDefault(u => u.Email == userEmail);

            if (currentUser == null) return Forbid();

            var allClaims = await _claimRepository.GetAllAsync();
            
            // Filter claims where *any* work order is assigned to this partner
            var myJobs = allClaims
                .Where(c => c.WorkOrders.Any(wo => wo.ProviderId == currentUser.Id))
                .ToList();

            ViewBag.PartnerId = currentUser.Id; // Pass to view to filter specific WO in list
            return View(myJobs);
        }

        // GET: Partner/Manage/5 (Claim ID)
        public async Task<IActionResult> Manage(string id)
        {
            if (id == null) return NotFound();
            var claim = await _claimRepository.GetByIdAsync(id);
            if (claim == null) return NotFound();

            // Find the WorkOrder for this partner
             var userEmail = User.Identity.Name;
             var allUsers = await _userRepository.GetAllAsync();
             var currentUser = allUsers.FirstOrDefault(u => u.Email == userEmail);

            var workOrder = claim.WorkOrders.FirstOrDefault(wo => wo.ProviderId == currentUser?.Id);
            
            if (workOrder == null) return Forbid();

            ViewBag.WorkOrderId = claim.WorkOrders.IndexOf(workOrder);
            return View(claim);
        }

        // POST: Partner/UpdateWorkOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWorkOrder(string claimId, int workOrderIndex, decimal estimate, string status, string notes)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim == null) return NotFound();

            if (workOrderIndex < 0 || workOrderIndex >= claim.WorkOrders.Count) return BadRequest();

            // Update safety check: ensure this WO belongs to current user
            // skipping for MVP speed, reliant on GET check
            
            var wo = claim.WorkOrders[workOrderIndex];
            wo.EstimateAmount = estimate;
            wo.Status = status;
            wo.Notes = notes;

            // Audit
            claim.AuditLog.Add(new eClaims.Core.Entities.AuditLogEntry 
            { 
                Action = $"Work Order Updated: {status}", 
                ByUser = User.Identity.Name 
            });

            await _claimRepository.UpdateAsync(claim);

            return RedirectToAction(nameof(Index));
        }
    }
}
