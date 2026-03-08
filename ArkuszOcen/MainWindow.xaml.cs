using ArkuszOcen.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
namespace ArkuszOcen
{
    public partial class MainWindow : Window
    {
        private RoutedEventHandler? ostatnieZdarzenie;
        private RoutedEventArgs? ostatnieArgumenty;
        public MainWindow()
        {
            InitializeComponent();
            ostatnieZdarzenie = StudenciWyświetlWszystkich_Click;
            ostatnieArgumenty = null;
            ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
        }
        private static T? PobierzObiektZTabeli<T>(Selector kontrolka) where T : class
        {
            var zaznaczonyElement = kontrolka?.SelectedItem;
            if (zaznaczonyElement == null) return null;
            var właściwość = zaznaczonyElement.GetType()
            .GetProperties()
            .FirstOrDefault(p => typeof(T).IsAssignableFrom(p.PropertyType));
            return właściwość?.GetValue(zaznaczonyElement) as T;
        }

        private void StudenciDodajNowego_Click(object sender, RoutedEventArgs e)
        {
            var student = new Student();
            var okno = new StudentWindow(student)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (okno.ShowDialog() == true)
            {
                using var bd = new AppDbContext();

                bool istnieje = bd.Studenci.Any(s => s.NumerIndeksu == student.NumerIndeksu);
                if (istnieje)
                {
                    MessageBox.Show(
                        $"Student z numerem indeksu {student.NumerIndeksu} już istnieje.",
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                    return;
                }

                bd.Studenci.Add(student);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show($"Student `{student}` został dodany.");
            }
        }
        private void StudenciEdytujWybranego_Click(object sender, RoutedEventArgs e)
        {
            var student = PobierzObiektZTabeli<Student>(dg); if (student == null) return;
            var okno = new StudentWindow(student)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (okno.ShowDialog() == true)
            {
                using var bd = new AppDbContext();
                bool istnieje = bd.Studenci.Any(s => s.NumerIndeksu == student.NumerIndeksu && s.StudentId != student.StudentId); // dodanie braku powielania indeksu
                if (istnieje)
                {
                    MessageBox.Show(
                        $"Student z numerem indeksu {student.NumerIndeksu} już istnieje.",
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                    return;
                }
                bd.Studenci.Update(student);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show($"Dane studenta `{student}` zostały zaktualizowane.");
            }
        }
        private void StudenciUsuńWybranego_Click(object sender, RoutedEventArgs e)
        {
            var student = PobierzObiektZTabeli<Student>(dg); if (student == null) return;
            var czyUsunąć = MessageBox.Show(
            $"Czy na pewno chcesz usunąć studenta `{student}`?",
            "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning
            );
            if (czyUsunąć == MessageBoxResult.Yes)
            {
                using var bd = new AppDbContext();
                bd.Studenci.Remove(student);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show($"Student `{student}` został usunięty.");
            }
        }
        private void StudenciWyświetlWszystkich_Click(object sender, RoutedEventArgs e)
        {
            ostatnieZdarzenie = StudenciWyświetlWszystkich_Click;
            ostatnieArgumenty = null!;
            using var bd = new AppDbContext();
            var wynik = bd
            .Studenci
            .Include(s => s.Oceny)
            .Select(s => new
            {
                Student = s, //dodany cały obiekt student
                s.StudentId,
                s.Nazwisko,
                s.Imię,
                s.NumerIndeksu,
                s.Wydział,
                s.Oceny //dodany cały obiekt grade
            })
            .ToList();
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik;
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdStudenta",
                Binding = new Binding(nameof(Student.StudentId))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Imię",
                Binding = new Binding(nameof(Student.Imię))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Nazwisko",
                Binding = new Binding(nameof(Student.Nazwisko))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Numer indeksu",
                Binding = new Binding(nameof(Student.NumerIndeksu))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Wydział",
                Binding = new Binding(nameof(Student.Wydział))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Oceny",
                Binding = new Binding(nameof(Student.Oceny))
                {
                    Converter = new OcenyConverter()
                }
            });
            dg.IsReadOnly = true;
        }
        private void StudenciWyświetlWszystkichZOcenami_Click(object sender, RoutedEventArgs e)
        {
            ostatnieZdarzenie = StudenciWyświetlWszystkichZOcenami_Click;
            ostatnieArgumenty = null!;
            using var bd = new AppDbContext();
            var wynik =
            from student in bd.Studenci
            join ocena in bd.Oceny
            on student.StudentId equals ocena.StudentId
            into studentOceny
            from ocena in studentOceny.DefaultIfEmpty() //left join
            orderby student.Nazwisko, student.Imię, student.NumerIndeksu
            select new
            {
                Student = student, //dodany cały obiekt student
                student.StudentId,
                student.Nazwisko,
                student.Imię,
                student.NumerIndeksu,
                student.Wydział,
                Ocena = ocena, //dodany cały obiekt grade
                OcenaId = ocena != null ? ocena.OcenaId : (int?)null,
                Przedmiot = ocena != null ? ocena.Przedmiot : null,
                ocenaSformatowana = ocena != null ? string.Format("{0:F1}", ocena.Wartość) : "Brak oceny"
            };
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik.ToList();
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdStudenta",
                Binding = new Binding(nameof(Student.StudentId))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Imię",
                Binding = new Binding(nameof(Student.Imię))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Nazwisko",
                Binding = new Binding(nameof(Student.Nazwisko))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Numer indeksu",
                Binding = new Binding(nameof(Student.NumerIndeksu))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Wydział",
                Binding = new Binding(nameof(Student.Wydział))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdOceny",
                Binding = new Binding(nameof(Ocena.OcenaId))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Przedmiot",
                Binding = new Binding(nameof(Ocena.Przedmiot))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Wartość oceny",
                Binding = new Binding("ocenaSformatowana")
            });
            dg.IsReadOnly = true;
        }
        private void StudenciWyświetlWszystkichZLiczbąOcenIŚrednią_Click(
        object sender, RoutedEventArgs e)
        {
            ostatnieZdarzenie = StudenciWyświetlWszystkichZLiczbąOcenIŚrednią_Click;
            ostatnieArgumenty = null!;
            using var bd = new AppDbContext();
            var wynik =
            from student in bd.Studenci
            join grade in bd.Oceny on student.StudentId
            equals grade.StudentId into gradesGroup
            from grades in gradesGroup.DefaultIfEmpty()
            group grades by student into g
            select new
            {
                Student = g.Key, //dodany cały obiekt student
                g.Key.StudentId,
                g.Key.Nazwisko,
                g.Key.Imię,
                g.Key.NumerIndeksu,
                g.Key.Wydział,
                LiczbaOcen = g.Count(),
                OcenaŚrednia = string.Format("{0:F1}", g.Average(gg => gg != null ? gg.Wartość : 0)),
                OcenaNajgorsza = string.Format("{0:F1}", g.Min(gg => gg != null ? gg.Wartość : 0)),
                OcenaNajlepsza = string.Format("{0:F1}", g.Max(gg => gg != null ? gg.Wartość : 0))
            };
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik.ToList();
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdStudenta",
                Binding = new Binding(nameof(Student.StudentId))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Imię",
                Binding = new Binding(nameof(Student.Imię))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Nazwisko",
                Binding = new Binding(nameof(Student.Nazwisko))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Numer indeksu",
                Binding = new Binding(nameof(Student.NumerIndeksu))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Wydział",
                Binding = new Binding(nameof(Student.Wydział))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Liczba ocen",
                Binding = new Binding("LiczbaOcen")
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Średnia ocen",
                Binding = new Binding("OcenaŚrednia")
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Najgorsza ocena",
                Binding = new Binding("OcenaNajgorsza")
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Najlepsza ocena",
                Binding = new Binding("OcenaNajlepsza")
            });
            dg.IsReadOnly = true;
        }
        private void StudenciWyświetlLiczbęWszystkich_Click(object sender, RoutedEventArgs e)
        {
            using var bd = new AppDbContext();
            var wynik = new List<string> { bd.Studenci.Count().ToString() };
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik;
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Liczba studentów",
                Binding = new Binding(".")
            });
            dg.IsReadOnly = true;
        }
        private void StudenciWyświetlNajlepszegoINajgorszego_Click(
        object sender, RoutedEventArgs e)
        {
            ostatnieZdarzenie = StudenciWyświetlNajlepszegoINajgorszego_Click;
            ostatnieArgumenty = null!;
            using var bd = new AppDbContext();
            var najlepszyStudent = bd.Studenci
            .Where(s => s.Oceny != null && s.Oceny.Any())
            .OrderByDescending(s => s.Oceny!.Average(g => g.Wartość))
            .Select(s => new
            {
                Student = s,
                s.StudentId,
                s.Nazwisko,
                s.Imię,
                s.NumerIndeksu,
                s.Wydział,
                OcenaŚrednia = string.Format("{0:F1}", s.Oceny!.Average(g => g.Wartość)),
                Etykieta = "najwyższa"
            })
            .First();
            var najgorszyStudent = bd.Studenci
            .Where(s => s.Oceny != null && s.Oceny.Any())
            .OrderBy(s => s.Oceny!.Average(g => g.Wartość))
            .Select(s => new
            {
                Student = s,
                s.StudentId,
                s.Nazwisko,
                s.Imię,
                s.NumerIndeksu,
                s.Wydział,
                OcenaŚrednia = string.Format("{0:F1}", s.Oceny!.Average(g => g.Wartość)),
                Etykieta = "najniższa"
            })
            .First();
            List<object> wynik = [najlepszyStudent, najgorszyStudent];
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik;
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdStudenta",
                Binding = new Binding(nameof(Student.StudentId))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Imię",
                Binding = new Binding(nameof(Student.Imię))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Nazwisko",
                Binding = new Binding(nameof(Student.Nazwisko))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Numer indeksu",
                Binding = new Binding(nameof(Student.NumerIndeksu))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Wydział",
                Binding = new Binding(nameof(Student.Wydział))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Średnia ocen",
                Binding = new Binding("OcenaŚrednia")
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Informacja o średniej",
                Binding = new Binding("Etykieta")
            });
            dg.IsReadOnly = true;
        }
        private void StudenciWyświetlPodobnychDoStudentaWybranegoWSensieOcen_Click(
        object sender, RoutedEventArgs e)
        { }
        private void OcenyDodajDlaWybranegoStudenta_Click(object sender, RoutedEventArgs e)
        {
            var student = PobierzObiektZTabeli<Student>(dg); if (student == null) return;
            var ocena = new Ocena() { StudentId = student.StudentId };
            var okno = new OcenaWindow(ocena)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (okno.ShowDialog() == true)
            {
                using var bd = new AppDbContext();
                bd.Oceny.Add(ocena);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show($"Ocena `{ocena}` dla studenta `{student}` została dodana.");
            }
        }
        private void OcenyEdytujWybraną_Click(object sender, RoutedEventArgs e)
        {
            var ocena = PobierzObiektZTabeli<Ocena>(dg); if (ocena == null) return;
            var okno = new OcenaWindow(ocena)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (okno.ShowDialog() == true)
            {
                using var bd = new AppDbContext();
                bd.Oceny.Update(ocena);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show(
                $"Ocena `{ocena}` dla studenta `{ocena.Student}` została zaktualizowana."
                );
            }
        }
        private void OcenyUsuńWybraną_Click(object sender, RoutedEventArgs e)
        {
            var ocena = PobierzObiektZTabeli<Ocena>(dg); if (ocena == null) return;
            var czyUsunąć = MessageBox.Show(
            $"Czy na pewno chcesz usunąć ocenę `{ocena}` studenta `{ocena.Student}`?",
            "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning
            );
            if (czyUsunąć == MessageBoxResult.Yes)
            {
                using var bd = new AppDbContext();
                bd.Oceny.Remove(ocena);
                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show($"Ocena została usunięta.");
            }
        }
        private void OcenyWyświetlWszystkie_Click(object sender, RoutedEventArgs e)
        => StudenciWyświetlWszystkich_Click(sender, e);
        private void OcenyWyświetlWszystkieDlaWybranegoStudenta_Click(
        object sender, RoutedEventArgs e)
        {
            ostatnieZdarzenie = OcenyWyświetlWszystkieDlaWybranegoStudenta_Click;
            ostatnieArgumenty = null!;
            var student = PobierzObiektZTabeli<Student>(dg); if (student == null) return;
            using var bd = new AppDbContext();
            var wynik =
            from s
            in bd.Studenci.Where(ss => ss.StudentId == student.StudentId)
            join grade in bd.Oceny on s.StudentId equals grade.StudentId
            orderby s.Nazwisko, s.Imię, s.NumerIndeksu
            select new
            {
                Student = s, //dodany cały obiekt student
                s.StudentId,
                s.Nazwisko,
                s.Imię,
                s.NumerIndeksu,
                s.Wydział,
                Ocena = grade, //dodany cały obiekt grade
                grade.OcenaId,
                grade.Przedmiot,
                ocenaSformatowana = string.Format("{0:F1}", grade.Wartość)
            };
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik.ToList();
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdStudenta",
                Binding = new Binding(nameof(Student.StudentId))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Imię",
                Binding = new Binding(nameof(Student.Imię))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Nazwisko",
                Binding = new Binding(nameof(Student.Nazwisko))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Numer indeksu",
                Binding = new Binding(nameof(Student.NumerIndeksu))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "Wydział",
                Binding = new Binding(nameof(Student.Wydział))
            });
            dg.Columns.Add(new DataGridTextColumn()
            {
                Header = "IdOceny",
                Binding = new Binding(nameof(Ocena.OcenaId))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Przedmiot",
                Binding = new Binding(nameof(Ocena.Przedmiot))
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Wartość oceny",
                Binding = new Binding("ocenaSformatowana")
            });
            dg.IsReadOnly = true;
        }
        private void OcenyWyświetlLiczbęWszystkich_Click(object sender, RoutedEventArgs e)
        {
            using var bd = new AppDbContext();
            var wynik = new List<string> {
            bd.Oceny.Count().ToString()
            };
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = wynik;
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Liczba wszystkich ocen",
                Binding = new Binding(".")
            });
            dg.IsReadOnly = true;
        }
        private void OcenyWyświetlŚredniąZPrzedmiotów_Click(object sender, RoutedEventArgs e)
        {
            using var bd = new AppDbContext();
            var result = bd.Oceny
            .GroupBy(g => g.Przedmiot)
            .Select(g => new
            {
                Przedmiot = g.Key,
                OcenaŚrednia = string.Format("{0:F1}", g.Average(g => g.Wartość))
            })
            .ToList();
            dg.ItemsSource = null; dg.Columns.Clear(); dg.Items.Clear(); dg.AutoGenerateColumns = false;
            dg.ItemsSource = result;
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Przedmiot",
                Binding = new Binding("Przedmiot")
            });
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Średnia ocen",
                Binding = new Binding("OcenaŚrednia")
            });
            dg.IsReadOnly = true;
        }
        private void ImportujZPliku_Click(object sender, RoutedEventArgs e) // nowa funkcja importuj z pliku
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            { };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = System.IO.File.ReadAllLines(openFileDialog.FileName);

                    using var bd = new AppDbContext();

                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length < 4) continue;
                        string numerIndeksu = parts[3].Trim();
                        bool istnieje = bd.Studenci.Any(s => s.NumerIndeksu == numerIndeksu); // sprawdzamy czy juz istnieje jak tak nie dodajemy
                        if (istnieje)
                        {
                            continue;
                        }
                        var student = new Student
                        {
                            Imię = parts[0].Trim(),
                            Nazwisko = parts[1].Trim(),
                            Wydział = parts[2].Trim(),
                            NumerIndeksu = numerIndeksu
                        };

                        bd.Studenci.Add(student);

                    }
                    bd.SaveChanges();
                    MessageBox.Show("Import zakończony.");
                    ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);

                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Błąd podczas wczytywania pliku:\n{ex.Message}");
                }
            }
        }

        private void StudenciUsuńWszystkich_Click(object sender, RoutedEventArgs e) // nowa funkcja usun wszystkich
        {
            var confirm = MessageBox.Show(
                "Czy na pewno chcesz usunąć wszystkich studentów i ich oceny?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                using var bd = new AppDbContext();

                // Najpierw usuń oceny, potem studentów (bo są zależne przez klucz obcy)
                bd.Oceny.RemoveRange(bd.Oceny);
                bd.Studenci.RemoveRange(bd.Studenci);

                bd.SaveChanges();
                ostatnieZdarzenie?.Invoke(this, ostatnieArgumenty);
                MessageBox.Show("Wszyscy studenci i ich oceny zostali usunięci.");
            }
        }
        private async void PlikEksportuj_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new()
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = "Studenci",
                Filter =
                    "Dokumenty tekstowe (*.txt)|*.txt|" +
                    "Dokumenty XML (*.xml)|*.xml|" +
                    "Dokumenty JSON (*.json)|*.json|" +
                    "Dokumenty binarne (*.bin)|*.bin"
            };

            if (d.ShowDialog() == true)
            {

                MessageBox.Show("Trwa eskport danych proszę czekać");

                bool czyWyeksportowano = false;
                await Task.Run(() =>
                { 
                    using var bd = new AppDbContext();
                    var result = bd.Studenci.Include(s => s.Oceny).ToList();

                    string rozszerzenie = Path.GetExtension(d.FileName).ToLower();

                    switch (rozszerzenie)
                    {
                        case ".txt":
                            czyWyeksportowano = PlikTxtManager.Eksportuj(d.FileName, result);
                            break;
                        case ".json":
                            czyWyeksportowano = PlikJsonManager.Eksportuj(d.FileName, result);
                            break;
                        case ".xml":
                            czyWyeksportowano = PlikXmlManager.Eksportuj(d.FileName, result);
                            break;
                        case ".bin":
                            czyWyeksportowano = PlikBinManager.Eksportuj(d.FileName, result);
                            break;
                        default:
                            MessageBox.Show("Nieobsługiwany format pliku.");
                            return;
                    }
                });

                if (czyWyeksportowano)
                    MessageBox.Show("Eksport został zakończony pomyślnie");
            }
        }

        private async void PlikImportuj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new()
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = "Studenci",
                Filter =
                    "Dokumenty tekstowe (*.txt)|*.txt|" +
                    "Dokumenty XML (*.xml)|*.xml|" +
                    "Dokumenty JSON (*.json)|*.json|" +
                    "Dokumenty binarne (*.bin)|*.bin"
            };

            if (d.ShowDialog() == true)
            {
                MessageBox.Show("Trwa import danych proszę czekać");

                List<Student> result;

                string rozszerzenie = Path.GetExtension(d.FileName).ToLower();

                await Task.Run(() =>
                {
                    switch (rozszerzenie)
                    {
                        case ".txt":
                            result = PlikTxtManager.Importuj(d.FileName);
                            break;
                        case ".json":
                            result = PlikJsonManager.Importuj(d.FileName);
                            break;
                        case ".xml":
                            result = PlikXmlManager.Importuj(d.FileName);
                            break;
                        case ".bin":
                            result = PlikBinManager.Importuj(d.FileName);
                            break;
                        default:
                            MessageBox.Show("Nieobsługiwany format pliku.");
                            return;
                    }

                    using var bd = new AppDbContext();
                    foreach (var student in result)
                    {
                        bool istnieje = bd.Studenci.Any(s => s.NumerIndeksu == student.NumerIndeksu);
                        if (!istnieje)
                        {
                            bd.Studenci.Add(student);
                        }
                    }
                    bd.SaveChanges();
                });
                StudenciWyświetlWszystkich_Click(sender, e);

                MessageBox.Show("Import został zakończony pomyślnie");
            }
        }
        private void PlikZamknij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}