using D2NG;
using System;
using System.Threading;

namespace ConsoleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting bot");
            BNCS client = new BNCS();
            Console.WriteLine("Connecting to Battle.Net");
            client.ConnectTo("useast.battle.net");
            Thread.Sleep(5000);
        }
    }
}
