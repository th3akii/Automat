using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vending_Machine
{
    /// <summary>
    /// Interaction logic for AddArtikalWindow.xaml
    /// </summary>
    public partial class AddArtikalWindow : Window
    {
        public Artikal NoviArtikal { get; set; }
        private int id;
        private DateTime? poslednjiRokTrajanja;

        public AddArtikalWindow(string pozicija)
        {
            InitializeComponent();
            TitleText.Text = $"Dodaj Artikal na Poziciju {pozicija}";

            id = int.Parse(pozicija.Replace("-", ""));

            var today = DateTime.Today;
            RokTrajanja.BlackoutDates.Clear();
            if (today > DateTime.MinValue)
                RokTrajanja.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, today.AddDays(-1)));
            RokTrajanja.DisplayDateStart = today;
            RokTrajanja.DisplayDate = today;
            RokTrajanja.SelectedDate = today;
        }

        public AddArtikalWindow(string pozicija, Artikal artikal) : this(pozicija)
        {
            poslednjiRokTrajanja = artikal.rokTrajanja.Date;

            Naziv.Text = artikal.ime;
            Tezina.Text = artikal.tezina.ToString(CultureInfo.InvariantCulture);
            Cena.Text = artikal.cena.ToString(CultureInfo.InvariantCulture);
            Popust.Text = artikal.popust.ToString(CultureInfo.InvariantCulture);

            DateTime last;
            var poslednjiDatum = Artikal.GetPoslednjiDatum(id);
            if (poslednjiDatum.HasValue)
            {
                last = poslednjiDatum.Value;
            }
            else
            {
                last = artikal.rokTrajanja;
            }
            poslednjiRokTrajanja = last.Date;

            var today = DateTime.Today;
            var minSelectable = poslednjiRokTrajanja.Value < today ? today : poslednjiRokTrajanja.Value;

            RokTrajanja.BlackoutDates.Clear();

            RokTrajanja.DisplayDateStart = minSelectable;
            RokTrajanja.DisplayDate = minSelectable;
            RokTrajanja.SelectedDate = minSelectable;

            if (minSelectable > DateTime.MinValue)
                RokTrajanja.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, minSelectable.AddDays(-1)));

            string srDan = artikal.dan switch
            {
                DayOfWeek.Monday => "Ponedeljak",
                DayOfWeek.Tuesday => "Utorak",
                DayOfWeek.Wednesday => "Sreda",
                DayOfWeek.Thursday => "Četvrtak",
                DayOfWeek.Friday => "Petak",
                DayOfWeek.Saturday => "Subota",
                DayOfWeek.Sunday => "Nedelja"
            };

            foreach (ComboBoxItem item in DanPopusta.Items)
            {
                if ((item.Content as string) == srDan)
                {
                    DanPopusta.SelectedItem = item;
                    break;
                }
            }

            Naziv.IsReadOnly = true;
            Tezina.IsReadOnly = true;
            Cena.IsReadOnly = true;
            Popust.IsReadOnly = true;
            DanPopusta.IsEnabled = false;
        }

        private void DodajArtikal_Click(object sender, RoutedEventArgs e)
        {
            string ime = Naziv.Text;
            if (string.IsNullOrWhiteSpace(ime))
            {
                MessageBox.Show("Naziv ne sme biti prazan.");
                return;
            }

            if (RokTrajanja.SelectedDate == null)
            {
                MessageBox.Show("Morate izabrati rok trajanja.");
                return;
            }
            DateTime rokTrajanja = RokTrajanja.SelectedDate.Value;

            string tezinaText = Tezina.Text;
            if (string.IsNullOrWhiteSpace(tezinaText))
            {
                MessageBox.Show("Težina ne sme biti prazna.");
                return;
            }
            double.TryParse(Tezina.Text, out double tezina);
            if (tezina <= 0)
            {
                MessageBox.Show("Težina mora biti veća od nule.");
                return;
            }

            string cenaText = Cena.Text;
            if (string.IsNullOrWhiteSpace(cenaText))
            {
                MessageBox.Show("Cena ne sme biti prazna.");
                return;
            }
            double.TryParse(Cena.Text, out double cena);
            if (cena < 0)
            {
                MessageBox.Show("Cena ne sme biti negativna.");
                return;
            }

            var selectedItem = DanPopusta.SelectedItem as ComboBoxItem;
            string danString = selectedItem?.Content as string;
            DayOfWeek dan = danString switch
            {
                "Ponedeljak" => DayOfWeek.Monday,
                "Utorak" => DayOfWeek.Tuesday,
                "Sreda" => DayOfWeek.Wednesday,
                "Četvrtak" => DayOfWeek.Thursday,
                "Petak" => DayOfWeek.Friday,
                "Subota" => DayOfWeek.Saturday,
                "Nedelja" => DayOfWeek.Sunday,
            };

            string popustText = Popust.Text;
            double popust = 0;
            if (!string.IsNullOrWhiteSpace(popustText))
            {
                double.TryParse(Popust.Text, out popust);
                if (popust < 0)
                {
                    MessageBox.Show("Popust ne sme biti negativan.");
                    return;
                }
            }

            this.NoviArtikal = new Artikal(id, ime, rokTrajanja, tezina, cena, dan, popust);
            MessageBox.Show("Artikal uspešno dodat!");

            this.DialogResult = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
