using System;
using System.Collections;
using System.Collections.Generic;

namespace SPraktika
{
    internal class CurrencyData : IEnumerable
    {
        public Dictionary<string, double> CurrencyRates;

        public HashSet<string> abc;//коллекция валют по всем источникам

        public struct Rating
        {
            public string Currency { get; set; }
            public string Rate { get; set; }
        }

        public void CalcAverageCurrencyRate(params CurrencyData[] mas)
        {
            foreach (var cur_abc in this.abc)
            {
                int count_of_contributors = 0;
                double sum = 0;
                for (int contributor = 0; contributor < mas.Length; contributor++)
                {
                    if (mas[contributor].CurrencyRates.ContainsKey(cur_abc))
                    {
                        sum += mas[contributor].CurrencyRates[cur_abc];
                        count_of_contributors++;
                    }
                }
                this.CurrencyRates.Add(cur_abc, sum / (double)count_of_contributors);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)abc).GetEnumerator();
        }

        public CurrencyData()
        {
            abc = new HashSet<string>();
            CurrencyRates = new Dictionary<string, double>();
        }
    }
}