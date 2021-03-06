﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Service_2Get_CurrencyRates
{
    internal class CurrencyRates_Writer
    {
        public static void CurrencyWrite(string way, params DataCurrencySet[] mas)
        {
            DateTime localDate = DateTime.Now;
            //Если файла нету, создаем
            if (!File.Exists(way))
            {
                XmlTextWriter textWritter = new XmlTextWriter(way, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("head");
                textWritter.WriteEndElement();
                textWritter.Close();
            }

            XmlDocument document = new XmlDocument();
            document.Load(way);

            XmlNode element_update = document.CreateElement("update");
            XmlAttribute attribute_date = document.CreateAttribute("date"); // создаём атрибут
            attribute_date.Value = localDate.ToString(); // устанавливаем значение атрибута
            element_update.Attributes.Append(attribute_date);
            document.DocumentElement.AppendChild(element_update);

            foreach (var item in mas)
            {
                CurrencyInfoOut(document, element_update, item);
            }

            document.Save(way);
            Console.WriteLine("Writing done");
        }

        public static void CurrencyInfoOut(XmlDocument document, XmlNode element_update, DataCurrencySet dataSet)
        {
            XmlNode element_rates_block = document.CreateElement("currency_data");
            element_update.AppendChild(element_rates_block); // указываем родителя

            XmlAttribute attribute_resource = document.CreateAttribute("resource"); // создаём атрибут
            attribute_resource.Value = dataSet.SourceName; // устанавливаем значение атрибута
            element_rates_block.Attributes.Append(attribute_resource); // добавляем атрибут

            foreach (var item in dataSet.data)
            {
                XmlNode element_currency = document.CreateElement("currency"); // даём имя
                element_currency.InnerText = item.val; // и значение
                element_rates_block.AppendChild(element_currency); // и указываем кому принадлежит

                XmlAttribute attribute_curr_name = document.CreateAttribute("name"); // создаём атрибут
                attribute_curr_name.Value = item.cur; // устанавливаем значение атрибута
                element_currency.Attributes.Append(attribute_curr_name); // добавляем атрибут
            }
        }
    }
}