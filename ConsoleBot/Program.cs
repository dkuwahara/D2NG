using D2NG;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;

namespace ConsoleBot
{
    static class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Starting bot");
            Client client = new Client("config.yml");

            client.BNCS.ConnectTo("useast.battle.net");
            Log.Debug("Waiting for input");
            Console.ReadKey(true);
        }
    }
}
