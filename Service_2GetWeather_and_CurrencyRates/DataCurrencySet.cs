using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_2Get_CurrencyRates
{
    public class DataCurrencySet
    {
        public List<CurrencyRating> data;
        public string SourceName;

        public DataCurrencySet(List<CurrencyRating> dat, string src)
        {
            data = dat;
            SourceName = src;
        }

        public DataCurrencySet()
        {
        }
    }
}