using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vending_Machine
{
    public class Artikal
    {
        public int ID;
        public string ime;
        public DateTime rokTrajanja;
        public double tezina;
        public double cena;
        public DayOfWeek dan;
        public double popust;

        public static List<Queue<Artikal>> artikli;

        static Artikal()
        {
            artikli = new List<Queue<Artikal>>();
            //3x4 grid
            for (int i = 0; i < 12; i++)
            {
                artikli.Add(new Queue<Artikal>());
            }
        }

        public Artikal(int id, string ime, DateTime rokTrajanja, double tezina, double cena, DayOfWeek dan, double popust)
        {

            this.ID = id;
            this.ime = ime;
            this.rokTrajanja = rokTrajanja;
            this.tezina = tezina;
            this.dan = dan;
            this.popust = popust;
            this.cena = calculatePrice(cena);

            DodajArtikalUSlot(this);
        }

        public double calculatePrice(double cena)
        {
            if (DateTime.Today.DayOfWeek == this.dan)
            {
                return cena - (cena * popust / 100);
            }
            return cena;
        }

        private static void DodajArtikalUSlot(Artikal artikal)
        {
            int row = artikal.ID / 10;
            int col = artikal.ID % 10;

            int index = (row - 1) * 4 + (col - 1);

            if (index >= 0 && index < artikli.Count)
            {
                artikli[index].Enqueue(artikal);
            }
        }

        public static Artikal GetArtikalIzSlota(int id)
        {
            int row = id / 10;
            int col = id % 10;
            int index = (row - 1) * 4 + (col - 1);

            if (index >= 0 && index < artikli.Count && artikli[index].Count > 0)
            {
                return artikli[index].Peek();
            }
            return null;
        }

        public static Artikal UkloniArtikalIzSlota(int id)
        {
            int row = id / 10;
            int col = id % 10;
            int index = (row - 1) * 4 + (col - 1);

            if (index >= 0 && index < artikli.Count && artikli[index].Count > 0)
            {
                return artikli[index].Dequeue();
            }
            return null;
        }

        public static DateTime? GetPoslednjiDatum(int id)
        {
            int row = id / 10;
            int col = id % 10;
            int index = (row - 1) * 4 + (col - 1);
            if (index < 0 || index >= artikli.Count || artikli[index].Count == 0) return null;

            return artikli[index].Last().rokTrajanja;
        }
    }
}
