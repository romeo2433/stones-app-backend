using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stones.Data;
using Stones.Models;
using Microsoft.Extensions.Logging;

namespace Stones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendrePierreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VendrePierreController> _logger;

        public VendrePierreController(ApplicationDbContext context, ILogger<VendrePierreController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/VendrePierre/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPierre(int id)
        {
            var pierre = await _context.Pierres
                .Include(p => p.Qualite)
                .Include(p => p.Forme)
                .Include(p => p.Carat)
                .FirstOrDefaultAsync(p => p.Id_pierre == id);

            if (pierre == null)
                return NotFound(new { message = "Pierre introuvable" });

            var qteEntree = await _context.MouvementStockDetail
                .Where(m => m.Id_pierre == id && m.MouvementStock.Type_mouvement == "Entrée")
                .SumAsync(m => m.Quantite);

            var qteSortie = await _context.MouvementStockDetail
                .Where(m => m.Id_pierre == id && m.MouvementStock.Type_mouvement == "Sortie")
                .SumAsync(m => m.Quantite);

            var dispo = qteEntree - qteSortie;

            return Ok(new
            {
                pierre.Id_pierre,
                pierre.Nom_pierre,
                Qualite = pierre.Qualite?.Nom_qualite,
                Carat = pierre.Carat?.valeur,
                Forme = pierre.Forme?.nom_forme,
                pierre.Prix_vente,
                QuantiteDisponible = dispo
            });
        }

        // POST api/VendrePierre
        [HttpPost]
        public async Task<IActionResult> Vendre([FromBody] VendrePierreViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var qteEntree = await _context.MouvementStockDetail
                .Where(m => m.Id_pierre == model.Id_pierre && m.MouvementStock.Type_mouvement == "Entrée")
                .SumAsync(m => m.Quantite);

            var qteSortie = await _context.MouvementStockDetail
                .Where(m => m.Id_pierre == model.Id_pierre && m.MouvementStock.Type_mouvement == "Sortie")
                .SumAsync(m => m.Quantite);

            var dispo = qteEntree - qteSortie;

            if (model.QuantiteAVendre > dispo)
                return BadRequest(new { message = "Quantité demandée non disponible en stock", QuantiteDisponible = dispo });

            try
            {
                var mouvement = new MouvementStock
                {
                    Date_mouvement = DateTime.UtcNow,
                    Type_mouvement = "Sortie",
                    Total = model.Total,
                    Id_utilisateur = 1,
                    Id_pavillon = 1
                };
                _context.MouvementStock.Add(mouvement);
                await _context.SaveChangesAsync();

                var detail = new MouvementStockDetail
                {
                    Id_pierre = model.Id_pierre,
                    Quantite = model.QuantiteAVendre,
                    Prix_unitaire = model.Prix_vente,
                    Id_mvt = mouvement.Id_mvt
                };
                _context.MouvementStockDetail.Add(detail);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Vente enregistrée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enregistrement de la vente");
                return StatusCode(500, new { message = "Erreur serveur lors de l'enregistrement de la vente" });
            }
        }
    }
}
