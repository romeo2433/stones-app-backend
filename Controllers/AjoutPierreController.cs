using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stones.Data;
using Stones.Models;

namespace Stones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AjoutPierreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AjoutPierreController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("qualites")]
        public async Task<IActionResult> GetQualites()
        {
            var qualites = await _context.Qualite
                .Select(q => new { q.Id_qua, q.Nom_qualite })
                .ToListAsync();
            return Ok(qualites);
        }

        [HttpGet("formes")]
        public async Task<IActionResult> GetFormes()
        {
            var formes = await _context.Forme
                .Select(f => new { f.id_forme, f.nom_forme })
                .ToListAsync();
            return Ok(formes);
        }
        [HttpGet("couleurs")]
        public async Task<IActionResult> GetCouleurs()
        {
            var couleurs = await _context.Couleurs  
                .Select(c => new { c.Id_couleur, c.Nom_couleur })
                .ToListAsync();
            return Ok(couleurs);
        }
        [HttpPost("ajouter")]
        public async Task<IActionResult> Ajouter([FromBody] AjouterPierreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si la pierre existe déjà
            var pierreExistante = await _context.Pierres.FirstOrDefaultAsync(p =>
                p.Nom_pierre.ToLower() == model.NomPierre.ToLower() &&
                p.Id_qua == model.IdQualite &&
                p.Id_forme == model.IdForme &&
                _context.Carat_.Any(c => c.valeur == model.ValeurCarat));

            if (pierreExistante != null)
            {
                return Conflict(new { message = "Une pierre identique existe déjà." });
            }

            // Ajouter la pierre et le mouvement
            var carat = await _context.Carat_.FirstOrDefaultAsync(c => c.valeur == model.ValeurCarat);
            if (carat == null)
            {
                carat = new Carat_ { valeur = model.ValeurCarat };
                _context.Carat_.Add(carat);
                await _context.SaveChangesAsync();
            }

            var nouvellePierre = new Pierre
            {
                Nom_pierre = model.NomPierre,
                Id_qua = model.IdQualite,
                Id_forme = model.IdForme,
                Id_carat = carat.id_carat,
                Prix_vente = model.PrixVente,
                Id_couleur = model.IdCouleur 
            };

            _context.Pierres.Add(nouvellePierre);
            await _context.SaveChangesAsync();

            var mouvement = new MouvementStock
            {
                Date_mouvement = DateTime.UtcNow,
                Type_mouvement = "Entrée",
                Total = model.Quantite * model.PrixVente,
                Id_utilisateur = 1, // à adapter
                Id_pavillon = 1     // à adapter
            };

            _context.MouvementStock.Add(mouvement);
            await _context.SaveChangesAsync();

            var detail = new MouvementStockDetail
            {
                Id_pierre = nouvellePierre.Id_pierre,
                Quantite = model.Quantite,
                Prix_unitaire = model.PrixVente,
                Id_mvt = mouvement.Id_mvt
            };

            _context.MouvementStockDetail.Add(detail);
            await _context.SaveChangesAsync();

            // ✅ Retour JSON pour React
            return Ok(new { success = true, message = "Pierre ajoutée avec succès." });
        }

    }
}
