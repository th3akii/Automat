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

namespace Vending_Machine
{
    /// <summary>
    /// Interaction logic for AddMoney.xaml
    /// </summary>
    public partial class AddMoney : Window
    {
        private Kasa kasa;
        public AddMoney(Kasa kasa)
        {
            InitializeComponent();
            this.kasa = kasa;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Money_10_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(10);
        }

        private void Money_20_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(20);
        }

        private void Money_50_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(50);
        }

        private void Money_100_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(100);
        }
        private void Money_200_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(200);
        }

        private void Money_500_Click(object sender, RoutedEventArgs e)
        {
            kasa.DodajNovcanice(500);
        }
    }
}
