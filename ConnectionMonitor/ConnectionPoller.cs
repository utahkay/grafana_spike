using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using StatsdClient;

namespace ConnectionMonitor
{
    public class ConnectionPoller : IDisposable
    {
        private bool cancelRequested;
        private Task task;
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionPoller));

        public void Run(string statsdHost, string statsdPrefix)
        {
            try
            {
                Metrics.Configure(new MetricsConfig { StatsdServerName = statsdHost, Prefix = statsdPrefix });
            }
            catch (Exception e)
            {
                log.Error(e);
            }


            task = Task.Run(() =>
                                {
                                    try
                                    {
                                        PollUntilCanceled();
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error(e); // is it safe to access log here?
                                    }

                                    log.Info("Shutting down");
                                });
           
        }

        private void PollUntilCanceled()
        {
            while (!cancelRequested)
            {
                // TODO: Move the netstat-specific code into netstat wrapper
                var portStateCounts = NetstatWrapper.GetNetstatStats();
                var keys = portStateCounts.Keys;
                var total = 0;
                foreach (var key in keys)
                {
                    var state = key.ToLower();
                    var count = portStateCounts[key];
                    total += count;
                    Metrics.GaugeAbsoluteValue($"tcp_{state}", count);
                    log.Info($"tcp_{state} {count}");
                }
                Metrics.GaugeAbsoluteValue($"tcp_total", total);
                log.Info($"tcp_total {total}");
                Thread.Sleep(500);
            }
        }

        public void Dispose()
        {
            cancelRequested = true;
            task?.Wait(1000);
        }
    }
}
