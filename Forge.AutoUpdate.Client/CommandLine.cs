using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Args;
using Args.Help;
using Args.Help.Formatters;
using Forge.AutoUpdate.Client.Properties;
using Forge.AutoUpdate.Scheduler;

namespace Forge.AutoUpdate.Client
{
    class CommandLine
    {
        public class Arguments
        {
            [Description("Install the update scheduler as a service (using the name from the configuration file)")]
            public bool InstallService { get; set; }
            [Description("Start the update scheduler service (using the name from the configuration file)")]
            public bool BeginService { get; set; }
            [Description("Stop the update scheduler service (using the name from the configuration file)")]
            public bool EndService { get; set; }
            [Description(@"Careful now... This uninstalls the update scheduler service by the name given in the configuration file. You better make sure that setting matches the name of service you want to uninstall!")]
            public bool UninstallService { get; set; }
            [Description("Run the update scheduler from the console")]
            public bool RunUpdateScheduler { get; set; }
        }

        internal CommandLine(string[] args)
        {
            var config = Configuration.Configure<Arguments>();
            var arguments = config.CreateAndBind(args);

            if (arguments.InstallService)
            {
                InstallService();
            }
            else if (arguments.BeginService)
            {
                StartService();
            }
            else if (arguments.EndService)
            {
                StopService();
            }
            else if (arguments.UninstallService)
            {
                UninstallService();
            }
            else if (arguments.RunUpdateScheduler)
            {
                RunUpdateScheduler();
            }
            else
            {
                ShowHelp(config);
            }
        }

        static void InstallService()
        {
            StartProcess(new ProcessStartInfo(
                "sc",
                string.Format(
                    "create \"{0}\" binpath= \"{1}\"",
                    Settings.Default.ServiceName,
                    Assembly.GetExecutingAssembly().Location)));
        }

        static void StartService()
        {
            StartProcess(new ProcessStartInfo(
                "net",
                string.Format(
                    "start \"{0}\"",
                    Settings.Default.ServiceName)));
        }

        static void StopService()
        {
            StartProcess(new ProcessStartInfo(
                "net",
                string.Format(
                    "stop \"{0}\"",
                    Settings.Default.ServiceName)));
        }

        static void UninstallService()
        {
            StartProcess(new ProcessStartInfo(
                "sc",
                string.Format(
                    "delete \"{0}\"",
                    Settings.Default.ServiceName)));
        }

        static void RunUpdateScheduler()
        {
            var updateScheduler = new UpdateScheduler();
            updateScheduler.Start();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

            updateScheduler.Stop();
        }

        static void ShowHelp(IModelBindingDefinition<Arguments> config)
        {
            new ConsoleHelpFormatter().WriteHelp(
                new HelpProvider().GenerateModelHelp(config),
                Console.Out);
        }

        static void StartProcess(ProcessStartInfo startInfo)
        {
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                StringBuilder output = new StringBuilder(),
                    error = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        output.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        error.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                Console.Write(output.ToString());
                Console.Write(error.ToString());
            }
        }
    }
}