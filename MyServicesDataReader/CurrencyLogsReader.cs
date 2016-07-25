using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyServicesDataReader
{
    public static class CurrencyLogsReader
    {
        public static List<CurrencyDataUnit> Read(string[] log_paths)
        {
            List<CurrencyDataUnit> ans = new List<CurrencyDataUnit>();

            //Проход по логам
            foreach (var path in log_paths)
            {
                //Чтение лога path
                try
                {
                    XDocument xdoc = XDocument.Load(path);
                    IEnumerable<XElement> updates = xdoc.Descendants("update");

                    //Проход по апдейтам
                    foreach (XElement update in updates)
                    {
                        //Дата апдейта
                        var datetime = update.Attribute("date").Value.Split(' ');
                        var cur_date = datetime[0];
                        var cur_time = datetime[1];

                        //Проход по ресурсам
                        IEnumerable<XElement> resources = update.Descendants("currency_data");
                        foreach (var resource in resources)
                        {
                            var cur_resource = resource.Attribute("resource").Value;

                            //Проход по отдельной валюте
                            IEnumerable<XElement> currencies = resource.Descendants("currency");
                            foreach (var currency in currencies)
                            {
                                //название валюты
                                var cur_currency_name = currency.Attribute("name").Value;
                                //цена валюты
                                var cur_price = currency.Value;

                                //Отдельный юнит готов. Суем в ответ
                                ans.Add(new CurrencyDataUnit(cur_currency_name, cur_price, cur_date, cur_time, cur_resource));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(path + "  " + e.Message, "Ошибка чтения лога");
                }
            }
            return ans;
        }
    }
}