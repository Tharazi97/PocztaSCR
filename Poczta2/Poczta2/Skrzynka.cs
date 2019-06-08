using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poczta2
{
    class Skrzynka
    {
        public char miasto;
        public Queue<Przesylka> zaladunek = new Queue<Przesylka>();
    }
}
