using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Kierowniczka
    {
        public List<Pracownik> pracownicy;
        public Queue<Klient> klienci;
        public Okienko[] okienka;
        public int otwarteOkienka = 0;
        public Skrzynka[] skrzynki;

        public Kierowniczka(Okienko[] oki, Skrzynka[] skr)
        {
            okienka = oki;
            skrzynki = skr;
        }

        public void SprawdzKolejke()
        {
            if ((klienci.Count / 3.0f <= okienka.Count()) && (klienci.Count / 3.0f > otwarteOkienka) && (pracownicy.Count() > otwarteOkienka))
            {
                Console.WriteLine("probuje otworzyc");
                Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic != Zajety.przyjmuje && x.coMaszRobic != Zajety.zamknijOkienko);
                if (tymczasowy != null)
                {
                    if(okienka[otwarteOkienka].zajete== true)
                    {
                        Console.WriteLine("musze poczekac");               
                    }
                    else
                    {
                        Console.WriteLine("otworzylAM");
                        okienka[otwarteOkienka].zajete = true;
                        tymczasowy.okienko = okienka[otwarteOkienka];
                        Thread.MemoryBarrier();
                        otwarteOkienka++;
                        tymczasowy.coMaszRobic = Zajety.przyjmuje;
                    }
                }
            }
            if ((klienci.Count / 3.0f > okienka.Count()) && (otwarteOkienka < okienka.Count()) && (pracownicy.Count() > otwarteOkienka))
            {
                for (int i = otwarteOkienka; i < okienka.Count(); i++)
                {
                    if (okienka[i].zajete == false)
                    {
                        Console.WriteLine("probuje otworzyc2");
                        Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic != Zajety.przyjmuje && x.coMaszRobic != Zajety.zamknijOkienko);
                        if (tymczasowy != null)
                        {
                            Console.WriteLine("otworzylAM2");
                            okienka[otwarteOkienka].zajete = true;
                            tymczasowy.okienko = okienka[otwarteOkienka];
                            Thread.MemoryBarrier();
                            otwarteOkienka++;
                            tymczasowy.coMaszRobic = Zajety.przyjmuje;
                        }
                    }
                }
            }
            if ((klienci.Count() < otwarteOkienka) && (otwarteOkienka > 0))
            {
                Console.WriteLine("probuje zamknac");
                Pracownik tymczasowy = pracownicy.Find(x => x.okienko == okienka[otwarteOkienka - 1]);
                if (tymczasowy != null)
                {
                    Console.WriteLine("zamknelam");
                    tymczasowy.coMaszRobic = Zajety.zamknijOkienko;
                    otwarteOkienka--;
                }
            }
        }

        public void SprawdzCzySortowac()
        {
            //Console.WriteLine("sprawdzam czy sortowac");
            Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic == Zajety.wolny);
            if(tymczasowy != null)
            {
                for (int i = 0; i < okienka.Count(); i++)
                {
                    if (okienka[i].skrzynka.Count != 0)
                    {
                        tymczasowy.coMaszRobic = Zajety.sortuje;
                        break;
                    }
                }
            }
            
        }

        public void Czuwaj()
        {
            while(true)
            {
                SprawdzKolejke();
                SprawdzCzySortowac();
            }
            
        }
    }
}
