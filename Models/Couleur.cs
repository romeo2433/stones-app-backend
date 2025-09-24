using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stones.Models
{
    [Table("couleurs")]
    public class Couleur
    {
        [Key]
        [Column("id_couleur")]
        public int Id_couleur { get; set; }

        [Required]
        [Column("nom_couleur")]
        public string Nom_couleur { get; set; }

        // Relation inverse (une couleur peut avoir plusieurs pierres)
        public ICollection<Pierre> Pierres { get; set; }
    }
}
