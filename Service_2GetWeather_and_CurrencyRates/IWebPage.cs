﻿using System.Collections.Generic;

namespace Service_2Get_CurrencyRates
{
    internal interface IWebPage
    {
        string Address { get; }

        void Read(object ABC = null);

        bool InReading { get; set; }

        string NameOfResource { get; }

        string Show();
    }
}