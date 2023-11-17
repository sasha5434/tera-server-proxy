#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tera.Game.Structures;
using TeraPartyMonitor.DataSender.Models;

namespace NetProxy
{
    internal static class Program
    {
        static readonly string configDir = "Config";
        private static void Main(string[] args)
        {
            try
            {
                var configJson = System.IO.File.ReadAllText($"{configDir}/config.json");
                ProxyConfig? config = JsonSerializer.Deserialize<ProxyConfig>(configJson) ?? throw new Exception("configs is null");
                var task = ProxyFromConfig(config);
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex}");
            }
        }

        private static Task ProxyFromConfig(ProxyConfig proxyConfig)
        {
            var forwardPort = proxyConfig.forwardPort;
            var localPort = proxyConfig.localPort;
            var forwardIp = proxyConfig.forwardIp;
            var localIp = proxyConfig.localIp;
            var webPort = proxyConfig.webPort;
            var protocol = proxyConfig.protocol;
            var logging = proxyConfig.logging;
            var hexy = proxyConfig.hexy;
            try
            {
                if (forwardIp == null)
                    throw new Exception("forwardIp is null");
                if (!forwardPort.HasValue)
                    throw new Exception("forwardPort is null");
                if (!localPort.HasValue)
                    throw new Exception("localPort is null");
                if (localIp == null)
                    throw new Exception("localIp is null");
                if (!webPort.HasValue)
                    throw new Exception("webPort is null");
                if (!protocol.HasValue)
                    throw new Exception("protocol is null");
                if (!logging.HasValue)
                    throw new Exception("logging is null");
                if (!hexy.HasValue)
                    throw new Exception("hexy is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start: {ex.Message}");
                throw;
            }

            Task task;
            try
            {
                var proxy = new TcpProxy();
                Globals.Opcode.Init(protocol.Value);
                Globals.Logs.enabled = logging.Value;
                Globals.Logs.hexy = hexy.Value;
                task = proxy.Start(forwardIp, forwardPort.Value, localPort.Value, localIp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start: {ex.Message}");
                throw;
            }

            var builder = WebApplication.CreateBuilder();
            builder.Configuration.SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(Path.Combine(configDir, "webconfig.json"));

            var app = builder.Build();

            app.MapGet("/online", GetPlayers);
            app.MapGet("/dungeons", GetDungeons);
            app.MapGet("/battlegrounds", GetBattlegrounds);
            app.Run($"http://{localIp}:{webPort.Value}");
            return task;
        }
        private static IEnumerable<Tera.Connection.CharacterInfo> GetPlayers()
        {
            return Globals.WebTeraData.Pools.GetPlayers();
        }
        private static Dictionary<uint, PartyMatchingModel> GetDungeons()
        {
            return Globals.WebTeraData.Pools.GetPartyMatchings()
                .Where(m => m.MatchingType == MatchingTypes.Dungeon)
                .SelectMany(m => m.Instances.Select(instance => (m.MatchingProfiles, instance.Id)))
                .GroupBy(s => s.Id)
                .Select(g => new KeyValuePair<uint, PartyMatchingModel>(g.Key, new PartyMatchingModel(g.Select(p => p.MatchingProfiles))))
                .ToDictionary(s => s.Key, s => s.Value);
        }

        private static Dictionary<uint, PartyMatchingModel> GetBattlegrounds()
        {
            return Globals.WebTeraData.Pools.GetPartyMatchings()
                .Where(m => m.MatchingType == MatchingTypes.Battleground)
                .SelectMany(m => m.Instances.Select(instance => (m.MatchingProfiles, instance.Id)))
                .GroupBy(s => s.Id)
                .Select(g => new KeyValuePair<uint, PartyMatchingModel>(g.Key, new PartyMatchingModel(g.Select(p => p.MatchingProfiles))))
                .ToDictionary(s => s.Key, s => s.Value);
        }
    }

    public class ProxyConfig
    {
        public ushort? localPort { get; set; }
        public string? localIp { get; set; }
        public string? forwardIp { get; set; }
        public ushort? forwardPort { get; set; }
        public ushort? webPort { get; set; }
        public int? protocol { get; set; }
        public bool? logging { get; set; }
        public bool? hexy { get; set; }
    }

    internal interface IProxy
    {
        Task Start(string remoteServerHostNameOrAddress, ushort remoteServerPort, ushort localPort, string? localIp = null);
    }
}