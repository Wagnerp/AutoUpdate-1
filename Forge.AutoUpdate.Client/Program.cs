using System;
using System.ServiceProcess;

namespace Forge.AutoUpdate.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive) //running as console app
            {
                new CommandLine(args);
            }
            else //running as service
            {
                using (var service = new WindowsService())
                {
                    ServiceBase.Run(service);
                }
            }
        }
    }
}