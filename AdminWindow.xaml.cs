using System;
using System.Collections.Generic;
using System.Linq;
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
using Application = System.Windows.Application;

namespace Vending_Machine
{
    public partial class AdminWindow : Window
    {
        Kasa kasa = new Kasa();

        public AdminWindow(Kasa kasa)
        {
            InitializeComponent();
            this.kasa = kasa;
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPanel();
            RefreshNovac();
        }

        private void AzurirajMainWindow(Artikal artikal)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.AzurirajArtikal(artikal.ID);
                    break;
                }
            }
        }
        
        public void AzurirajAdminPanel(int id)
        {
            Artikal artikal = Artikal.GetArtikalIzSlota(id);
            if (artikal != null)
            {
                PrikaziArtikle(artikal);
            }
            else
            {
                UkloniArtikal(id);
            }
        }

        //paneli
        private void Zalihe_Click(object sender, RoutedEventArgs e)
        {
            ZalihePanel.Visibility = Visibility.Visible;
            NovacPanel.Visibility = Visibility.Collapsed;
        }
        private void Novac_Click(object sender, RoutedEventArgs e)
        {
            ZalihePanel.Visibility = Visibility.Collapsed;
            NovacPanel.Visibility = Visibility.Visible;
        }

        //artikli grid
        private void PrikaziArtikle(Artikal artikal)
        {
            int row = artikal.ID / 10;
            int col = artikal.ID % 10;

            if (row < 1 || row > 3 || col < 1 || col > 4) return;

            string pozicijaID = $"{row}_{col}";

            var namePolje = this.FindName($"AdminName_{pozicijaID}") as TextBlock;
            var sizePolje = this.FindName($"AdminSize_{pozicijaID}") as TextBlock;
            var rokPolje = this.FindName($"AdminRok_{pozicijaID}") as TextBlock;
            var countBlock = this.FindName($"AdminCount_{pozicijaID}") as TextBlock;

            if (namePolje != null) namePolje.Text = artikal.ime;
            if (sizePolje != null) sizePolje.Text = artikal.tezina.ToString("F0") + "g";
            if (rokPolje != null) rokPolje.Text = artikal.rokTrajanja.ToString("dd/MM/yy");
            if (countBlock != null)
            {
                int index = (row - 1) * 4 + (col - 1);
                if (Artikal.artikli != null && index < Artikal.artikli.Count)
                {
                    int count = Artikal.artikli[index].Count;
                    if (count == 1)
                    {
                        countBlock.Text = $"{count} artikl";
                    }
                    else
                    {
                        countBlock.Text = $"{count} artikla";
                    }
                }
                else
                {
                    countBlock.Text = "0 artikla";
                }
            }
        }

        public void UkloniArtikal(int artikalID)
        {
            int row = artikalID / 10;
            int col = artikalID % 10;
            string pozicijaID = $"{row}_{col}";

            var namePolje = this.FindName($"AdminName_{pozicijaID}") as TextBlock;
            var sizePolje = this.FindName($"AdminSize_{pozicijaID}") as TextBlock;
            var rokPolje = this.FindName($"AdminRok_{pozicijaID}") as TextBlock;
            var countBlock = this.FindName($"AdminCount_{pozicijaID}") as TextBlock;

            if (namePolje != null) namePolje.Text = "Prazno";
            if (sizePolje != null) sizePolje.Text = "/";
            if (rokPolje != null) rokPolje.Text = "/";
            if (countBlock != null) countBlock.Text = "0 artikla";
        }

        private void AdminAdd_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string pozicija = btn.Tag as string;
            if (pozicija == null) return;

            int id = int.Parse(pozicija.Replace("-", ""));
            int row = id / 10;
            int col = id % 10;
            int index = (row - 1) * 4 + (col - 1);
            int count = 0;
            if (index >= 0 && index < Artikal.artikli.Count)
            {
                count = Artikal.artikli[index].Count;
            }
            else
            {
                count = 0;
            }

            const int maxKapacitet = 10;
            if (count >= maxKapacitet)
            {
                MessageBox.Show($"Mesto je popunjeno!");
                return;
            }

            AddArtikalWindow addArtikalWindow;

            if (count > 0)
            {
                Artikal postojeciArtikal = Artikal.GetArtikalIzSlota(id);
                if (postojeciArtikal != null)
                {
                    addArtikalWindow = new AddArtikalWindow(pozicija, postojeciArtikal);
                }
                else
                {
                    addArtikalWindow = new AddArtikalWindow(pozicija);
                }
            }
            else
            {
                addArtikalWindow = new AddArtikalWindow(pozicija);
            }

            addArtikalWindow.Owner = this;
            if (addArtikalWindow.ShowDialog() == true)
            {
                Artikal noviArtikal = addArtikalWindow.NoviArtikal;

                if (noviArtikal != null)
                {
                    AzurirajMainWindow(noviArtikal);
                    AzurirajAdminPanel(noviArtikal.ID);
                }
            }

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Uspešno ste se izlogovali.");
            this.Close();
        }

        private void RefreshPanel()
        {
            for (int row = 1; row <= 3; row++)
            {
                for (int col = 1; col <= 4; col++)
                {
                    int id = row * 10 + col;
                    AzurirajAdminPanel(id);
                }
            }
        }

        //kod za novac
        private void RefreshNovac()
        {
            AzurirajKolicinu();
            AzurirajUkupno();
        }

        private void AzurirajKolicinu()
        {
            var Kolicina10 = this.FindName($"Kolicina_10") as TextBlock;
            var Kolicina20 = this.FindName($"Kolicina_20") as TextBlock;
            var Kolicina50 = this.FindName($"Kolicina_50") as TextBlock;
            var Kolicina100 = this.FindName($"Kolicina_100") as TextBlock;
            var Kolicina200 = this.FindName($"Kolicina_200") as TextBlock;
            var Kolicina500 = this.FindName($"Kolicina_500") as TextBlock;

            if (Kolicina10 != null) Kolicina10.Text = Convert.ToString(kasa.getKolicina(10));
            if (Kolicina20 != null) Kolicina20.Text = Convert.ToString(kasa.getKolicina(20));
            if (Kolicina50 != null) Kolicina50.Text = Convert.ToString(kasa.getKolicina(50));
            if (Kolicina100 != null) Kolicina100.Text = Convert.ToString(kasa.getKolicina(100));
            if (Kolicina200 != null) Kolicina200.Text = Convert.ToString(kasa.getKolicina(200));
            if (Kolicina500 != null) Kolicina500.Text = Convert.ToString(kasa.getKolicina(500));
        }
        private void AzurirajUkupno()
        {
            var UkupnoNovca = this.FindName($"Ukupno") as TextBlock;
            if (UkupnoNovca != null) UkupnoNovca.Text = Convert.ToString(kasa.ukupno);

            var Ukupno10 = this.FindName($"Ukupno_10") as TextBlock;
            var Ukupno20 = this.FindName($"Ukupno_20") as TextBlock;
            var Ukupno50 = this.FindName($"Ukupno_50") as TextBlock;
            var Ukupno100 = this.FindName($"Ukupno_100") as TextBlock;
            var Ukupno200 = this.FindName($"Ukupno_200") as TextBlock;
            var Ukupno500 = this.FindName($"Ukupno_500") as TextBlock;

            if (Ukupno10 != null) Ukupno10.Text = Convert.ToString(10 * kasa.getKolicina(10));
            if (Ukupno20 != null) Ukupno20.Text = Convert.ToString(20 * kasa.getKolicina(20));
            if (Ukupno50 != null) Ukupno50.Text = Convert.ToString(50 * kasa.getKolicina(50));
            if (Ukupno100 != null) Ukupno100.Text = Convert.ToString(100 * kasa.getKolicina(100));
            if (Ukupno200 != null) Ukupno200.Text = Convert.ToString(200 * kasa.getKolicina(200));
            if (Ukupno500 != null) Ukupno500.Text = Convert.ToString(500 * kasa.getKolicina(500));
        }

        private void DodajNovac_Click(object sender, RoutedEventArgs e)
        {
            var Dodaj10 = this.FindName($"Dodaj_10") as TextBox;
            var Dodaj20 = this.FindName($"Dodaj_20") as TextBox;
            var Dodaj50 = this.FindName($"Dodaj_50") as TextBox;
            var Dodaj100 = this.FindName($"Dodaj_100") as TextBox;
            var Dodaj200 = this.FindName($"Dodaj_200") as TextBox;
            var Dodaj500 = this.FindName($"Dodaj_500") as TextBox;

            if (Dodaj10 != null && int.TryParse(Dodaj10.Text, out int kolicina10))
            {
                if (kolicina10 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 10.");
                    return;
                }
                for (int i = 0; i < kolicina10; i++)
                {
                    kasa.DodajNovcaniceAdmin(10);
                }
            }
            if (Dodaj20 != null && int.TryParse(Dodaj20.Text, out int kolicina20))
            {
                if (kolicina20 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 20.");
                    return;
                }
                for (int i = 0; i < kolicina20; i++)
                {
                    kasa.DodajNovcaniceAdmin(20);
                }
            }
            if (Dodaj50 != null && int.TryParse(Dodaj50.Text, out int kolicina50))
            {
                if (kolicina50 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 50.");
                    return;
                }
                for (int i = 0; i < kolicina50; i++)
                {
                    kasa.DodajNovcaniceAdmin(50);
                }
            }
            if (Dodaj100 != null && int.TryParse(Dodaj100.Text, out int kolicina100))
            {
                if (kolicina100 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 100.");
                    return;
                }
                for (int i = 0; i < kolicina100; i++)
                {
                    kasa.DodajNovcaniceAdmin(100);
                }
            }
            if (Dodaj200 != null && int.TryParse(Dodaj200.Text, out int kolicina200))
            {
                if (kolicina200 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 200.");
                    return;
                }
                for (int i = 0; i < kolicina200; i++)
                {
                    kasa.DodajNovcaniceAdmin(200);
                }
            }
            if (Dodaj500 != null && int.TryParse(Dodaj500.Text, out int kolicina500))
            {
                if (kolicina500 < 0)
                {
                    MessageBox.Show("Unesite validan broj za novčanice od 500.");
                    return;
                }
                for (int i = 0; i < kolicina500; i++)
                {
                    kasa.DodajNovcaniceAdmin(500);
                }
            }

            if (Dodaj10 != null) Dodaj10.Text = "0";
            if (Dodaj20 != null) Dodaj20.Text = "0";
            if (Dodaj50 != null) Dodaj50.Text = "0";
            if (Dodaj100 != null) Dodaj100.Text = "0";
            if (Dodaj200 != null) Dodaj200.Text = "0";
            if (Dodaj500 != null) Dodaj500.Text = "0";

            RefreshNovac();
        }
    }
}
