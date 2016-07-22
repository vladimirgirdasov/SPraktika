using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPraktika
{
    public interface IWebPage
    {
        string Address { get; }

        void Read(object ABC = null);

        List<CurrencyRating> Read();

        System.Threading.Tasks.Task<List<CurrencyRating>> ReadAsync();

        bool InReading { get; set; }
    }
}