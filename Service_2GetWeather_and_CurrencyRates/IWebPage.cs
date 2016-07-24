using System.Collections.Generic;

namespace Service_2Get_CurrencyRates
{
    public interface IWebPage
    {
        string Address { get; }

        void Read(object ABC = null);

        DataCurrencySet Read();

        System.Threading.Tasks.Task<DataCurrencySet> ReadAsync();

        bool InReading { get; set; }
    }
}