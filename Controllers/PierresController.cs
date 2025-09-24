using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Stones.Data;
using Stones.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stones.Models.ViewModels;


namespace Stones.Controllers
{
     [Route("api/[controller]")]
    [ApiController]
    public class PierresController : ControllerBase  
    {
        private readonly ApplicationDbContext _context;

        public PierresController(ApplicationDbContext context)
        {
            _context = context;
        }

       
       [HttpPost("stock")]
        public async Task<IActionResult> Stock([FromBody] SearchCriteria search)
        {
            // Requête de base
            var baseQuery = @"
            SELECT 
                p.id_pierre AS Id_pierre,
                p.nom_pierre AS Nom_pierre,
                p.prix_vente AS Prix_vente,
                q.nom_qualite AS Qualite,
                c.valeur AS Carat,
                f.nom_forme AS Forme,
                coul.nom_couleur AS Couleur,
                COALESCE(SUM(CASE WHEN ms.type_mouvement = 'Entrée' THEN msd.quantite ELSE 0 END), 0)
                - COALESCE(SUM(CASE WHEN ms.type_mouvement = 'Sortie' THEN msd.quantite ELSE 0 END), 0)
                AS Quantite_totale
            FROM pierres p
            JOIN qualite q ON p.id_qua = q.id_qua
            JOIN carat_ c ON p.id_carat = c.id_carat
            JOIN forme f ON p.id_forme = f.id_forme
            JOIN couleurs coul ON p.id_couleur = coul.id_couleur
            LEFT JOIN mouvement_stock_detail msd ON p.id_pierre = msd.id_pierre
            LEFT JOIN mouvement_stock ms ON ms.id_mvt = msd.id_mvt
        ";

            var whereClauses = new List<string>();
            var havingClauses = new List<string>();
            var parameters = new List<NpgsqlParameter>();
            var paramIndex = 0;

            // Filtres WHERE
            if (!string.IsNullOrEmpty(search.NomPierre))
            {
                whereClauses.Add($"p.nom_pierre ILIKE @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", $"%{search.NomPierre}%"));
                paramIndex++;
            }

