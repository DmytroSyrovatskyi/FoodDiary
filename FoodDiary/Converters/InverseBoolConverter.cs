using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FoodDiary.Converters 
{
    // Konwerter odwracający wartość logiczną (bool) na jej przeciwną
    public class InverseBoolConverter : IValueConverter
    {
        // Metoda konwertująca wartość bool na przeciwną (true -> false, false -> true)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            // Jeśli wartość nie jest typu bool, zwraca false
            return false;
        }

        // Metoda konwertująca z powrotem wartość bool na przeciwną (używana przy dwukierunkowym wiązaniu)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            // Jeśli wartość nie jest typu bool, zwraca false
            return false;
        }
    }
}