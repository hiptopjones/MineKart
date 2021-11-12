using NLog;
using System;
using System.Diagnostics;

namespace MineKart
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private const int SuccessExitCode = 0;
        private const int FailureExitCode = 1;

        static int Main(string[] args)
        {
            int exitCode = FailureExitCode;
            CommandLineOptions options = new CommandLineOptions();

            try
            {
                if (!options.ParseArguments(args))
                {
                    throw new Exception($"Unable to parse command line arguments: '{Environment.CommandLine}'");
                }

                using (MyGame game = new MyGame())
                {
                    game.StartLoop();
                }

                exitCode = SuccessExitCode;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Processing failed: {ex.Message}");
                exitCode = FailureExitCode;
            }
            finally
            {
                if (options.PromptUser || (exitCode != SuccessExitCode && Debugger.IsAttached))
                {
                    PromptToContinue();
                }
            }

            return exitCode;
        }

        private static void PromptToContinue()
        {
            LogManager.Flush();

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}