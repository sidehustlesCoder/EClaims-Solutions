using eClaims.Core.Entities;
using eClaims.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eClaims.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IRepository<Claim> _claimRepository;

        public ClaimsController(IRepository<Claim> claimRepository)
        {
            _claimRepository = claimRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claims = await _claimRepository.GetAllAsync();
            return Ok(claims);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var claim = await _claimRepository.GetByIdAsync(id);
            if (claim == null) return NotFound();
            return Ok(claim);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Claim claim)
        {
            var created = await _claimRepository.AddAsync(claim);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Claim claim)
        {
            if (id != claim.Id) return BadRequest();
            await _claimRepository.UpdateAsync(claim);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _claimRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
