using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Miasto
    {
        public char nazwa;
        public Queue<Klient> klienci = new Queue<Klient>();
        public List<Dostawczak> dostawczakiDoRozladunku = new List<Dostawczak>();
        public List<Dostawczak> dostawczakiDoZaladunku = new List<Dostawczak>();
        public Skrzynka[] skrzynki;
        public Okienko[] okienka;
        public Mutex mutSorto = new Mutex();
        public List<Pracownik> pracownicy = new List<Pracownik>();
        public List<Thread> threads = new List<Thread>();
        public Kierowniczka kierowniczka;
        public Thread kier;

        public static List<Miasto> miasta = new List<Miasto>();

        Random rnd = new Random();
        DateTime time = new DateTime();
        bool dziewiata = false;

        public Miasto(char nazwa, int iloscOkienek, int iloscMiast)
        {
            this.nazwa = nazwa;

            skrzynki = new Skrzynka[iloscMiast];
            for(int i=0; i<iloscMiast; i++)
            {
                skrzynki[i] = new Skrzynka();
                skrzynki[i].miasto = (char)(i + 65);
            }

            okienka = new Okienko[iloscOkienek];
            for(int i=0; i<iloscOkienek; i++)
            {
                okienka[i] = new Okienko();
            }
        }

    }
}
