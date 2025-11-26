using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Definition;
using Sharp.Shared.Enums;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using System.Text.Json;

namespace MS_AutoExec
{
    public class AutoExec : IModSharpModule, IGameListener
    {
        public string DisplayName => "AutoExec";
        public string DisplayAuthor => "DarkerZ[RUS]";
        public AutoExec(ISharedSystem sharedSystem, string dllPath, string sharpPath, Version version, IConfiguration coreConfiguration, bool hotReload)
        {
            _modSharp = sharedSystem.GetModSharp();
            _convars = sharedSystem.GetConVarManager();
            _clients = sharedSystem.GetClientManager();
            _logger = sharedSystem.GetLoggerFactory().CreateLogger<AutoExec>();
            _hooks = sharedSystem.GetHookManager();
            _dllPath = dllPath;
        }
        public static IModSharp? _modSharp;
        public static IConVarManager? _convars;
        private readonly IClientManager _clients;
        public static ILogger<AutoExec>? _logger;
        private readonly IHookManager _hooks;
        private readonly string _dllPath;

        ConfigJSON? cfg = new();
        ConfigJSON? cfgPrefix = new();
        ConfigJSON? cfgMap = new();

        public bool Init()
        {
            LoadCFG();
            _modSharp!.InstallGameListener(this);
            _clients.InstallCommandCallback("ae_reload", OnReload);
            _hooks.TerminateRound.InstallHookPost(OnTerminateRoundPost);
            return true;
        }

        public void Shutdown()
        {
            _clients.RemoveCommandCallback("ae_reload", OnReload);
            _modSharp!.RemoveGameListener(this);

            cfg?.StopTimers();
            cfgPrefix?.StopTimers();
            cfgMap?.StopTimers();

            _hooks.TerminateRound.RemoveHookPost(OnTerminateRoundPost);
        }

        public void OnGameActivate() //OnMapStart
        {
            cfg?.OnMapSpawnHandler();
            LoadCFGPrefix();
            cfgPrefix?.OnMapSpawnHandler();
            LoadCFGMap();
            cfgMap?.OnMapSpawnHandler();
        }

        public void OnGameDeactivate() //OnMapEnd
        {
            cfg?.OnMapEndHandler();
            cfgPrefix?.OnMapEndHandler();
            cfgPrefix?.StopTimers();
            cfgPrefix = null;
            cfgMap?.OnMapEndHandler();
            cfgMap?.StopTimers();
            cfgMap = null;
        }

        public void OnRoundRestart()
        {
            cfg?.OnRoundPrestartHandler();
            cfgPrefix?.OnRoundPrestartHandler();
            cfgMap?.OnRoundPrestartHandler();
        }

        public void OnRoundRestarted()
        {
            cfg?.OnRoundStartHandler();
            cfgPrefix?.OnRoundStartHandler();
            cfgMap?.OnRoundStartHandler();
        }

        private void OnTerminateRoundPost(ITerminateRoundHookParams @params, HookReturnValue<EmptyHookReturn> value)
        {
            cfg?.OnRoundEndHandler();
            cfgPrefix?.OnRoundEndHandler();
            cfgMap?.OnRoundEndHandler();
        }

        void LoadCFG()
        {
            string sConfig = $"{Path.Join(_dllPath, "config.json")}";
            string sData;
            if (File.Exists(sConfig))
            {
                try
                {
                    sData = File.ReadAllText(sConfig);
                    cfg = JsonSerializer.Deserialize<ConfigJSON>(sData);
                }
                catch
                {
                    cfg = null;
                    PrintToConsole($"Bad Config file ({sConfig})");
                }
            }
            else
            {
                cfg = null;
                PrintToConsole($"Config file ({sConfig}) not found");
            }
        }
        void LoadCFGMap()
        {
            if (_modSharp!.GetMapName() is { } mapname)
            {
                string sConfig = $"{Path.Join(_dllPath, $"/maps/{mapname.ToLower()}.json")}";
                string sData;
                if (File.Exists(sConfig))
                {
                    try
                    {
                        sData = File.ReadAllText(sConfig);
                        cfgMap = JsonSerializer.Deserialize<ConfigJSON>(sData);
                    }
                    catch
                    {
                        cfgMap = null;
                        PrintToConsole($"Bad Config file ({sConfig})");
                    }
                }
                else
                {
                    cfgMap = null;
                    PrintToConsole($"Config file ({sConfig}) not found");
                }
            } else cfgMap = null;
        }
        void LoadCFGPrefix()
        {
            if (_modSharp!.GetMapName() is { } mapname)
            {
                int iIndex = mapname.ToLower().IndexOf('_');
                if (iIndex <= 0)
                {
                    cfgPrefix = null;
                    return;
                }
                string sPrefix = mapname.ToLower()[..iIndex];
                string sConfig = $"{Path.Join(_dllPath, $"/prefix/{sPrefix}.json")}";
                string sData;
                if (File.Exists(sConfig))
                {
                    try
                    {
                        sData = File.ReadAllText(sConfig);
                        cfgPrefix = JsonSerializer.Deserialize<ConfigJSON>(sData);
                    }
                    catch
                    {
                        cfgPrefix = null;
                        PrintToConsole($"Bad Config file ({sConfig})");
                    }
                }
                else
                {
                    cfgPrefix = null;
                    PrintToConsole($"Config file ({sConfig}) not found");
                }
            } else cfgPrefix = null;
        }

