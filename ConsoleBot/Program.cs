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
            Client client = new Client();
            Console.WriteLine("Connecting to Battle.Net");
            client.ConnectToBattleNet("useast.battle.net");
            Thread.Sleep(5000);
        }
    }
}
