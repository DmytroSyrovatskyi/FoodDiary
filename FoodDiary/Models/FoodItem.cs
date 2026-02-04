using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDiary.Models
{
    // Klasa reprezentująca pojedynczy produkt spożywczy
    public class FoodItem
    {
        // Klucz główny produktu
        [Key]
        public int FoodItemId { get; set; }

        // Nazwa produktu (wymagana, maksymalnie 200 znaków)
        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        [StringLength(200, ErrorMessage = "Nazwa produktu nie może być dłuższa niż 200 znaków.")]
        public string Name { get; set; }

        // Liczba kalorii w produkcie (0-5000)
        [Range(0, 5000, ErrorMessage = "Kalorie muszą być wartością nieujemną (max 5000).")]
        public double Calories { get; set; }

        // Ilość białka w produkcie (0-100)
        [Range(0, 100, ErrorMessage = "Białko musi być wartością nieujemną (max 100).")]
        public double Protein { get; set; }

        // Ilość tłuszczu w produkcie (0-100)
        [Range(0, 100, ErrorMessage = "Tłuszcz musi być wartością nieujemną (max 100).")]
        public double Fat { get; set; }

        // Ilość węglowodanów w produkcie (0-100)
        [Range(0, 100, ErrorMessage = "Węglowodany muszą być wartością nieujemną (max 100).")]
        public double Carbohydrates { get; set; }

        // Jednostka porcji (np. g, ml, szt.)
        [StringLength(50)]
        public string ServingUnit { get; set; } = "g";

        // Identyfikator kategorii produktu (opcjonalny)
        public int? FoodCategoryId { get; set; }

        // Nawigacja do kategorii produktu
        [ForeignKey("FoodCategoryId")]
        public virtual FoodCategory Category { get; set; }

        // Kolekcja wpisów posiłków, w których użyto ten produkt
        public virtual ICollection<MealEntry> MealEntries { get; set; } = new List<MealEntry>();

        // Podsumowanie wartości odżywczych w formie tekstowej (nie jest mapowane do bazy)
        [NotMapped]
        public string NutritionSummary
        {
            get
            {
                string perUnitText = ServingUnit == "szt." ? "na 1 szt." : $"na 100 {ServingUnit}";
                return $"Kcal: {Calories}, B: {Protein}g, T: {Fat}g, W: {Carbohydrates}g ({perUnitText})";
            }
        }
    }
}
