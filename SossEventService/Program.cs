using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SossEventService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Run with -debug flag the from command line or within Visual Studio.
            // Otherwise, install as a Windows Service using installutil.exe or
            // a setup project (Installshield, WiX Toolset, etc.).
            // See: https://msdn.microsoft.com/en-us/library/sd8zc8ha.aspx

            if (args.Length > 0 && args[0].EndsWith("debug", StringComparison.CurrentCultureIgnoreCase))
            {
                // run from the command line for development/debugging purposes

                var service = new ExpirationEventService();
                service.Debug(args);
                Console.WriteLine("Hit ENTER to stop.");
                Console.ReadLine();
                service.Stop();
            }
            else
            {
                // Run as Windows service

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new ExpirationEventService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
