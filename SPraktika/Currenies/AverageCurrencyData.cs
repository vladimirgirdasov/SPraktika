using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPraktika
{
    public static class AverageCurrencyData
    {
        public static List<CurrencyRating> CalcAverageRates(params List<CurrencyRating>[] resources)
        {
            var ans = new List<CurrencyRating>();
            //Создается набор всех возможных валют:
            var ABC = new HashSet<string>();
            foreach (var resource in resources)
            {
                foreach (var cur in resource)
                {
                    ABC.Add(cur.cur);
                }
            }
            //Считается среднее по всем источникам:
            foreach (var currency in ABC)
            {
                double sum = 0;
                int count = 0;
                var buf = new CurrencyRating();
                //Считается Сумма цен валют и количество контрибутеров
                foreach (var resource in resources)
                {
                    var price = (from line in resource
                                 where line.cur == currency
                                 select line.val)
                                .ToList();
                    if (price.Count != 0)
                    {
                        sum += Convert.ToDouble(price.First());
                        count++;
                    }
                }

                buf.cur = currency;
                buf.val = (sum / (double)count).ToString();
                ans.Add(buf);
            }
            return ans;
        }
    }
}