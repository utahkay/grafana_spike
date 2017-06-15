using System.Configuration;
using System.ServiceProcess;

namespace ConnectionMonitor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class MonitorService : ServiceBase
    {
        private ConnectionPoller connectionPoller;

        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            var statsdHost = ConfigurationManager.AppSettings["statsdHost"];
            var statsdPrefix = ConfigurationManager.AppSettings["statsdPrefix"];
            connectionPoller = new ConnectionPoller();
            connectionPoller.Run(statsdHost, statsdPrefix);
        }

        protected override void OnStop()
        {
            connectionPoller?.Dispose();
            connectionPoller = null;
        }
    }
}
