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
        public Mutex mutKlienci = new Mutex();
        public List<Pracownik> pracownicy = new List<Pracownik>();
        public List<Thread> threads = new List<Thread>();
        public Kierowniczka kierowniczka;
        public Thread kier;

        public static List<Miasto> miasta = new List<Miasto>();

        Random rnd = new Random();
        DateTime time = new DateTime();
        bool dziewiata = false;
        bool szesnasta = false;
        bool siedemnasta = false;

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

            kierowniczka = new Kierowniczka(okienka, skrzynki);
            kierowniczka.pracownicy = pracownicy;
            kierowniczka.miasto = nazwa;
            kierowniczka.klienci = klienci;
            kierowniczka.dostawczakiDoRozladunku = dostawczakiDoRozladunku;
            kierowniczka.dostawczakiDoZaladunku = dostawczakiDoZaladunku;

            kier = new Thread(new ThreadStart(kierowniczka.Czuwaj));
            lock(miasta)
            {
                miasta.Add(this);
            }
        }

        public void Symuluj()
        {
            while (DateTime.Now.Minute % 10 != 4);
            while (true)
            {
                time = DateTime.Now;

                if ((time.Minute % 10 == 4) && !dziewiata)// jest 9 otwieramy poczte
                {
                    siedemnasta = false;
                    szesnasta = false;
                    dziewiata = true;
                    int ilosc = rnd.Next(3, 8);
                    for (int i = 0; i < ilosc; i++)
                    {
                        pracownicy.Add(new Pracownik(klienci, mutKlienci, mutSorto, skrzynki, okienka));
                        threads.Add(new Thread(new ThreadStart(pracownicy.Last().Pracuj)));
                        threads.Last().Start();
                    }

                    //kierowniczka.DostawczakiDoRozladunku = dostawczakiDoRozladowania;
                    kier.Start();
                }



                if ((rnd.Next(10) == 0) && (time.Minute % 10 > 0) && (time.Minute % 10 < 6))
                {
                    mutKlienci.WaitOne();
                    klienci.Enqueue(new Klient());
                    mutKlienci.ReleaseMutex();
                }

                Thread.Sleep(200);

            }
        }

    }
}
