using D2NG;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace ConsoleBot
{
    class Program
    {
        static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Config File", LongName = "config", ShortName = "c")]
        [Required]
        public String ConfigFile { get; }

        private void OnExecute()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Starting bot");
            Client client = new Client();

            Config config = Config.FromFile(this.ConfigFile);

            try
            {
                client.Bncs.ConnectTo(config.Realm, config.ClassicKey, config.ExpansionKey);
                client.Bncs.Login(config.Username, config.Password);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
            Log.Debug("Waiting for input");
            Console.ReadKey(true);
        }
    }
}
