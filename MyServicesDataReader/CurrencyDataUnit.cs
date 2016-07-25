using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServicesDataReader
{
    public class CurrencyDataUnit
    {
        public string date { get; set; }//дата измерения
        public string time { get; set; }//время измерения
        public string name { get; set; }//Код валют:USD/RUB/EUR etc.
        public string price { get; set; }
        public string source { get; set; }//сайт источник

        public CurrencyDataUnit(string cur, string price, string date, string time, string source)
        {
            this.name = cur;
            this.price = price;
            this.date = date;
            this.time = time;
            this.source = source;
        }
    }
}