using D2NG;
using Serilog;
using System;

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
            Client client = new Client();
            client.Bncs.ConnectTo("useast.battle.net");
            Log.Debug("Waiting for input");
            Console.ReadKey(true);
        }
    }
}
