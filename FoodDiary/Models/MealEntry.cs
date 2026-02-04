using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDiary.Models
{
    // Klasa reprezentująca pojedynczy wpis posiłku (konkretna ilość produktu w danym posiłku)
    public class MealEntry
    {
        // Klucz główny wpisu posiłku
        [Key]
        public int MealEntryId { get; set; }

        // Ilość produktu w posiłku (wymagana, wartość dodatnia do 10000)
        [Required(ErrorMessage = "Ilość jest wymagana.")]
        [Range(0.1, 10000, ErrorMessage = "Ilość musi być wartością dodatnią (max 10000).")]
        public double Quantity { get; set; } 

        // Identyfikator produktu spożywczego
        public int FoodItemId { get; set; }
        // Nawigacja do produktu spożywczego
        [ForeignKey("FoodItemId")]
        public virtual FoodItem FoodItem { get; set; }

        // Identyfikator posiłku
        public int MealId { get; set; }
        // Nawigacja do posiłku
        [ForeignKey("MealId")]
        public virtual Meal Meal { get; set; }
    }
}