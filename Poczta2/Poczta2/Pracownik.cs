﻿using System;
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
        public volatile Zajety coRobie = Zajety.wolny;
        public Mutex mutSort;
        public Skrzynka skrzynkaDoRozladunku;
        public Dostawczak dostawczakDoZaladunku;
        public Dostawczak dostawczakDoRozladunku;

        public Pracownik(Queue<Klient> klienci, Mutex mutKlienci, Mutex mutSort, Skrzynka[] skrzynki, Okienko[] okienka)
        {
            this.klienci = klienci;
            this.mutKlienci = mutKlienci;
            this.mutSort = mutSort;
            this.skrzynki = skrzynki;
            this.okienka = okienka;
        }

        public void PrzyjmijPrzesylke()
        {
            coRobie = Zajety.przyjmuje;
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
            coRobie = Zajety.zamknijOkienko;
            if(okienko != null)
            {
                okienko.zajete = false;
                Thread.MemoryBarrier();
                okienko = null;
                Thread.MemoryBarrier();
            }
            else
                coMaszRobic = Zajety.wolny;
        }

        public void Sortuj()
        {
            coRobie = Zajety.sortuje;
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
            coRobie = Zajety.laduje;
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
                skrzynkaDoRozladunku = null;
                dostawczakDoZaladunku = null;
                Thread.MemoryBarrier();
                coMaszRobic = Zajety.wolny;
            }

        }

        public void RozladujDostawczak()
        {
            coRobie = Zajety.rozladowuje;
            dostawczakDoRozladunku.mutDos.WaitOne();
            if(dostawczakDoRozladunku.zaladunek.Count!=0)
            {
                Thread.Sleep(100);
                Przesylka tymczasowa = dostawczakDoRozladunku.zaladunek.Dequeue();
                dostawczakDoRozladunku.mutDos.ReleaseMutex();
                Thread.Sleep(100 + (int)(tymczasowa.masa * 10));
            }
            else
            {
                dostawczakDoRozladunku.mutDos.ReleaseMutex();
                dostawczakDoRozladunku = null;
                Thread.MemoryBarrier();
                coMaszRobic = Zajety.wolny;
            }
        }

        public void Pracuj()
        {
            while(coMaszRobic != Zajety.zamknij)
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
                    case Zajety.wolny:
                        coRobie = Zajety.wolny;
                        Thread.Sleep(100);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
