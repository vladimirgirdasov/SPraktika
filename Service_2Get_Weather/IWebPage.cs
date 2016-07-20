using System.Collections.Generic;

namespace Service_2Get_Weather
{
    internal interface IWebPage
    {
        string Address { get; set; }

        void Read();

        bool InReading { get; set; }

        string Show();

        string NameOfResource { get; }
    }
}