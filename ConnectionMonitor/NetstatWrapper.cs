using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;

namespace ConnectionMonitor
{
    public class NetstatWrapper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetstatWrapper));

        public static Dictionary<string, int> GetNetstatStats()
        {
            using (var p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                                  {
                                      Arguments = "-n",
                                      FileName = "netstat.exe",
                                      UseShellExecute = false,
                                      CreateNoWindow = true,
                                      RedirectStandardInput = false,
                                      RedirectStandardError = true,
                                      RedirectStandardOutput = true
                                  };
                p.Start();

                var stdOut = p.StandardOutput.ReadToEnd();
                var errorOut = p.StandardError.ReadToEnd();

                p.WaitForExit(); // maybe not necessary because of ReadToEnds above?

                if (p.ExitCode != 0)
                {
                    throw new Exception(errorOut);
                }

                log.Info("Success");
                log.Debug(stdOut);
                return ParsePorts(stdOut);
            }
        }

        private static Dictionary<string, int> ParsePorts(string content)
        {
            var counts = new Dictionary<string, int>();

            var rows = Regex.Split(content, "\r\n");
            foreach (var row in rows)
            {
                var tokens = Regex.Split(row, "\\s+").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (tokens.Length >= 4 && tokens[0] == "TCP")
                {
                    var state = tokens[3];
                    Increment(counts, state);
                }
            }
            return counts;
        }

        private static void Increment(Dictionary<string, int> counts, string state)
        {
            if (!counts.ContainsKey(state))
            {
                counts.Add(state, 1);
            }
            else
            {
                counts[state]++;
            }
        }
    }
}