using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDiary.Models
{
    // Klasa reprezentująca dzienne podsumowanie spożycia dla użytkownika
    public class DailySummary
    {
        // Klucz główny podsumowania dziennego
        [Key]
        public int DailySummaryId { get; set; }

        // Data, której dotyczy podsumowanie (wymagana)
        [Required(ErrorMessage = "Data podsumowania jest wymagana.")]
        [DataType(DataType.Date)] 
        public DateTime Date { get; set; } = DateTime.Today;

        // Suma spożytych kalorii w danym dniu
        public double TotalCalories { get; set; }
        // Suma spożytego białka w danym dniu
        public double TotalProtein { get; set; }
        // Suma spożytych tłuszczów w danym dniu
        public double TotalFat { get; set; }
        // Suma spożytych węglowodanów w danym dniu
        public double TotalCarbohydrates { get; set; }

        // Identyfikator użytkownika powiązanego z podsumowaniem
        public int UserId { get; set; }
        // Nawigacja do użytkownika (relacja wiele podsumowań do jednego użytkownika)
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Lista posiłków powiązanych z tym podsumowaniem
        public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();
    }
}