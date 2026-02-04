using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodDiary.Models
{
    // Klasa reprezentująca kategorię produktów spożywczych
    public class FoodCategory
    {
        // Klucz główny kategorii
        [Key]
        public int FoodCategoryId { get; set; }

        // Nazwa kategorii (pole wymagane, maksymalnie 100 znaków)
        [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
        [StringLength(100)]
        public string Name { get; set; }

        // Kolekcja produktów przypisanych do tej kategorii
        public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
