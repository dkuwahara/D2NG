using D2NG;
using System;
using System.Threading;

namespace ConsoleBot
{
    static class Program
    {
        static void Main(string[] args)
        {
            BNCS client = new BNCS();
            client.ConnectTo("useast.battle.net");
            Thread.Sleep(5000);
        }
    }
}
