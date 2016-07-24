namespace Service_2Get_CurrencyRates
{
    public class CurrencyRating
    {
        public string cur;
        public string val;

        public CurrencyRating(string nameOfCurrency, string priceOfCurrency)
        {
            cur = nameOfCurrency;
            val = priceOfCurrency;
        }

        public CurrencyRating()
        {
        }
    }
}