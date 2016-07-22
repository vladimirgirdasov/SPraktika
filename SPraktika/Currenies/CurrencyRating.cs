using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPraktika
{
    public class CurrencyRating
    {
        public string cur;
        public string val;

        public CurrencyRating(string nameOfCurrency, string priceOfCurrency)
        {
            cur = nameOfCurrency;
            val = priceOfCurrency;
        }

        public CurrencyRating()
        {
        }
    }
}