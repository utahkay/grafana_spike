using System;
using System.ServiceProcess;
using log4net.Config;

namespace ConnectionMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            if (Environment.UserInteractive)
            {
                new MonitorService().Start(new string[0]);
                Console.ReadKey();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[]
                {
                    new MonitorService()
                });
            }
        }
    }
}
