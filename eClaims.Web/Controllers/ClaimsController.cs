using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace eClaims.Web.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly IRepository<Claim> _claimRepository;
        private readonly INotificationService _notificationService;

        public ClaimsController(IRepository<Claim> claimRepository, INotificationService notificationService)
        {
            _claimRepository = claimRepository;
            _notificationService = notificationService;
        }

        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var claims = await _claimRepository.GetAllAsync();
            return View(claims);
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var claim = await _claimRepository.GetByIdAsync(id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, List<IFormFile> documents)
        {
            if (ModelState.IsValid)
            {
                // In a real app, UserID comes from Auth context
                // claim.UserId = User.Identity.GetUserId(); 
                if (User.Identity.IsAuthenticated)
                {
                     // Typically you'd get the actual ID claim
                     claim.UserId = User.Identity.Name ?? "Anonymous"; // Fallback
                }
                
                // Handle File Uploads
                if (documents != null && documents.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            claim.Documents.Add(new ClaimDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + fileName,
                                FileSize = file.Length,
                                ContentType = file.ContentType
                            });
                        }
                    }
                }

                await _claimRepository.AddAsync(claim);

                // Send Notification
                await _notificationService.SendAsync("admin@ycompany.com", "New Claim Submitted", $"Claim {claim.PolicyNumber} has been submitted.");

                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }
    }
}
