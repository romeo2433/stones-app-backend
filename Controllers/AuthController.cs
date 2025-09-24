using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stones.Data;
using Stones.Models;
using System.Threading.Tasks;

namespace Stones.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/Inscription
        [HttpPost("Inscription")]
        public async Task<IActionResult> Inscription([FromBody] Utilisateur utilisateur)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vérifier si le numéro existe déjà
            bool numeroExists = await _context.Utilisateur.AnyAsync(u => u.Numero == utilisateur.Numero);
            if (numeroExists)
                return Conflict(new { message = "Ce numéro est déjà utilisé" });

            _context.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Inscription réussie", utilisateur });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var utilisateur = await _context.Utilisateur
                .FirstOrDefaultAsync(u => u.Numero == model.Numero);

            if (utilisateur == null)
                return Unauthorized(new { message = "Numéro incorrect" });

            return Ok(new
            {
                message = "Connexion réussie",
                utilisateur = new { utilisateur.IdUtilisateur, utilisateur.NomComplet, utilisateur.Numero }
            });
        }
    }
}
