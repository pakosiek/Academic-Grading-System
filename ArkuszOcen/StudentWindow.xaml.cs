using ArkuszOcen.Model;
using System.Text.RegularExpressions;
using System.Windows;
namespace ArkuszOcen;
public partial class StudentWindow : Window {
    public Student? Student { get; set; }
    public StudentWindow(Student student) {
        InitializeComponent();
        Student = student;
        tbImię.Text = Student.Imię;
        tbNazwisko.Text = Student.Nazwisko;
        tbNumerIndeksu.Text = Student.NumerIndeksu;
        tbWydział.Text = Student.Wydział;
    }
    private void Zatwierdź_Click(object sender, RoutedEventArgs e) {
        if (
        !Regex.IsMatch(tbImię.Text, @"^\p{Lu}\p{Ll}{1,20}$") ||
        !Regex.IsMatch(tbNazwisko.Text, @"^\p{Lu}\p{Ll}{1,20}(-\p{Lu}\p{Ll}{1,20})?$") ||
        !Regex.IsMatch(tbNumerIndeksu.Text, @"^[0-9]{4,10}$") ||
        !Regex.IsMatch(tbWydział.Text, @"^[\p{Lu}|\p{Ll}]{1,12}$")
        ) {
            MessageBox.Show("Wprowadzone dane są niepoprawne.");
            return;
        }
        if (Student is null) return;
        Student.Imię = tbImię.Text;
        Student.Nazwisko = tbNazwisko.Text;
        Student.NumerIndeksu = tbNumerIndeksu.Text;
        Student.Wydział = tbWydział.Text;
        DialogResult = true; //można zamknąć okno i pobrać dane z w. Student
    }
    private void Anuluj_Click(object sender, RoutedEventArgs e)
    => DialogResult = false;
    private void Window_Loaded(object sender, RoutedEventArgs e)
    => tbImię.Focus();
}