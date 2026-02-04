using FoodDiary.Core.Data;
using FoodDiary.Core.Models;
using FoodDiary.Core.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FoodDiary.Tests
{
    // Testy jednostkowe dla serwisu FoodItemService
    public class FoodItemServiceTests
    {
        // Mock fabryki kontekstu bazy danych
        private readonly Mock<IDbContextFactory<AppDbContext>> _mockFactory;
        // Mock kontekstu bazy danych
        private readonly Mock<AppDbContext> _mockContext;
        // Lista przykładowych produktów spożywczych
        private readonly List<FoodItem> _foodItems;
        // Lista przykładowych wpisów posiłków
        private readonly List<MealEntry> _mealEntries;

        // Konstruktor inicjalizujący dane testowe i mocki
        public FoodItemServiceTests()
        {
            // Przykładowe produkty spożywcze
            _foodItems = new List<FoodItem>
            {
                new FoodItem { FoodItemId = 1, Name = "Jabłko", Calories = 52 },
                new FoodItem { FoodItemId = 2, Name = "Banan", Calories = 89 },
                new FoodItem { FoodItemId = 3, Name = "Kurczak", Calories = 165 }
            };

            // Przykładowe wpisy posiłków (jeden wpis z produktem o ID 3)
            _mealEntries = new List<MealEntry>
            {
                new MealEntry { MealEntryId = 1, FoodItemId = 3 }
            };

            // Konfiguracja mocka kontekstu bazy danych
            _mockContext = new Mock<AppDbContext>();
            _mockContext.Setup(c => c.FoodItems).ReturnsDbSet(_foodItems);
            _mockContext.Setup(c => c.MealEntries).ReturnsDbSet(_mealEntries);

            // Mockowanie metody zapisu zmian
            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1) 
                .Verifiable();

            // Konfiguracja mocka fabryki kontekstu
            _mockFactory = new Mock<IDbContextFactory<AppDbContext>>();
            _mockFactory.Setup(f => f.CreateDbContextAsync(default)).ReturnsAsync(_mockContext.Object);
        }

        // Test: Usuwanie produktu, który nie jest używany w żadnym posiłku
        [Fact]
        public async Task DeleteFoodItemAsync_WithUnusedItem_ShouldSucceed()
        {
            var service = new FoodItemService(_mockFactory.Object);
            int itemIdToDelete = 1; // ID produktu do usunięcia

            // Mockowanie wyszukiwania produktu po ID
            _mockContext.Setup(c => c.FoodItems.FindAsync(itemIdToDelete))
                .ReturnsAsync(_foodItems.First(i => i.FoodItemId == itemIdToDelete));

            var (success, errorMessage) = await service.DeleteFoodItemAsync(itemIdToDelete);

            Assert.True(success); // Operacja powinna się powieść
            Assert.Null(errorMessage);
            _mockContext.Verify(c => c.FoodItems.Remove(It.IsAny<FoodItem>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Test: Pobieranie wszystkich produktów, posortowanych po nazwie
        [Fact]
        public async Task GetFoodItemsAsync_ShouldReturnAllItems_SortedByName()
        {
            var service = new FoodItemService(_mockFactory.Object);
            var result = await service.GetFoodItemsAsync();
            Assert.Equal(3, result.Count);
            Assert.Equal("Banan", result[0].Name);
        }

        // Test: Dodawanie poprawnego produktu spożywczego
        [Fact]
        public async Task AddFoodItemAsync_WithValidItem_ShouldReturnTrueAndSaveChanges()
        {
            var service = new FoodItemService(_mockFactory.Object);
            var newItem = new FoodItem { Name = "Nowy Produkt", Calories = 100 };
            var result = await service.AddFoodItemAsync(newItem);
            Assert.True(result);
            _mockContext.Verify(c => c.FoodItems.Add(newItem), Times.Once());
        }

        // Test: Próba dodania niepoprawnego produktu (brak nazwy)
        [Fact]
        public async Task AddFoodItemAsync_WithInvalidItem_ShouldReturnFalseAndNotSave()
        {
            var service = new FoodItemService(_mockFactory.Object);
            var invalidItem = new FoodItem { Calories = 100 };
            var result = await service.AddFoodItemAsync(invalidItem);
            Assert.False(result);
        }

        // Test: Walidacja poprawnego produktu
        [Fact]
        public void ValidateFoodItem_WithValidItem_ShouldReturnTrueAndNoErrors()
        {
            var service = new FoodItemService(_mockFactory.Object);
            var validItem = new FoodItem { Name = "Poprawny Produkt", Calories = 50 };
            var isValid = service.ValidateFoodItem(validItem, out var results);
            Assert.True(isValid);
            Assert.Empty(results);
        }

        // Test: Walidacja niepoprawnego produktu (brak nazwy)
        [Fact]
        public void ValidateFoodItem_WithInvalidItem_ShouldReturnFalseWithErrors()
        {
            var service = new FoodItemService(_mockFactory.Object);
            var invalidItem = new FoodItem { Calories = 50 };
            var isValid = service.ValidateFoodItem(invalidItem, out var results);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        // Test: Próba usunięcia produktu, który jest używany w posiłku
        [Fact]
        public async Task DeleteFoodItemAsync_WithUsedItem_ShouldFail()
        {
            var service = new FoodItemService(_mockFactory.Object);
            int itemIdToDelete = 3;
            var (success, errorMessage) = await service.DeleteFoodItemAsync(itemIdToDelete);
            Assert.False(success);
            Assert.NotNull(errorMessage);
        }
    }
}
    