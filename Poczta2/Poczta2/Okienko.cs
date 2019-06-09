using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Okienko
    {
        public Queue<Przesylka> skrzynka = new Queue<Przesylka>();
        public Mutex mutOkienko = new Mutex();
        public Klient klient;
        public UInt32 klientID = 0;
        public bool zajete = false;
        //public int ID;
    }
}
