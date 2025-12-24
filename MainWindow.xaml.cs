using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vending_Machine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private Kasa kasa = new Kasa();

        public MainWindow()
        {
            InitializeComponent();
        }

        //otvara admin panel
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            if (loginWindow.ShowDialog() == true)
            {
                AdminWindow adminWindow = new AdminWindow(kasa);
                adminWindow.Owner = this;
                adminWindow.ShowDialog();
            }
        }

        //logika za strelice
        private void RowUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowTextBox.Text, out int value))
            {
                if (value < 3) { value++; }
                else { value = 1; }
                RowTextBox.Text = value.ToString();
            }
            else
            {
                RowTextBox.Text = "1";
            }
        }
        private void RowDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowTextBox.Text, out int value))
            {
                if (value > 1) { value--; }
                else { value = 3; }
                RowTextBox.Text = value.ToString();
            }
            else
            {
                RowTextBox.Text = "3";
            }
        }

        private void ColumnUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ColumnTextBox.Text, out int value))
            {
                if (value < 4) { value++; }
                else { value = 1; }
                ColumnTextBox.Text = value.ToString();
            }
            else
            {
                ColumnTextBox.Text = "1";
            }
        }
        private void ColumnDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ColumnTextBox.Text, out int value))
            {
                if (value > 1) { value--; }
                else { value = 4; }
                ColumnTextBox.Text = value.ToString();
            }
            else
            {
                ColumnTextBox.Text = "4";
            }
        }

        //radi na enter i proverava da li je izmedju 1 i 3/4
        private void RowTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(RowTextBox.Text, out int value))
                {
                    if (value < 1) value = 1;
                    if (value > 3) value = 3;
                    RowTextBox.Text = value.ToString();
                }
                else
                {
                    RowTextBox.Text = "1";
                }
            }
        }
        private void ColumnTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(ColumnTextBox.Text, out int value))
                {
                    if (value < 1) value = 1;
                    if (value > 4) value = 4;
                    ColumnTextBox.Text = value.ToString();
                }
                else
                {
                    ColumnTextBox.Text = "1";
                }
            }
        }

        private void UbaciNovac_Click(object sender, RoutedEventArgs e)
        {
            AddMoney addMoney = new AddMoney(kasa);
            addMoney.Owner = this;
            addMoney.ShowDialog();
        }

        private void UzmiKusur_Click(object sender, RoutedEventArgs e)
        {
            kasa.vratiKusur();
        }

        private void Kupi_Click(object sender, RoutedEventArgs e)
        {
            int row = int.Parse(RowTextBox.Text);
            int col = int.Parse(ColumnTextBox.Text);

            int id = row * 10 + col;

            Kupi(id);
        }

        private void Kupi(int id)
        {
            Artikal kupljenArtikal = Artikal.GetArtikalIzSlota(id);

            if (kupljenArtikal != null)
            {
                double cenaZaNaplatu = kupljenArtikal.GetCena();
                if (kasa.kredit < cenaZaNaplatu)
                {
                    MessageBox.Show("Nemas dovoljno novca!");
                    return;
                }
                else
                {
                    kasa.kredit -= Convert.ToInt32(cenaZaNaplatu);
                    Artikal.UkloniArtikalIzSlota(id);
                    MessageBox.Show($"Uspesno ste kupili artikal: {kupljenArtikal.ime}");

                    AzurirajStanje(kasa.kredit);
                    AzurirajArtikal(id);
                    AdminWindow adminWindow = System.Windows.Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();
                    if (adminWindow != null)
                    {
                        adminWindow.AzurirajAdminPanel(id);
                    }
                }
            }
            else
            {
                MessageBox.Show("Jel vidis da je prazno?");
            }
        }

        //metoda koja menja text u slotu
        public void AzurirajArtikal(int artikalID)
        {
            int row = artikalID / 10;
            int col = artikalID % 10;

            if (row < 1 || row > 3 || col < 1 || col > 4) return;
            int index = (row - 1) * 4 + (col - 1);

            Artikal? artikal = null;
            if (index >= 0 && index < Artikal.artikli.Count && Artikal.artikli[index].Count > 0)
            {
                artikal = Artikal.GetArtikalIzSlota(artikalID);
            }

            string pozicijaID = $"{row}_{col}";

            var namePolje = this.FindName($"ArticalName_{pozicijaID}") as TextBlock;
            var sizePolje = this.FindName($"ArticalSize_{pozicijaID}") as TextBlock;
            var pricePolje = this.FindName($"ArticalPrice_{pozicijaID}") as TextBlock;
            var rokPolje = this.FindName($"ArticalRok_{pozicijaID}") as TextBlock;
            var countBlock = this.FindName($"ArticalAvailable_{pozicijaID}") as TextBlock;

            if (artikal != null)
            {
                if (namePolje != null) namePolje.Text = artikal.ime;
                if (sizePolje != null) sizePolje.Text = artikal.tezina.ToString("F0") + "g";
                if (pricePolje != null) pricePolje.Text = artikal.GetCena().ToString();
                if (rokPolje != null) rokPolje.Text = artikal.rokTrajanja.ToString("dd/MM/yy");
                if (countBlock != null)
                {
                    int count = Artikal.artikli[index].Count;
                    if (count == 1)
                    {
                        countBlock.Text = $"{count} artikal";
                    }
                    else
                    {
                        countBlock.Text = $"{count} artikla";
                    }
                }
            }
            else
            {
                if (namePolje != null) namePolje.Text = "Prazno";
                if (sizePolje != null) sizePolje.Text = "";
                if (pricePolje != null) pricePolje.Text = "";
                if (rokPolje != null) rokPolje.Text = "";
                if (countBlock != null) countBlock.Text = "0 artikla";
            }
        }

        public void UkloniArtikal(int artikalID)
        {
            int row = artikalID / 10;
            int col = artikalID % 10;
            string pozicijaID = $"{row}_{col}";

            var namePolje = this.FindName($"ArticalName_{pozicijaID}") as TextBlock;
            var sizePolje = this.FindName($"ArticalSize_{pozicijaID}") as TextBlock;
            var pricePolje = this.FindName($"ArticalPrice_{pozicijaID}") as TextBlock;
            var rokPolje = this.FindName($"ArticalRok_{pozicijaID}") as TextBlock;
            var countBlock = this.FindName($"ArticalAvailable_{pozicijaID}") as TextBlock;

            if (namePolje != null) namePolje.Text = "Prazno";
            if (sizePolje != null) sizePolje.Text = "";
            if (pricePolje != null) pricePolje.Text = "";
            if (rokPolje != null) rokPolje.Text = "";
            if (countBlock != null) countBlock.Text = "0 artikla";
        }

        public void AzurirajStanje(double novac)
        {
            Kredit.Text = $"{novac:F2} RSD";
        }

        private void Artical_1_1_Click(object sender, RoutedEventArgs e)
        {
            Kupi(11);
        }
        private void Artical_1_2_Click(object sender, RoutedEventArgs e)
        {
            Kupi(12);
        }
        private void Artical_1_3_Click(object sender, RoutedEventArgs e)
        {
            Kupi(13);
        }
        private void Artical_1_4_Click(object sender, RoutedEventArgs e)
        {
            Kupi(14);
        }
        private void Artical_2_1_Click(object sender, RoutedEventArgs e)
        {
            Kupi(21);
        }
        private void Artical_2_2_Click(object sender, RoutedEventArgs e)
        {
            Kupi(22);
        }
        private void Artical_2_3_Click(object sender, RoutedEventArgs e)
        {
            Kupi(23);
        }
        private void Artical_2_4_Click(object sender, RoutedEventArgs e)
        {
            Kupi(24);
        }
        private void Artical_3_1_Click(object sender, RoutedEventArgs e)
        {
            Kupi(31);
        }
        private void Artical_3_2_Click(object sender, RoutedEventArgs e)
        {
            Kupi(32);
        }
        private void Artical_3_3_Click(object sender, RoutedEventArgs e)
        {
            Kupi(33);
        }
        private void Artical_3_4_Click(object sender, RoutedEventArgs e)
        {
            Kupi(34);
        }
    }
}
