using FoodDiary.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FoodDiary.Services
{
    // Interfejs serwisu do obsługi operacji na produktach spożywczych
    public interface IFoodItemService
    {
        // Asynchronicznie pobiera listę wszystkich produktów spożywczych
        Task<List<FoodItem>> GetFoodItemsAsync();

        // Asynchronicznie wyszukuje produkty spożywcze po nazwie (ignoruje wielkość liter)
        Task<List<FoodItem>> SearchFoodItemsByNameAsync(string searchTerm);

        // Asynchronicznie dodaje nowy produkt spożywczy do bazy
        Task<bool> AddFoodItemAsync(FoodItem newItem);

        // Waliduje produkt spożywczy na podstawie atrybutów danych
        bool ValidateFoodItem(FoodItem item, out ICollection<ValidationResult> results);

        // Asynchronicznie usuwa produkt spożywczy, jeśli nie jest używany w żadnym posiłku
        Task<(bool Success, string ErrorMessage)> DeleteFoodItemAsync(int foodItemId);
    }
}
