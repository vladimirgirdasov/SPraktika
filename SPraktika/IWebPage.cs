using System.Collections.Generic;

namespace SPraktika
{
    internal interface IWebPage
    {
        string Address { get; }

        void Read(object ABC);

        bool InReading { get; set; }

        string Show();
    }
}