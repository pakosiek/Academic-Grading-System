using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using ArkuszOcen.Model;
namespace ArkuszOcen;
public partial class OcenaWindow : Window {
    public Ocena Ocena { get; set; }
    public OcenaWindow(Ocena ocena) {
        InitializeComponent();
        Ocena = ocena;
        tbPrzedmiot.Text = Ocena.Przedmiot;
        tbWartość.Text = string.Format($"{Ocena.Wartość:F1}");
    }
    private void Zatwierdź_Click(object sender, RoutedEventArgs e) {
        if (
        !Regex.IsMatch(tbPrzedmiot.Text, @"^[\p{Lu}|\p{Ll}]{1,12}$") ||
        !Regex.IsMatch(tbWartość.Text, @"^[2-5][.,][0,5]$")
        ) {
            MessageBox.Show("Wprowadzone dane są niepoprawne.");
            return;
        }
        Ocena.Przedmiot = tbPrzedmiot.Text;
        if (!double.TryParse(
        tbWartość.Text, CultureInfo.CurrentCulture, out double wartość
        )) {
            string kropkaCzyPrzecinek =
            CultureInfo.CurrentCulture.
            NumberFormat.CurrencyDecimalSeparator;
            MessageBox.Show($"Użyj separatora: '{kropkaCzyPrzecinek}'.");
            return;
        }
        Ocena.Wartość = wartość;
        DialogResult = true;
    }
    private void Anuluj_Click(object sender, RoutedEventArgs e)
    => DialogResult = false;
    private void Window_Loaded(object sender, RoutedEventArgs e)
    => tbPrzedmiot.Focus();
}