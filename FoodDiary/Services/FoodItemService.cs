using FoodDiary.Data;
using FoodDiary.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Services
{
    // Serwis odpowiedzialny za operacje na produktach spożywczych
    public class FoodItemService : IFoodItemService
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        // Konstruktor przyjmujący fabrykę kontekstu
        public FoodItemService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Pobiera listę wszystkich produktów spożywczych z bazy, posortowaną alfabetycznie
        public async Task<List<FoodItem>> GetFoodItemsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.FoodItems
                .Include(fi => fi.Category)
                .OrderBy(fi => fi.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        // Wyszukuje produkty spożywcze po nazwie (ignorując wielkość liter)
        public async Task<List<FoodItem>> SearchFoodItemsByNameAsync(string searchTerm)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetFoodItemsAsync();
            }

            return await context.FoodItems
                .Include(fi => fi.Category)
                .Where(fi => fi.Name.ToLower().Contains(searchTerm.ToLower()))
                .OrderBy(fi => fi.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        // Dodaje nowy produkt spożywczy do bazy, jeśli przeszedł walidację
        public async Task<bool> AddFoodItemAsync(FoodItem newItem)
        {
            if (!ValidateFoodItem(newItem, out _))
            {
                return false;
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            context.FoodItems.Add(newItem);
            await context.SaveChangesAsync();
            return true;
        }

        // Waliduje produkt spożywczy na podstawie atrybutów danych
        public bool ValidateFoodItem(FoodItem item, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var validationContext = new ValidationContext(item);
            return Validator.TryValidateObject(item, validationContext, results, true);
        }

        // Usuwa produkt spożywczy, jeśli nie jest używany w żadnym posiłku
        public async Task<(bool Success, string ErrorMessage)> DeleteFoodItemAsync(int foodItemId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // Sprawdzenie, czy produkt jest używany w jakimkolwiek wpisie posiłku
            bool isUsed = await context.MealEntries.AnyAsync(e => e.FoodItemId == foodItemId);
            if (isUsed)
            {
                return (false, "Nie można usunąć produktu, ponieważ jest używany w zapisanych posiłkach.");
            }

            // Wyszukanie produktu w bazie
            var itemInDb = await context.FoodItems.FindAsync(foodItemId);
            if (itemInDb != null)
            {
                context.FoodItems.Remove(itemInDb);
                await context.SaveChangesAsync();
                return (true, null);
            }

            // Produkt nie został znaleziony
            return (false, "Nie znaleziono produktu do usunięcia.");
        }
    }
}
