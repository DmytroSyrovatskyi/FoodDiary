using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace FoodDiary.Core.Models
{
    // Typ wyliczeniowy określający rodzaj posiłku
    public enum MealType
    {
        Breakfast, // Śniadanie
        Lunch,     // Obiad
        Dinner,    // Kolacja
        Snack,     // Przekąska
        Other      // Inny
    }

    // Klasa reprezentująca pojedynczy posiłek użytkownika
    public class Meal
    {
        // Klucz główny posiłku
        [Key]
        public int MealId { get; set; }

        // Typ posiłku (np. śniadanie, obiad) - pole wymagane
        [Required(ErrorMessage = "Typ posiłku jest wymagany.")]
        public MealType Type { get; set; }

        // Data i godzina spożycia posiłku - pole wymagane
        [Required(ErrorMessage = "Data i godzina posiłku są wymagane.")]
        public DateTime MealTime { get; set; } = DateTime.Now;

        // Identyfikator powiązanego dziennego podsumowania
        public int DailySummaryId { get; set; }
        // Nawigacja do dziennego podsumowania (relacja wiele posiłków do jednego podsumowania)
        [ForeignKey("DailySummaryId")]
        public virtual DailySummary DailySummary { get; set; }

        // Kolekcja wpisów posiłku (produkty wchodzące w skład posiłku)
        public virtual ICollection<MealEntry> MealEntries { get; set; } = new List<MealEntry>();
    }
}