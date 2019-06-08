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
        public char miasto;
        public List<Pracownik> pracownicy;
        public Queue<Klient> klienci;
        public Okienko[] okienka;
        public int otwarteOkienka = 0;
        public Skrzynka[] skrzynki;
        public List<Dostawczak> dostawczakiDoZaladunku = new List<Dostawczak>();
        public List<Dostawczak> dostawczakiDoRozladunku = new List<Dostawczak>();
        public bool zamknij = false;
        public bool koniec = false;

        public Kierowniczka(Okienko[] oki, Skrzynka[] skr)
        {
            okienka = oki;
            skrzynki = skr;
        }

        public void SprawdzKolejke()
        {
            if ((klienci.Count / 3.0f <= okienka.Count()) && (klienci.Count / 3.0f > otwarteOkienka) && (pracownicy.Count() > otwarteOkienka))
            {
                //Console.WriteLine("probuje otworzyc");
                Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic != Zajety.przyjmuje && x.coMaszRobic != Zajety.zamknijOkienko);
                if (tymczasowy != null)
                {
                    tymczasowy.coMaszRobic = Zajety.wolny;
                    while (tymczasowy.coRobie != Zajety.wolny) ;
                    if(okienka[otwarteOkienka].zajete== true)
                    {
                        //Console.WriteLine("musze poczekac");               
                    }
                    else
                    {
                        //Console.WriteLine("otworzylAM");
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
                        //Console.WriteLine("probuje otworzyc2");
                        Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic != Zajety.przyjmuje && x.coMaszRobic != Zajety.zamknijOkienko);
                        if (tymczasowy != null)
                        {
                            tymczasowy.coMaszRobic = Zajety.wolny;
                            while (tymczasowy.coRobie != Zajety.wolny) ;
                            //Console.WriteLine("otworzylAM2");
                            tymczasowy.okienko = okienka[otwarteOkienka];
                            okienka[otwarteOkienka].zajete = true;
                            Thread.MemoryBarrier();
                            otwarteOkienka++;
                            tymczasowy.coMaszRobic = Zajety.przyjmuje;
                        }
                    }
                }
            }
            if ((klienci.Count() < otwarteOkienka) && (otwarteOkienka > 0))
            {

                //Console.WriteLine("probuje zamknac");
                Pracownik tymczasowy = pracownicy.Find(x => x.okienko == okienka[otwarteOkienka - 1]);
                if (tymczasowy != null)
                {
                    tymczasowy.coMaszRobic = Zajety.wolny;
                    while (tymczasowy.coRobie != Zajety.wolny) ;
                    //Console.WriteLine("zamknelam");
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

        public void SprawdzCzyZaladowac()
        {
            //Console.WriteLine("sprawdzam czy zaladowac");
            if(dostawczakiDoZaladunku.Count!=0)
            {
                Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic != Zajety.przyjmuje && x.coMaszRobic != Zajety.zamknijOkienko && x.coMaszRobic != Zajety.laduje && x.coMaszRobic != Zajety.rozladowuje);
                if (tymczasowy != null)
                {
                    for (int i = 0; i < skrzynki.Length; i++)
                    {
                        if(skrzynki[i].miasto!=miasto)
                        {
                            if (skrzynki[i].zaladunek.Count != 0)
                            {
                                try
                                {
                                    tymczasowy.skrzynkaDoRozladunku = skrzynki[i];
                                    tymczasowy.dostawczakDoZaladunku = dostawczakiDoZaladunku.Find(x => x.miasto == skrzynki[i].miasto);
                                    Thread.MemoryBarrier();
                                    tymczasowy.coMaszRobic = Zajety.laduje;
                                }
                                finally { }
                                break;
                            }
                        }
                    }
                }
            }
            

        }

        public void SprawdzCzyRozladowac()
        {
            if (dostawczakiDoRozladunku.Count != 0)
            {
                Pracownik tymczasowy = pracownicy.Find(x => x.coMaszRobic == Zajety.wolny || x.coMaszRobic == Zajety.laduje || x.coMaszRobic == Zajety.sortuje);
                if(tymczasowy != null)
                {
                    foreach(var dost in dostawczakiDoRozladunku)
                    {
                        if(dost.zaladunek.Count != 0)
                        {
                            try
                            {
                                tymczasowy.dostawczakDoRozladunku = dost;
                                Thread.MemoryBarrier();
                                tymczasowy.coMaszRobic = Zajety.rozladowuje;
                            }
                            finally { }
                            break;
                        }
                    }
                }
            }
        }

        public void Czuwaj()
        {
            while(!koniec)
            {
                if(zamknij)
                {
                    if(dostawczakiDoRozladunku.Any(x => x.zaladunek.Count !=0))
                    {
                        SprawdzCzyRozladowac();
                    }
                    else
                    {
                        dostawczakiDoRozladunku.Clear();
                        foreach (var prac in pracownicy)
                        {
                            prac.coMaszRobic = Zajety.zamknij;
                        }
                    }
                }
                else
                {
                    SprawdzKolejke();
                    SprawdzCzySortowac();
                    SprawdzCzyZaladowac();
                    SprawdzCzyRozladowac();
                    if(dostawczakiDoRozladunku.Count !=0)
                    {
                        if (dostawczakiDoRozladunku.All(x => x.zaladunek.Count == 0))
                        {
                            dostawczakiDoRozladunku.Clear();
                        }
                    }
                    
                }
                Thread.Sleep(100);
            }
            
        }
    }
}
