﻿using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace SPraktika
{
    partial class YandexCities : IWebPage
    {
        private string city_id;
        private string city;

        public string City_id { get { return city_id; } }
        public string City { get { return city; } }

        public YandexCities()
        {
            city = "";
            city_id = "";
        }

        public string Address
        {
            get { return "https://pogoda.yandex.ru/static/cities.xml"; }
        }

        public bool InReading
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public void Read(object city)//city - запрашиваемый город string
        {
            try
            {
                XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));
                IEnumerable<XElement> elements = xdoc.Descendants("country");
                string id = "";
                foreach (XElement item in elements)
                {
                    var ans = from xe in item.Elements("city")
                              where xe.Value == (string)city
                              select xe.Attribute("region");
                    if (ans.Count() != 0)
                        id += ans.First().Value;
                }

                if (id == "")
                    MessageBox.Show("Информация по городу не предоставлется или город введен не корректно", "Ошибка");
                else
                {
                    MessageBox.Show("Ок. Город определен (" + id + ")");
                    this.city = (string)city;
                    city_id = id;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
            }
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}