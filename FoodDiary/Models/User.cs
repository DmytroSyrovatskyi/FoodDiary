using System.ComponentModel.DataAnnotations; 
using System.Collections.Generic; 

namespace FoodDiary.Models
{
    // Klasa reprezentująca użytkownika aplikacji
    public class User
    {
        // Klucz główny użytkownika
        [Key] 
        public int UserId { get; set; }

        // Nazwa użytkownika (wymagana, maksymalnie 100 znaków)
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa użytkownika nie może być dłuższa niż 100 znaków.")]
        public string Username { get; set; }

        // Data utworzenia konta użytkownika
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Kolekcja dziennych podsumowań powiązanych z użytkownikiem
        public virtual ICollection<DailySummary> DailySummaries { get; set; } = new List<DailySummary>();
    }
}