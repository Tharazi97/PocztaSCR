using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Klient
    {
        public static UInt32 ilosc = 0;
        public static Mutex mut;
        public UInt32 ID;
        public Przesylka przesylka;
        static Random rnd = new Random();
        public static int iloscMiast = 5;
        
        public Klient()
        {
            ID = ++ilosc;
            przesylka.typ = Convert.ToBoolean(rnd.Next(2));
            if (przesylka.typ)
                przesylka.masa = 0;
            else
                przesylka.masa = rnd.Next(200) / 10.0f;
            przesylka.miasto = (char)rnd.Next(65, (65 + iloscMiast));
        }
    }
}
