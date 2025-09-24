using System.ComponentModel.DataAnnotations;

namespace Stones.Models.ViewModels
{
    public class ModifierPierreViewModel
    {
        public int Id_pierre { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name = "Nom de la pierre")]
        public string Nom_pierre { get; set; }

        [Required(ErrorMessage = "Le prix est requis")]
        [Range(0, 9999999999.99)]
        [Display(Name = "Prix de vente")]
        public decimal Prix_vente { get; set; }

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(0, 9999999999.99)]
        [Display(Name = "Quantité")]
        public decimal Quantite { get; set; }
    }
}
