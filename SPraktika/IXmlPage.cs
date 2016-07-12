using System.Collections.Generic;

namespace SPraktika
{
    internal interface IXmlPage
    {
        string Address { get; }

        void Read(HashSet<string> ABC);

        string Show();
    }
}