﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poczta2
{
    class Skrzynka
    {
        public char miasto;
        public Queue<Przesylka> zaladunek = new Queue<Przesylka>();
        public Mutex mutSkrz = new Mutex();
    }
}
