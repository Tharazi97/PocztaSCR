using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    public struct Przesylka
    {
        public float masa;
        public bool typ;
        public char miasto;
    }

    public enum Zajety
    {
        wolny, przyjmuje, laduje, rozladowuje, sortuje, zamknij, zamknijOkienko
    }
    class Program
    {
        static void Main(string[] args)
        {
            Queue<Klient> klients = new Queue<Klient>();
            for (int i = 0; i < 100; i++)
            {
                klients.Enqueue(new Klient());
            }

            List<Dostawczak> dostawczaks = new List<Dostawczak>();
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'A';
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'B';
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'C';
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'D';
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'E';

            Skrzynka[] skrzynkas = { new Skrzynka(), new Skrzynka(), new Skrzynka(), new Skrzynka(), new Skrzynka() };
            skrzynkas[0].miasto = 'A';
            skrzynkas[1].miasto = 'B';
            skrzynkas[2].miasto = 'C';
            skrzynkas[3].miasto = 'D';
            skrzynkas[4].miasto = 'E';

            Okienko[] okienka = { new Okienko(), new Okienko(), new Okienko() };
            okienka[0].ID = 1;
            okienka[1].ID = 2;
            okienka[2].ID = 3;

            Mutex dopa = new Mutex();

            Pracownik pracownik = new Pracownik();
            pracownik.mutKlienci = new Mutex();
            pracownik.mutSort = dopa;
            Pracownik pracownik2 = new Pracownik();
            pracownik2.mutKlienci = pracownik.mutKlienci;
            pracownik2.mutSort = dopa;
            Pracownik pracownik3 = new Pracownik();
            pracownik3.mutKlienci = pracownik.mutKlienci;
            pracownik3.mutSort = dopa;

            pracownik.klienci = klients;
            pracownik2.klienci = klients;
            pracownik3.klienci = klients;

            pracownik.skrzynki = skrzynkas;
            pracownik2.skrzynki = skrzynkas;
            pracownik3.skrzynki = skrzynkas;

            pracownik.okienka = okienka;
            pracownik2.okienka = okienka;
            pracownik3.okienka = okienka;

            List<Pracownik> pracowniks = new List<Pracownik>();
            pracowniks.Add(pracownik);
            pracowniks.Add(pracownik2);
            pracowniks.Add(pracownik3);

            Thread thread = new Thread(new ThreadStart(pracownik.Pracuj));
            Thread thread2 = new Thread(new ThreadStart(pracownik2.Pracuj));
            Thread thread3 = new Thread(new ThreadStart(pracownik3.Pracuj));

            thread.Start();
            thread2.Start();
            thread3.Start();

            Kierowniczka kierowniczka = new Kierowniczka(okienka, skrzynkas);
            kierowniczka.pracownicy = pracowniks;
            kierowniczka.klienci = klients;
            kierowniczka.dostawczakiDoZaladunku = dostawczaks;
            Thread kier = new Thread(new ThreadStart(kierowniczka.Czuwaj));
            kier.Start();
            Random rnd = new Random();
            while(true)
            {
                Thread.Sleep(100);
                if (rnd.Next(15) == 0)
                    klients.Enqueue(new Klient());
                
                if (okienka[0].zajete == true)
                    Console.Write(" S1(o):" + okienka[0].klientID);
                else
                    Console.Write(" S1(z):");

                if (okienka[1].zajete == true)
                    Console.Write(" S2(o):" + okienka[1].klientID);
                else
                    Console.Write(" S2(z):");

                if (okienka[2].zajete == true)
                    Console.Write(" S3(o):" + okienka[2].klientID);
                else
                    Console.Write(" S3(z):");

                Console.Write(" " + okienka[0].skrzynka.Count + " " + okienka[1].skrzynka.Count + " " + skrzynkas[0].zaladunek.Count);
                Console.WriteLine(" A:" + dostawczaks.ElementAt(0).zaladunek.Count + " B:" + dostawczaks.ElementAt(1).zaladunek.Count);

            }
            Console.ReadKey();
        }
    }
}
