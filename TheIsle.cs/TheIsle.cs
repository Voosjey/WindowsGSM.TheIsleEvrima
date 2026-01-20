using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;
using WindowsGSM.GameServer.Engine;
using System.IO;
using System.Linq;
using System.Net;

namespace WindowsGSM.Plugins
{
    public class TheIsle : SteamCMDAgent
    {
        // Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.TheIsleEvrima",
            author = "Voosjey",
            description = "WindowsGSM plugin for The Isle Evrima Dedicated Server",
            version = "1.4.0",
            url = "https://github.com/Voosjey/WindowsGSM.TheIsleEvrima",
            color = "#34c9eb"
        };

        // SteamCMD settings
        public override bool loginAnonymous => true;
        public override string AppId => "412680 -beta evrima"; // Evrima branch

        // Standard constructor
        private readonly ServerConfig _serverData;
        public TheIsle(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;

        // Fixed variables
        public override string StartPath => @"TheIsle\Binaries\Win64\TheIsleServer-Win64-Shipping.exe";
        public string FullName = "The Isle Evrima Dedicated Server";
        public bool AllowsEmbedConsole = true; // wie 1.3.1 – GSM nutzt das Flag
        public int PortIncrements = 1;
        public object QueryMethod = new A2S();

        // Default values (werden von GSM überschrieben)
        public string Port = "6777";
        public string QueryPort = "6000";
        public string Defaultmap = "Gateway";
        public string Maxplayers = "75";
        public string Additional = "";

        public string Error, Notice;

        // Create default cfg (Game.ini + Engine.ini)
        public async void CreateServerCFG()
        {
            string gameIniPath = ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Game.ini");
            string engineIniPath = ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Engine.ini");

            Directory.CreateDirectory(Path.GetDirectoryName(gameIniPath));
            Directory.CreateDirectory(Path.GetDirectoryName(engineIniPath));

            // Game.ini
            if (await DownloadGameServerConfig(gameIniPath, gameIniPath, "Game"))
            {
                string configText = File.ReadAllText(gameIniPath);
                configText = configText.Replace("{{session_name}}", _serverData.ServerName);
                File.WriteAllText(gameIniPath, configText);
            }

            // Engine.ini
            if (!File.Exists(engineIniPath))
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        await webClient.DownloadFileTaskAsync(
                            "https://raw.githubusercontent.com/Voosjey/The-Isle-Evrima-ini/refs/heads/main/Engine.ini",
                            engineIniPath
                        );
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Github.DownloadEngineIniConfig {e}");
                }
            }
        }

        // Start server
        public async Task<Process> Start()
        {
            // tbb DLLs wie im Original kopieren
            string win64 = Path.Combine(ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Binaries\Win64\"));
            string[] neededFiles = { "tbb.dll", "tbb12.dll", "tbbmalloc.dll" };

            foreach (string file in neededFiles)
            {
                string src = Path.Combine(ServerPath.GetServersServerFiles(_serverData.ServerID), file);
                string dst = Path.Combine(win64, file);
                if (!File.Exists(dst) && File.Exists(src))
                {
                    File.Copy(src, dst);
                }
            }

            string shipExePath = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);

            // Engine.ini sicherstellen
            string engineIniPath = ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Engine.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(engineIniPath));
            await adaptIniOnLaunch(engineIniPath, engineIniPath, "Engine");

            // Game.ini sicherstellen + aktualisieren
            string gameIniPath = ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Game.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(gameIniPath));
            await adaptIniOnLaunch(gameIniPath, gameIniPath, "Game");

            // Game.ini mit GSM‑Werten aktualisieren
            UpdateGameIniWithGSM(gameIniPath);

            // Adminlisten + zusätzliche Settings aus ServerParam
            await UpdateSettings(_serverData.ServerParam, gameIniPath);

            // Startparameter bauen (UE5‑kompatibel, ohne Map‑Pfad)
            string param = BuildStartParameter();

            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = param,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Verhalten wie 1.3.1: EmbedConsole = true → Redirect + kein Fenster
            if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;

                try
                {
                    p.Start();
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    return null;
                }

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return p;
            }
            else
            {
                // Falls du AllowsEmbedConsole später mal auf false setzt
                p.StartInfo.CreateNoWindow = true;

                try
                {
                    p.Start();
                    return p;
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    return null;
                }
            }
        }

        // Stop server
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
            });
        }

        // Startparameter – angepasst für Evrima/UE5
        private string BuildStartParameter()
        {
            // Keine Map im Param – Map kommt aus Game.ini (MapName)
            // Ports als Flags, nicht als URL‑Query
            string param = string.Empty;

            if (!string.IsNullOrWhiteSpace(_serverData.ServerPort))
            {
                param += $" -Port={_serverData.ServerPort}";
            }

            if (!string.IsNullOrWhiteSpace(_serverData.ServerQueryPort))
            {
                param += $" -QueryPort={_serverData.ServerQueryPort}";
            }

            // Standardflags für Dedicated Server
            param += " -log -NoSound -nosteamclient -server -game";

            return param.Trim();
        }

        // Game.ini mit GSM‑Werten aktualisieren (Name, MaxPlayer, Map, QueryPort)
        private void UpdateGameIniWithGSM(string gameIniPath)
        {
            if (!File.Exists(gameIniPath))
            {
                return;
            }

            var lines = File.ReadAllLines(gameIniPath).ToList();
            bool inSession = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (line.Equals("[/Script/TheIsle.TIGameSession]", StringComparison.OrdinalIgnoreCase))
                {
                    inSession = true;
                    continue;
                }

                if (inSession && line.StartsWith("[") && !line.Equals("[/Script/TheIsle.TIGameSession]", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (inSession)
                {
                    if (line.StartsWith("ServerName=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"ServerName={_serverData.ServerName}";
                    }

                    if (line.StartsWith("MaxPlayerCount=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"MaxPlayerCount={_serverData.ServerMaxPlayer}";
                    }

                    if (line.StartsWith("MapName=", StringComparison.OrdinalIgnoreCase))
                    {
                        string map = string.IsNullOrWhiteSpace(_serverData.ServerMap)
                            ? Defaultmap
                            : _serverData.ServerMap.Trim();
                        lines[i] = $"MapName={map}";
                    }

                    if (line.StartsWith("QueryPort=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrWhiteSpace(_serverData.ServerQueryPort))
                        {
                            lines[i] = $"QueryPort={_serverData.ServerQueryPort}";
                        }
                    }
                }
            }

            File.WriteAllLines(gameIniPath, lines);
        }

        // Download Game/Engine ini
        public static async Task<bool> DownloadGameServerConfig(string fileSource, string filePath, string iniType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
            {
                return true;
            }

            string downloadUrl = iniType == "Game"
                ? "https://raw.githubusercontent.com/Voosjey/The-Isle-Evrima-ini/refs/heads/main/Game.ini"
                : "https://raw.githubusercontent.com/Voosjey/The-Isle-Evrima-ini/refs/heads/main/Engine.ini";

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(downloadUrl, filePath);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Github.DownloadGameServerConfig {e}");
            }

            return File.Exists(filePath);
        }

        public static async Task<bool> adaptIniOnLaunch(string fileSource, string filePath, string iniType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (!File.Exists(filePath))
            {
                try
                {
                    await DownloadGameServerConfig(fileSource, filePath, iniType);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Github.DownloadGameServerConfig {e}");
                }
            }

            return File.Exists(filePath);
        }

        // ServerParam → Settings + Adminlisten in Game.ini
        public static async Task UpdateSettings(string _serverData, string gameIniPath)
        {
            if (string.IsNullOrWhiteSpace(_serverData) || !File.Exists(gameIniPath))
            {
                return;
            }

            string[] splitServerData = _serverData.Split(';');
            var adminListFiles = new Dictionary<string, string>();
            var otherSettings = new Dictionary<string, string>();

            foreach (string s in splitServerData)
            {
                string[] splitSetting = s.Split('=');
                if (splitSetting.Length == 2)
                {
                    if (splitSetting[0].StartsWith("adminList", StringComparison.OrdinalIgnoreCase))
                    {
                        adminListFiles[splitSetting[0].Trim()] = splitSetting[1].Trim();
                    }
                    else
                    {
                        otherSettings[splitSetting[0].Trim()] = splitSetting[1].Trim();
                    }
                }
            }

            // Adminlisten kombinieren
            var combinedAdminList = new List<string>();
            foreach (var kvp in adminListFiles)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string txtFile = await client.DownloadStringTaskAsync(kvp.Value);
                        string[] fileLines = txtFile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        foreach (string line in fileLines)
                        {
                            string trimmed = line.Trim();
                            if (!string.IsNullOrEmpty(trimmed))
                            {
                                combinedAdminList.Add(trimmed);
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            var lines = File.ReadAllLines(gameIniPath).ToList();

            // /Script/TheIsle.TIGameSession – andere Settings
            int startIndex = lines.FindIndex(x => x.StartsWith("[/Script/TheIsle.TIGameSession]"));
            int endIndex = -1;
            if (startIndex != -1)
            {
                endIndex = lines.FindIndex(startIndex + 1, x => x.StartsWith("[") && !x.StartsWith("[/Script/TheIsle.TIGameSession]"));
                if (endIndex == -1)
                {
                    endIndex = lines.Count;
                }

                int currentIndex = startIndex + 1;
                while (currentIndex < endIndex)
                {
                    if (otherSettings.Keys.Any(key => lines[currentIndex].StartsWith(key + "=", StringComparison.OrdinalIgnoreCase)))
                    {
                        lines.RemoveAt(currentIndex);
                        endIndex--;
                    }
                    else
                    {
                        currentIndex++;
                    }
                }

                int insertIndex = endIndex;
                foreach (var setting in otherSettings)
                {
                    lines.Insert(insertIndex, $"{setting.Key}={setting.Value}");
                    insertIndex++;
                }
            }

            // AdminsSteamIDs in /Script/TheIsle.TIGameStateBase
            if (combinedAdminList.Count > 0)
            {
                int adminListStartIndex = lines.FindIndex(x => x.StartsWith("[/Script/TheIsle.TIGameStateBase]"));
                if (adminListStartIndex != -1)
                {
                    int adminListEndIndex = lines.FindIndex(adminListStartIndex + 1, x => x.StartsWith("WhitelistIDs="));
                    if (adminListEndIndex == -1)
                    {
                        adminListEndIndex = lines.Count;
                    }

                    int currentIndex = adminListStartIndex + 1;
                    while (currentIndex < adminListEndIndex)
                    {
                        if (lines[currentIndex].StartsWith("AdminsSteamIDs=", StringComparison.OrdinalIgnoreCase))
                        {
                            lines.RemoveAt(currentIndex);
                            adminListEndIndex--;
                        }
                        else
                        {
                            currentIndex++;
                        }
                    }

                    int adminInsertIndex = adminListEndIndex;
                    foreach (string adminId in combinedAdminList)
                    {
                        lines.Insert(adminInsertIndex, $"AdminsSteamIDs={adminId}");
                        adminInsertIndex++;
                    }
                }
            }

            File.WriteAllLines(gameIniPath, lines);
        }
    }
}