        private ECommandAction OnReload(IGameClient client, StringCommand command)
        {
            return OnAdminCommand(client, command, "root", OnReloadCallback);
        }

        private void OnReloadCallback(IGameClient client, StringCommand command)
        {
            cfg?.StopTimers();
            cfgPrefix?.StopTimers();
            cfgMap?.StopTimers();
            cfg = null;
            cfgPrefix = null;
            cfgMap = null;
            LoadCFG();
            LoadCFGPrefix();
            LoadCFGMap();
            var player = client.GetPlayerController();
            if (cfg != null)
            {
                if (command.ChatTrigger) player?.Print(HudPrintChannel.Chat, " \x0B[\x04 AutoExec \x0B]\x01 Global ConfigFile reloaded!");
                else player?.Print(HudPrintChannel.Console, "[AutoExec] Global ConfigFile reloaded!");
                PrintToConsole($"Global ConfigFile reloaded by {player?.PlayerName} ({player?.SteamId})");
            }
            if (_modSharp!.GetMapName() is { } mapname)
            {
                if (cfgPrefix != null)
                {
                    int iIndex = mapname.ToLower().IndexOf('_');
                    if (iIndex > 0)
                    {
                        string sPrefix = mapname.ToLower()[..iIndex];
                        if (command.ChatTrigger) player?.Print(HudPrintChannel.Chat, $" \x0B[\x04 AutoExec \x0B]\x01 ConfigFile for prefix {sPrefix} reloaded!");
                        else player?.Print(HudPrintChannel.Console, $"[AutoExec] ConfigFile for prefix {sPrefix} reloaded!");
                        PrintToConsole($"ConfigFile for prefix {sPrefix} reloaded by {player?.PlayerName} ({player?.SteamId})");
                    }
                }
                if (cfgMap != null)
                {
                    if (command.ChatTrigger) player?.Print(HudPrintChannel.Chat, $" \x0B[\x04 AutoExec \x0B]\x01 ConfigFile for map {mapname.ToLower()} reloaded!");
                    else player?.Print(HudPrintChannel.Console, $"[AutoExec] ConfigFile for map {mapname.ToLower()} reloaded!");
                    PrintToConsole($"ConfigFile for map {mapname.ToLower()} reloaded by {player?.PlayerName} ({player?.SteamId})");
                }
            }
            if (cfg == null && cfgMap == null && cfgPrefix == null)
            {
                if (command.ChatTrigger) player?.Print(HudPrintChannel.Chat, " \x0B[\x04 AutoExec \x0B]\x01 Bad ConfigFiles!");
                else player?.Print(HudPrintChannel.Console, $"[AutoExec] Bad ConfigFiles!");
                PrintToConsole($"Bad ConfigFiles!");
            }
        }

        delegate void AdminCommandCallback(IGameClient client, StringCommand command);
        private ECommandAction OnAdminCommand(IGameClient client, StringCommand command, string permission, AdminCommandCallback callback)
        {
            if (callback is not null && client.IsValid)
            {
                if (permission.Equals("<EmptyPermission>"))
                {
                    InvokeClientCallback(client, command, callback);
                    return ECommandAction.Stopped;
                }
                var admin = client.IsAuthenticated ? _clients.FindAdmin(client.SteamId) : null;
                if (admin is not null && admin.HasPermission(permission)) InvokeClientCallback(client, command, callback);
                else
                {
                    if (command.ChatTrigger) client.GetPlayerController()?.Print(HudPrintChannel.Chat, $" {ChatColor.Green}[MS] {ChatColor.Red}You have no permission to use this command!");
                    else client.GetPlayerController()?.Print(HudPrintChannel.Console, "[MS] You have no permission to use this command!");
                }
            }
            return ECommandAction.Stopped;
        }

        private void InvokeClientCallback(IGameClient client, StringCommand command, AdminCommandCallback callbacks)
        {
            foreach (var callback in callbacks.GetInvocationList())
            {
                try
                {
                    ((AdminCommandCallback)callback).Invoke(client, command);
                }
                catch (Exception e)
                {
                    _logger!.LogError(e, "An error occurred while calling CommandCallback::{s}", command.CommandName);
                }
            }
        }

        public static void PrintToConsole(string sValue)
        {
            Console.ForegroundColor = (ConsoleColor)8;
            Console.Write("[");
            Console.ForegroundColor = (ConsoleColor)6;
            Console.Write("AutoExec");
            Console.ForegroundColor = (ConsoleColor)8;
            Console.Write("] ");
            Console.ForegroundColor = (ConsoleColor)3;
            Console.WriteLine(sValue);
            Console.ResetColor();
        }

        int IGameListener.ListenerVersion => IGameListener.ApiVersion;
        int IGameListener.ListenerPriority => 0;
    }
}
