using ArkuszOcen.Model;
using System.Globalization;
using System.Windows.Data;
namespace ArkuszOcen;
public class OcenyConverter : IValueConverter {
    public object? Convert(
    object value, Type targetType, object parameter, CultureInfo culture
    ) {
        if (value is not IEnumerable<Ocena> oceny) return null;
        return string.Join("; ", oceny.Select(o => $"{o.Przedmiot}: {o.Wartość:F1}"));
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string stringWartość) {
            var ocenyArray = stringWartość.Split(';');
            var ocenyList = new List<Ocena>();
            foreach (var ocenaString in ocenyArray) {
                var przedmiotWartoscArray = ocenaString.Trim().Split(':');
                if (float.TryParse(
                przedmiotWartoscArray[1].Trim(),
                out float wartoscOceny)
                ) {
                    ocenyList.Add(
                    new Ocena {
                        Przedmiot = przedmiotWartoscArray[0].Trim(),
                        Wartość = wartoscOceny
                    }
                    );
                }
            }
            return ocenyList;
        }
        return null!;
    }
}