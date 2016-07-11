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
            public string Currency { get; set; }
            public string Rate { get; set; }
        }
    }
}