using D2NG;
using System;

namespace ConsoleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting bot");
            D2NG.Client client = new Client();
            Console.WriteLine("Connecting to Battle.Net");
            client.ConnectToBattleNet("useast.battle.net");
        }
    }
}