            if (!string.IsNullOrEmpty(search.Qualite))
            {
                whereClauses.Add($"q.nom_qualite = @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.Qualite));
                paramIndex++;
            }

            if (search.CaratMin.HasValue)
            {
                whereClauses.Add($"c.valeur >= @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.CaratMin.Value));
                paramIndex++;
            }

            if (search.CaratMax.HasValue)
            {
                whereClauses.Add($"c.valeur <= @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.CaratMax.Value));
                paramIndex++;
            }

            if (!string.IsNullOrEmpty(search.Forme))
            {
                whereClauses.Add($"f.nom_forme = @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.Forme));
                paramIndex++;
            }

            // Filtres HAVING
            if (search.QuantiteMin.HasValue)
            {
                havingClauses.Add($"COALESCE(SUM(msd.quantite), 0) >= @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.QuantiteMin.Value));
                paramIndex++;
            }

            if (search.QuantiteMax.HasValue)
            {
                havingClauses.Add($"COALESCE(SUM(msd.quantite), 0) <= @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex}", search.QuantiteMax.Value));
                paramIndex++;
            }

            // Requête finale
            if (whereClauses.Any())
            {
                baseQuery += "WHERE " + string.Join(" AND ", whereClauses) + "\n";
            }

            baseQuery += "GROUP BY p.id_pierre, p.nom_pierre, p.prix_vente, q.nom_qualite, c.valeur, f.nom_forme, coul.nom_couleur\n";

            if (havingClauses.Any())
            {
                baseQuery += "HAVING " + string.Join(" AND ", havingClauses) + "\n";
            }

            baseQuery += "ORDER BY p.nom_pierre ASC";

            try
            {
                var query = _context.PierreStockViewModels.FromSqlRaw(baseQuery, parameters.ToArray());
                var pierresStock = await query.AsNoTracking().ToListAsync();

                // ✅ Retour JSON
                return Ok(pierresStock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Récupérer la pierre
            var pierre = await _context.Pierres.FirstOrDefaultAsync(p => p.Id_pierre == id);
            if (pierre == null)
                return NotFound(new { message = "Pierre introuvable." });

            // Récupérer les mouvements stock liés à cette pierre via les détails
            var mouvementsIds = await _context.MouvementStockDetail
                .Where(m => m.Id_pierre == id)
                .Select(m => m.Id_mvt)
                .Distinct()
                .ToListAsync();

            // Récupérer les mouvements parent correspondants
            var mouvements = await _context.MouvementStock
                .Where(mvt => mouvementsIds.Contains(mvt.Id_mvt))
                .ToListAsync();

            // Supprimer tous les détails liés à la pierre
            var mouvementsDetails = _context.MouvementStockDetail.Where(m => m.Id_pierre == id);
            _context.MouvementStockDetail.RemoveRange(mouvementsDetails);

            // Supprimer les mouvements récupérés
            _context.MouvementStock.RemoveRange(mouvements);

            // Supprimer la pierre
            _context.Pierres.Remove(pierre);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pierre et ses mouvements supprimés avec succès." });
        }

  // GET api/Pierres/{id}
[HttpGet("{id}")]
public async Task<IActionResult> GetPierre(int id)
{
    var pierre = await _context.Pierres.FindAsync(id);
    if (pierre == null)
        return NotFound(new { message = "Pierre introuvable" });

    var quantite = await _context.MouvementStockDetail
        .Where(msd => msd.Id_pierre == id)
        .Join(_context.MouvementStock,
            msd => msd.Id_mvt,
            ms => ms.Id_mvt,
            (msd, ms) => new { msd, ms })
        .SumAsync(x => x.ms.Type_mouvement == "Entrée" ? x.msd.Quantite : -x.msd.Quantite);

    return Ok(new {
        Id_pierre = pierre.Id_pierre,
        Nom_pierre = pierre.Nom_pierre,
        Prix_vente = pierre.Prix_vente,
        Quantite = quantite
    });
}

// PUT api/Pierres/{id}
[HttpPut("{id}")]
public async Task<IActionResult> UpdatePierre(int id, [FromBody] ModifierPierreViewModel model)
{
    if (id != model.Id_pierre)
        return BadRequest(new { message = "ID incohérent" });

    var pierre = await _context.Pierres.FindAsync(id);
    if (pierre == null)
        return NotFound(new { message = "Pierre introuvable" });

    // Mise à jour du nom et prix
    pierre.Nom_pierre = model.Nom_pierre;
    pierre.Prix_vente = model.Prix_vente;

    // 🔹 Récupère la quantité actuelle via les mouvements
    var quantiteActuelle = await _context.MouvementStockDetail
        .Where(d => d.Id_pierre == id)
        .Join(_context.MouvementStock,
            d => d.Id_mvt,
            m => m.Id_mvt,
            (d, m) => new { d.Quantite, m.Type_mouvement })
        .SumAsync(x => x.Type_mouvement == "Entrée" ? x.Quantite : -x.Quantite);

    // 🔹 Calcul de la différence pour ajuster
    var difference = model.Quantite - quantiteActuelle;

    if (difference != 0)
    {
        // Crée un mouvement d'ajustement
        var mouvement = new MouvementStock
        {
            Date_mouvement = DateTime.UtcNow,
            Type_mouvement = difference > 0 ? "Entrée" : "Sortie",
            Total = Math.Abs(difference) * model.Prix_vente,
            Id_utilisateur = 1,
            Id_pavillon = 1
        };

        _context.MouvementStock.Add(mouvement);
        await _context.SaveChangesAsync();

        var detail = new MouvementStockDetail
        {
            Id_mvt = mouvement.Id_mvt,
            Id_pierre = id,
            Quantite = Math.Abs(difference),
            Prix_unitaire = model.Prix_vente
        };

        _context.MouvementStockDetail.Add(detail);
        await _context.SaveChangesAsync();
    }

    // Sauvegarde du nom et prix
    await _context.SaveChangesAsync();

    return Ok(new { message = "Pierre modifiée avec succès" });
}
    







    }
}