using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Pracownik
    {
        public Okienko okienko;
        public Okienko[] okienka;
        public Mutex mutKlienci;
        public Queue<Klient> klienci;
        public Skrzynka[] skrzynki;
        public volatile Zajety coMaszRobic = Zajety.wolny;
        public Mutex mutSort;
        public Skrzynka skrzynkaDoRozladunku;
        public Dostawczak dostawczakDoZaladunku;
        public Dostawczak DostawczakDoRozladunku;


        public void PrzyjmijPrzesylke()
        {

            bool przyjal = false;
            mutKlienci.WaitOne();
            if (klienci.Count != 0)
            {
                okienko.klient = klienci.Dequeue();
                okienko.klientID = okienko.klient.ID;
                Thread.Sleep(100);
                przyjal = true;
            }
            mutKlienci.ReleaseMutex();
            if (przyjal)
            {
                Thread.Sleep(100 + (int)(okienko.klient.przesylka.masa * 10));
                okienko.mutOkienko.WaitOne();
                if(okienko.klientID!=0)
                    okienko.skrzynka.Enqueue(okienko.klient.przesylka);
                Thread.Sleep(100);
                Thread.MemoryBarrier();
                okienko.klientID = 0;
                okienko.mutOkienko.ReleaseMutex();
            }
            
        }

        public void ZamknijOkienko()
        {
            okienko.zajete = false;
            Thread.MemoryBarrier();
            okienko = null;
            coMaszRobic = Zajety.wolny;
        }

        public void Sortuj()
        {
            Okienko tymczasowe = null;
            Przesylka tymczasowa;
            mutSort.WaitOne();
            for(int i = 0; i < okienka.Count(); i++)
            {
                if(okienka[i].skrzynka.Count != 0)
                {
                    tymczasowe = okienka[i];
                    break;
                }
            }
            if (tymczasowe != null)
            {
                tymczasowe.mutOkienko.WaitOne();
                tymczasowa = tymczasowe.skrzynka.Dequeue();
                tymczasowe.mutOkienko.ReleaseMutex();
                mutSort.ReleaseMutex();
                Thread.Sleep(100 + (int)(tymczasowa.masa*10));
                for (int i = 0; i < skrzynki.Length; i++)
                {
                    if (skrzynki[i].miasto == tymczasowa.miasto)
                    {
                        skrzynki[i].mutSkrz.WaitOne();
                        skrzynki[i].zaladunek.Enqueue(tymczasowa);
                        skrzynki[i].mutSkrz.ReleaseMutex();
                        break;
                    }
                }

            }
            else
            {
                coMaszRobic = Zajety.wolny;
                mutSort.ReleaseMutex();
            }
            
            
        }

        public void LadujDostawczak()
        {
            Przesylka tymczasowa;
            skrzynkaDoRozladunku.mutSkrz.WaitOne();
            if(skrzynkaDoRozladunku.zaladunek.Count!=0)
            {
                tymczasowa = skrzynkaDoRozladunku.zaladunek.Dequeue();
                Thread.Sleep(100);
                skrzynkaDoRozladunku.mutSkrz.ReleaseMutex();

                Thread.Sleep(100 + (int)(tymczasowa.masa * 30));

                dostawczakDoZaladunku.mutDos.WaitOne();
                dostawczakDoZaladunku.zaladunek.Enqueue(tymczasowa);
                dostawczakDoZaladunku.mutDos.ReleaseMutex();
            }
            else
            {
                skrzynkaDoRozladunku.mutSkrz.ReleaseMutex();
                coMaszRobic = Zajety.wolny;
            }

        }

        public void RozladujDostawczak()
        {
            DostawczakDoRozladunku.mutDos.WaitOne();
            if(DostawczakDoRozladunku.zaladunek.Count!=0)
            {
                Thread.Sleep(100);
                Przesylka tymczasowa = DostawczakDoRozladunku.zaladunek.Dequeue();
                DostawczakDoRozladunku.mutDos.ReleaseMutex();
                Thread.Sleep(100 + (int)(tymczasowa.masa * 10));
            }
            else
            {
                DostawczakDoRozladunku.mutDos.ReleaseMutex();
                coMaszRobic = Zajety.wolny;
            }
        }

        public void Pracuj()
        {
            while(true)
            {
                switch (coMaszRobic)
                {
                    case Zajety.przyjmuje:
                        PrzyjmijPrzesylke();
                        break;
                    case Zajety.zamknijOkienko:
                        ZamknijOkienko();
                        break;
                    case Zajety.sortuje:
                        Sortuj();
                        break;
                    case Zajety.laduje:
                        LadujDostawczak();
                        break;
                    case Zajety.rozladowuje:
                        RozladujDostawczak();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
