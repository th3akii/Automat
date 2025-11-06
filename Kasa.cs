using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vending_Machine
{
    public class Kasa
    {
        public int kredit;
        private int[,] novcanice;
        public int ukupno { get; private set; }

        public Kasa()
        {
            kredit = 0;
            novcanice = new int[,]
            {
                {10,  5},
                {20,  5},
                {50,  2},
                {100, 2},
                {200, 1},
                {500, 0}
            };
            ukupno = 650;
        }

        private int pronadjiRed(int novcanica)
        {
            for (int i = 0; i < novcanice.GetLength(0); i++)
            {
                if (novcanice[i, 0] == novcanica)
                    return i;
            }
            return -1;
        }

        public void DodajNovcanice(int novcanica)
        {
            int row = pronadjiRed(novcanica);
            if (row >= 0)
            {
                novcanice[row, 1]++;
                kredit += novcanica;
                AzurirajMainWindow(kredit);
            }
            else
            {
                MessageBox.Show("Nepoznat novcanica!");
            }

            ukupno += novcanica;
        }

        public void DodajNovcaniceAdmin(int novcanica)
        {
            int row = pronadjiRed(novcanica);
            if (row >= 0)
            {
                novcanice[row, 1]++;
            }
            else
            {
                MessageBox.Show("Nepoznat novcanica!");
            }

            ukupno += novcanica;
        }

        public void UkloniNovcaniceAdmin(int novcanica)
        {
            int row = pronadjiRed(novcanica);
            if (row >= 0)
            {
                novcanice[row, 1]--;
            }
            else
            {
                MessageBox.Show("Nepoznat novcanica!");
            }

            ukupno -= novcanica;
        }

        private void AzurirajMainWindow(double novac)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.AzurirajStanje(novac);
                    break;
                }
            }
        }

        public int getKolicina(int novcanica)
        {
            int row = pronadjiRed(novcanica);
            if (row >= 0)
                return novcanice[row, 1];
            return 0;
        }

        //kusur: https://larry-skola.notion.site/kusur
        public void vratiKusur()
        {
            int cilj = kredit;
            if (cilj <= 0) return;
            napraviMatricu(cilj);
            int isplaceno = prikaziResenje(cilj);
            ukupno -= isplaceno;
            if (ukupno < 0) ukupno = 0;
            AzurirajMainWindow(kredit);
        }

        private int[,] matrica = new int[6, 1000];
        private int[,] iskoriscene = new int[6, 1000];

        private void napraviMatricu(int cilj)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j <= cilj; j += 10)
                {
                    matrica[i, j] = int.MaxValue;
                }
                matrica[i, 0] = 0;
            }

            for (int j = 10; j <= cilj; j += 10)
            {
                if (j / 10 <= novcanice[0, 1])
                {
                    matrica[0, j] = j / 10;
                    iskoriscene[0, j] = j / 10;
                }
            }

            for (int i = 1; i < 6; i++)
            {
                int vrednost = novcanice[i, 0];
                int maxKolicina = novcanice[i, 1];

                for (int j = 10; j <= cilj; j += 10)
                {
                    matrica[i, j] = matrica[i - 1, j];
                    iskoriscene[i, j] = 0;

                    int maksimalnoMoze = Math.Min(maxKolicina, j / vrednost);
                    if (maksimalnoMoze == 0)
                    {
                        continue;
                    }

                    for (int k = 1; k <= maksimalnoMoze; k++)
                    {
                        int preostaliIznos = j - k * vrednost;
                        int prethodnoResenje = matrica[i - 1, preostaliIznos];

                        if (prethodnoResenje == int.MaxValue)
                        {
                            continue;
                        }

                        int novoResenje = prethodnoResenje + k;
                        if (novoResenje < matrica[i, j])
                        {
                            matrica[i, j] = novoResenje;
                            iskoriscene[i, j] = k;
                        }
                    }
                }
            }
        }

        private int prikaziResenje(int cilj)
        {
            int i = 5;
            int j = cilj;

            while (j >= 0 && matrica[i, j] == int.MaxValue)
            {
                j -= 10;
            }

            if (j < 0)
            {
                MessageBox.Show("Nijedan iznos nije moguće isplatiti zadatim novčanicama.");
                return 0;
            }

            if (j != cilj)
            {
                MessageBox.Show($"Nije moguće isplatiti {cilj}. Najbliži manji iznos je {j}.");
            }

            int[] rezultat = new int[6];
            int preostaliIznos = ZaokruziNaDeset(j);
            int red = 5;

            while (red >= 0 && preostaliIznos > 0)
            {
                if (matrica[red, preostaliIznos] == int.MaxValue)
                {
                    red--;
                    continue;
                }

                int k = iskoriscene[red, preostaliIznos];
                if (k == 0)
                {
                    red--;
                    continue;
                }

                rezultat[red] = k;
                preostaliIznos -= k * novcanice[red, 0];
                red--;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Vraćen kusur");

            int isplaceno = 0;
            for (int f = 5; f >= 0; f--)
            {
                if (rezultat[f] > 0)
                {
                    sb.AppendLine($"{rezultat[f]} × {novcanice[f, 0]} RSD");
                    novcanice[f, 1] -= rezultat[f];
                    isplaceno += rezultat[f] * novcanice[f, 0];
                }
            }

            kredit -= isplaceno;
            if (kredit < 0) kredit = 0;

            MessageBox.Show(sb.ToString());
            return isplaceno;
        }

        private static int ZaokruziNaDeset(int n)
        {
            int temp = n % 10;
            return n - temp;
        }
    }
}
