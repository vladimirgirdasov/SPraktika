using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPraktika
{
    internal class CurrencyData
    {
        public Dictionary<string, double> CurrencyRates;

        public struct Rating
        {
            public string Name { get; set; }
            public string Price { get; set; }
        }
    }
}