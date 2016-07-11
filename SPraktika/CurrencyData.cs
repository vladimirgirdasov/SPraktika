using System.Collections.Generic;

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