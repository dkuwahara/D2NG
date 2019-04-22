using D2NG;
using Serilog;
using System.Threading;

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
            BNCS client = new BNCS();
            client.ConnectTo("useast.battle.net");
            Thread.Sleep(5000);
        }
    }
}
