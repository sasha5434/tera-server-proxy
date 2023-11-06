#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Globals;

namespace NetProxy
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var configJson = System.IO.File.ReadAllText("config.json");
                Dictionary<string, ProxyConfig>? configs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, ProxyConfig>>(configJson);
                if (configs == null)
                {
                    throw new Exception("configs is null");
                }

                var tasks = configs.SelectMany(c => ProxyFromConfig(c.Key, c.Value));
                Task.WhenAll(tasks).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex}");
            }
        }

        private static IEnumerable<Task> ProxyFromConfig(string proxyName, ProxyConfig proxyConfig)
        {
            var forwardPort = proxyConfig.forwardPort;
            var localPort = proxyConfig.localPort;
            var forwardIp = proxyConfig.forwardIp;
            var localIp = proxyConfig.localIp;
            var protocol = proxyConfig.protocol;
            try
            {
                if (forwardIp == null)
                {
                    throw new Exception("forwardIp is null");
                }
                if (!forwardPort.HasValue)
                {
                    throw new Exception("forwardPort is null");
                }
                if (!localPort.HasValue)
                {
                    throw new Exception("localPort is null");
                }
                if (!protocol.HasValue)
                {
                    throw new Exception("protocol is null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start {proxyName} : {ex.Message}");
                throw;
            }

            Task task;
            try
            {
                var proxy = new TcpProxy();
                Globals.Global.initOpCodeNamer(protocol.Value);
                task = proxy.Start(forwardIp, forwardPort.Value, localPort.Value, localIp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start {proxyName} : {ex.Message}");
                throw;
            }

            yield return task;
        }
    }

    public class ProxyConfig
    {
        public ushort? localPort { get; set; }
        public string? localIp { get; set; }
        public string? forwardIp { get; set; }
        public ushort? forwardPort { get; set; }
        public int? protocol { get; set; }
    }

    internal interface IProxy
    {
        Task Start(string remoteServerHostNameOrAddress, ushort remoteServerPort, ushort localPort, string? localIp = null);
    }
}