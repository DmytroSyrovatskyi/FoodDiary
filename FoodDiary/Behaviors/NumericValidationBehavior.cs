using System.Linq;
using Microsoft.Maui.Controls;

namespace FoodDiary.Behaviors
{
    // Zachowanie walidujące, czy wpisany tekst w polu Entry jest liczbą (cyfry i jeden przecinek)
    public class NumericValidationBehavior : Behavior<Entry>
    {
        // Metoda wywoływana po dołączeniu zachowania do kontrolki Entry
        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            // Subskrypcja zdarzenia zmiany tekstu
            entry.TextChanged += OnEntryTextChanged;
        }

        // Metoda wywoływana po odłączeniu zachowania od kontrolki Entry
        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);
            // Odsubskrybowanie zdarzenia zmiany tekstu
            entry.TextChanged -= OnEntryTextChanged;
        }

        // Obsługa zdarzenia zmiany tekstu w polu Entry
        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // Jeśli nowy tekst jest pusty lub zawiera tylko białe znaki, nie wykonuj walidacji
            if (string.IsNullOrWhiteSpace(args.NewTextValue))
                return;

            // Sprawdzenie, czy wszystkie znaki to cyfry lub przecinek
            bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x) || x == ',');

            // Sprawdzenie, czy nie ma więcej niż jednego przecinka
            if (args.NewTextValue.Count(x => x == ',') > 1)
            {
                isValid = false;
            }

            // Jeśli tekst jest poprawny, pozostaje bez zmian; w przeciwnym razie przywracany jest poprzedni tekst
            ((Entry)sender).Text = isValid ? args.NewTextValue : args.OldTextValue;
        }
    }
}
