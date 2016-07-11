namespace SPraktika
{
    internal interface IXmlPage
    {
        string Address { get; }

        void Read();

        string Show();
    }
}