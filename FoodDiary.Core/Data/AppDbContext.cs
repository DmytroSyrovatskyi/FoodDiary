using Microsoft.EntityFrameworkCore;
using FoodDiary.Core.Models; 
using System.Collections.Generic;
using System;

namespace FoodDiary.Core.Data 
{
    // Klasa kontekstu bazy danych dla aplikacji FoodDiary
    public class AppDbContext : DbContext
    {
        // Tabela użytkowników
        public virtual DbSet<User> Users { get; set; }
        // Tabela produktów spożywczych
        public virtual DbSet<FoodItem> FoodItems { get; set; }
        // Tabela posiłków
        public virtual DbSet<Meal> Meals { get; set; }
        // Tabela wpisów posiłków (pojedyncze produkty w posiłkach)
        public virtual DbSet<MealEntry> MealEntries { get; set; }
        // Tabela dziennych podsumowań
        public virtual DbSet<DailySummary> DailySummaries { get; set; }
        // Tabela kategorii produktów spożywczych
        public virtual DbSet<FoodCategory> FoodCategories { get; set; }

        // Konstruktor domyślny (potrzebny np. do migracji)
        public AppDbContext() { }

        // Konstruktor przyjmujący opcje kontekstu (np. połączenie z bazą)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Konfiguracja modelu bazy danych i relacji
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacja: Meal -> MealEntries (kaskadowe usuwanie wpisów przy usunięciu posiłku)
            modelBuilder.Entity<Meal>()
                .HasMany(m => m.MealEntries)
                .WithOne(me => me.Meal)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja: FoodItem -> FoodCategory (kategoria opcjonalna)
            modelBuilder.Entity<FoodItem>()
                .HasOne(fi => fi.Category)
                .WithMany(c => c.FoodItems)
                .HasForeignKey(fi => fi.FoodCategoryId)
                .IsRequired(false);

            // Dodanie danych początkowych do bazy
            SeedData(modelBuilder);
        }

        // Metoda do wypełniania bazy przykładowymi danymi startowymi
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Przykładowi użytkownicy
            var user1 = new User { UserId = 1, Username = "TestUser1", DateCreated = DateTime.UtcNow.AddDays(-5) };
            var user2 = new User { UserId = 2, Username = "JanKowalski", DateCreated = DateTime.UtcNow.AddDays(-2) };
            modelBuilder.Entity<User>().HasData(user1, user2);

            // Przykładowe kategorie produktów
            var categories = new List<FoodCategory>
            {
                new FoodCategory { FoodCategoryId = 1, Name = "Owoce" },
                new FoodCategory { FoodCategoryId = 2, Name = "Mięso i drób" },
                new FoodCategory { FoodCategoryId = 3, Name = "Pieczywo" },
                new FoodCategory { FoodCategoryId = 4, Name = "Nabiał i jaja" },
                new FoodCategory { FoodCategoryId = 5, Name = "Warzywa" },
                new FoodCategory { FoodCategoryId = 6, Name = "Tłuszcze" }
            };
            modelBuilder.Entity<FoodCategory>().HasData(categories);

            // Przykładowe produkty spożywcze
            var foodItems = new List<FoodItem>
            {
                new FoodItem { FoodItemId = 1, FoodCategoryId = 1, Name = "Jabłko", Calories = 52, Protein = 0.3, Fat = 0.2, Carbohydrates = 14, ServingUnit = "g" },
                new FoodItem { FoodItemId = 2, FoodCategoryId = 2, Name = "Pierś z kurczaka", Calories = 165, Protein = 31, Fat = 3.6, Carbohydrates = 0, ServingUnit = "g" },
                new FoodItem { FoodItemId = 3, FoodCategoryId = 3, Name = "Chleb żytni", Calories = 259, Protein = 8.5, Fat = 3.3, Carbohydrates = 48, ServingUnit = "kromka" },
                new FoodItem { FoodItemId = 4, FoodCategoryId = 4, Name = "Jajko", Calories = 78, Protein = 6, Fat = 5, Carbohydrates = 0.6, ServingUnit = "szt." },
                new FoodItem { FoodItemId = 5, FoodCategoryId = 4, Name = "Mleko 2%", Calories = 50, Protein = 3.3, Fat = 2, Carbohydrates = 4.8, ServingUnit = "ml" },
                new FoodItem { FoodItemId = 6, FoodCategoryId = 5, Name = "Marchew", Calories = 41, Protein = 0.9, Fat = 0.2, Carbohydrates = 10, ServingUnit = "g" },
                new FoodItem { FoodItemId = 7, FoodCategoryId = 6, Name = "Oliwa z oliwek", Calories = 884, Protein = 0, Fat = 100, Carbohydrates = 0, ServingUnit = "ml" }
            };
            modelBuilder.Entity<FoodItem>().HasData(foodItems);

            // Przykładowe dzienne podsumowania dla użytkowników
            var summaryUser1 = new DailySummary { DailySummaryId = 1, UserId = 1, Date = DateTime.Today };
            var summaryUser2 = new DailySummary { DailySummaryId = 2, UserId = 2, Date = DateTime.Today };
            modelBuilder.Entity<DailySummary>().HasData(summaryUser1, summaryUser2);

            // Przykładowe posiłki dla użytkowników
            var breakfastUser1 = new Meal { MealId = 1, DailySummaryId = 1, Type = MealType.Breakfast, MealTime = DateTime.Today.AddHours(8) };
            var lunchUser1 = new Meal { MealId = 2, DailySummaryId = 1, Type = MealType.Lunch, MealTime = DateTime.Today.AddHours(13) };
            var breakfastUser2 = new Meal { MealId = 3, DailySummaryId = 2, Type = MealType.Breakfast, MealTime = DateTime.Today.AddHours(9) };
            modelBuilder.Entity<Meal>().HasData(breakfastUser1, lunchUser1, breakfastUser2);

            // Przykładowe wpisy posiłków (produkty w posiłkach)
            var mealEntries = new List<MealEntry>
            {
                new MealEntry { MealEntryId = 1, MealId = 1, FoodItemId = 4, Quantity = 2 },
                new MealEntry { MealEntryId = 2, MealId = 1, FoodItemId = 3, Quantity = 1 },
                new MealEntry { MealEntryId = 3, MealId = 2, FoodItemId = 2, Quantity = 150 },
                new MealEntry { MealEntryId = 4, MealId = 2, FoodItemId = 1, Quantity = 100 },
                new MealEntry { MealEntryId = 5, MealId = 3, FoodItemId = 5, Quantity = 200 }
            };
            modelBuilder.Entity<MealEntry>().HasData(mealEntries);
        }
    }
}
