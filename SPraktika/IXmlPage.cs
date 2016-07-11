using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPraktika
{
    internal interface IXmlPage
    {
        string Address { get; }

        void Read();

        string Show();
    }
}