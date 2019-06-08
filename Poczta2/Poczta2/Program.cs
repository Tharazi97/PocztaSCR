﻿using System;
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

            Przesylka przykladowa;
            przykladowa.masa = 5.0f;
            przykladowa.miasto = 'E';
            przykladowa.typ = false;

            List<Dostawczak> dostawczaks = new List<Dostawczak>();
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'A';
            for(int i = 0; i < 35 ; i++)
            {
                dostawczaks.Last().zaladunek.Enqueue(przykladowa);
            }
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'B';
            for (int i = 0; i < 35; i++)
            {
                dostawczaks.Last().zaladunek.Enqueue(przykladowa);
            }
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'C';
            for (int i = 0; i < 35; i++)
            {
                dostawczaks.Last().zaladunek.Enqueue(przykladowa);
            }
            dostawczaks.Add(new Dostawczak());
            dostawczaks.Last().miasto = 'D';
            for (int i = 0; i < 35; i++)
            {
                dostawczaks.Last().zaladunek.Enqueue(przykladowa);
            }
            //dostawczaks.Add(new Dostawczak());
            //dostawczaks.Last().miasto = 'E';

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
            //kierowniczka.dostawczakiDoZaladunku = dostawczaks;
            kierowniczka.dostawczakiDoRozladunku = dostawczaks;
            kierowniczka.miasto = 'E';
            Thread kier = new Thread(new ThreadStart(kierowniczka.Czuwaj));
            kier.Start();
            Random rnd = new Random();
            DateTime time = new DateTime();
            bool dziewiata = false;
            while (true)
            {
                //time = DateTime.Now;
                //if ((time.Minute % 10 == 1) && !dziewiata)
                //{
                //    kierowniczka.dostawczakiDoZaladunku = dostawczaks;
                //}

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

                //Console.WriteLine(" " + okienka[0].skrzynka.Count + " " + okienka[1].skrzynka.Count + " " + skrzynkas[0].zaladunek.Count);
                if(kierowniczka.dostawczakiDoZaladunku.Count==4)
                    Console.Write(" A:" + dostawczaks.ElementAt(0).zaladunek.Count + " B:" + dostawczaks.ElementAt(1).zaladunek.Count + " C:" + dostawczaks.ElementAt(2).zaladunek.Count + " D:" + dostawczaks.ElementAt(3).zaladunek.Count);
                if (kierowniczka.dostawczakiDoRozladunku.Count == 4)
                    Console.Write(" RA:" + dostawczaks.ElementAt(0).zaladunek.Count + " RB:" + dostawczaks.ElementAt(1).zaladunek.Count + " RC:" + dostawczaks.ElementAt(2).zaladunek.Count + " RD:" + dostawczaks.ElementAt(3).zaladunek.Count);
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
