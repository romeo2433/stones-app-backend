using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stones.Data;
using Stones.Models;

namespace Stones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MouvementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MouvementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Mouvements/Sorties?startDate=2025-09-01&endDate=2025-09-15
        [HttpGet("Sorties")]
        public async Task<IActionResult> HistoriqueSorties([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var mouvementsQuery = _context.MouvementStock
                .Include(m => m.MouvementStockDetails)
                    .ThenInclude(d => d.Pierre)
                .Where(m => m.Type_mouvement.ToLower() == "sortie");

            if (startDate.HasValue)
                mouvementsQuery = mouvementsQuery.Where(m => m.Date_mouvement >= startDate.Value.ToUniversalTime());

            if (endDate.HasValue)
                mouvementsQuery = mouvementsQuery.Where(m => m.Date_mouvement <= endDate.Value.ToUniversalTime());

            var mouvements = await mouvementsQuery
                .OrderByDescending(m => m.Date_mouvement)
                .Select(m => new
                {
                    m.Id_mvt,
                    m.Date_mouvement,
                    m.Type_mouvement,
                    Détails = m.MouvementStockDetails.Select(d => new
                    {
                        d.Id_pierre,
                        d.Quantite,
                        d.Prix_unitaire,
                        Nom_pierre = d.Pierre.Nom_pierre,
                        Qualite = d.Pierre.Qualite.Nom_qualite, // ← nom au lieu de l’ID
                        Forme = d.Pierre.Forme.nom_forme,       // ← nom au lieu de l’ID
                        Carat = d.Pierre.Carat.valeur,           // ou selon ta table
                        Couleur = d.Pierre.Couleur.Nom_couleur
                    })
                })

                .ToListAsync();

            return Ok(new
            {
                StartDate = startDate?.ToString("yyyy-MM-dd"),
                EndDate = endDate?.ToString("yyyy-MM-dd"),
                Mouvements = mouvements
            });
        }
    }
}
