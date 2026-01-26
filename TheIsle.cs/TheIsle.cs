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
using System.Text.RegularExpressions;

namespace WindowsGSM.Plugins
{
    public class TheIsle : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.TheIsleEvrima", // WindowsGSM.XXXX
            author = "Voosjey",
            description = "WindowsGSM plugin for supporting TheIsle Evrima Dedicated Server",
            version = "1.4.0",
            url = "https://raw.githubusercontent.com/voosjey/WindowsGSM.TheIsle", // GitHub repository link
            color = "#34c9eb" // Color Hex
        };

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "412680 -beta evrima"; // Game server appId, TheIsle is 412680

        // - Standard Constructor and properties
        public TheIsle(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // - Game server Fixed variables
        public override string StartPath => @"TheIsle\Binaries\Win64\TheIsleServer-Win64-Shipping.exe"; // Game server start path
        public string FullName = "The Isle Evrima Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?  // Evrima nolonger outputs to console
        public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = new A2S(); // Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        // - Game server default values
        public string Port = "6777"; // Default port - adjusted from 7777 to 6777 to avoid accidently overlapping with other Unreal Engine Servers by default.
        public string QueryPort = "6000"; //Adjusted to start at 6000 to avoid overlapping in WGSM
        public string Defaultmap = "Gateway"; // Default map name
        public string Maxplayers = "75"; // Default maxplayers
        public string Additional = ""; // Additional server start parameter

        // - Create a default cfg for the game server after installation for game.ini and engine.ini
        public async void CreateServerCFG()
        {
            // Game.ini path
            string gameIniPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Game.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(gameIniPath));

            // Engine.ini path
            string engineIniPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Engine.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(engineIniPath));
            
            // Download Game.ini if missing
            if (await DownloadGameServerConfig(gameIniPath, gameIniPath, "Game"))
            {
                string configText = File.ReadAllText(gameIniPath);
                configText = configText.Replace("{{session_name}}", _serverData.ServerName);
                File.WriteAllText(gameIniPath, configText);
            }

            // Download Engine.ini if missing
            if (!File.Exists(engineIniPath))
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        await webClient.DownloadFileTaskAsync($"https://raw.githubusercontent.com/voosjey/The-Isle-Evrima-ini/main/Engine.ini", engineIniPath);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"Github.DownloadEngineIniConfig {e}");
                }
            }
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            // Check for files in Win64
            string win64 = Path.Combine(ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Binaries\Win64\"));
            string[] neededFiles = { "tbb.dll", "tbb12.dll", "tbbmalloc.dll" };

            foreach (string file in neededFiles)
            {
                string srcFile = Path.Combine(ServerPath.GetServersServerFiles(_serverData.ServerID), file);
                string destFile = Path.Combine(win64, file);
                if (File.Exists(srcFile))
                {
                    File.Copy(srcFile, destFile, true);
                }
            }

            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);

            string engineIniPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Engine.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(engineIniPath));
            if (await adaptIniOnLaunch(engineIniPath, engineIniPath, "Engine")) { }

            string gameIniPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"TheIsle\Saved\Config\WindowsServer\Game.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(gameIniPath));

            if (await adaptIniOnLaunch(gameIniPath, gameIniPath, "Game"))
            {
                string section = "/Script/TheIsle.TIGameSession";
                string newServerNameValue = _serverData.ServerName;
                string serverNameKey = "ServerName";
                string newMaxPlayerValue = _serverData.ServerMaxPlayer;
                string maxPlayerKey = "MaxPlayerCount";

                string[] lines = File.ReadAllLines(gameIniPath);
                bool foundSection = false;
                bool serverNameUpdated = false;
                bool maxPlayerCountUpdated = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Equals("[" + section + "]")) { foundSection = true; continue; }

                    if (foundSection)
                    {
                        if (!serverNameUpdated && lines[i].Trim().StartsWith(serverNameKey))
                        {
                            string[] parts = lines[i].Split('=');
                            if (parts.Length >= 2 && !parts[1].Equals(newServerNameValue))
                                lines[i] = serverNameKey + "=" + newServerNameValue;
                            serverNameUpdated = true;
                        }
                        if (!maxPlayerCountUpdated && lines[i].Trim().StartsWith(maxPlayerKey))
                        {
                            string[] parts = lines[i].Split('=');
                            if (parts.Length >= 2 && !parts[1].Equals(newMaxPlayerValue))
                                lines[i] = maxPlayerKey + "=" + newMaxPlayerValue;
                            maxPlayerCountUpdated = true;
                        }
                        if (serverNameUpdated && maxPlayerCountUpdated) { File.WriteAllLines(gameIniPath, lines); break; }
                    }
                }
                if (foundSection && serverNameUpdated && !maxPlayerCountUpdated)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().StartsWith(serverNameKey))
                        {
                            lines[i] += Environment.NewLine + maxPlayerKey + "=" + newMaxPlayerValue;
                            File.WriteAllLines(gameIniPath, lines);
                            break;
                        }
                    }
                }
            }

            await UpdateSettings(_serverData.ServerParam, gameIniPath);

            string param = string.IsNullOrWhiteSpace(_serverData.ServerPort) ? string.Empty : $"?Port={_serverData.ServerPort}";
            param += $"? -nosteamclient -game -server -log";

            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
                try { p.Start(); } catch (Exception e) { Error = e.Message; return null; }
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return p;
            }

            try { p.Start(); return p; } catch (Exception e) { Error = e.Message; return null; }
        }

        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
            });
        }

        public static async Task<bool> DownloadGameServerConfig(string fileSource, string filePath, string iniType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (File.Exists(filePath)) return true;

            string downloadUrl = iniType == "Game" 
                ? $"https://raw.githubusercontent.com/voosjey/The-Isle-Evrima-ini/main/Game.ini"
                : $"https://raw.githubusercontent.com/voosjey/The-Isle-Evrima-ini/main/Engine.ini";

            try { using (WebClient webClient = new WebClient()) { await webClient.DownloadFileTaskAsync(downloadUrl, filePath); } }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"Github.DownloadGameServerConfig {e}"); }

            return File.Exists(filePath);
        }

        public static async Task<bool> adaptIniOnLaunch(string fileSource, string filePath, string iniType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (!File.Exists(filePath)) { await DownloadGameServerConfig(fileSource, filePath, iniType); }
            return File.Exists(filePath);
        }

        public static async Task UpdateSettings(string _serverData, string gameIniPath)
        {
            string[] splitServerData = _serverData.Split(';');
            var adminListFiles = new Dictionary<string, string>();
            var otherSettings = new Dictionary<string, string>();

            foreach (string s in splitServerData)
            {
                string[] splitSetting = s.Split('=');
                if (splitSetting.Length == 2)
                {
                    if (splitSetting[0].StartsWith("adminList", StringComparison.OrdinalIgnoreCase))
                        adminListFiles.Add(splitSetting[0].Trim(), splitSetting[1].Trim());
                    else
                        otherSettings.Add(splitSetting[0].Trim(), splitSetting[1].Trim());
                }
            }

            var combinedAdminList = new List<string>();
            foreach (var kvp in adminListFiles)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string txtFile = await client.DownloadStringTaskAsync(kvp.Value);
                        string[] fileLines = txtFile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        foreach (string line in fileLines) combinedAdminList.Add(line.Trim());
                    }
                }
                catch (Exception) { continue; }
            }

            var lines = File.ReadAllLines(gameIniPath).ToList();
            int startIndex = lines.FindIndex(x => x.StartsWith("[/Script/TheIsle.TIGameSession]"));
            int endIndex = lines.FindIndex(startIndex, x => x.StartsWith("[") && !x.StartsWith("[/Script/TheIsle.TIGameSession]"));
            if (startIndex == -1 || endIndex == -1) return;

            int currentIndex = startIndex + 1;
            while (currentIndex < endIndex)
            {
                if (otherSettings.Keys.Any(key => lines[currentIndex].StartsWith(key))) { lines.RemoveAt(currentIndex); endIndex--; } else { currentIndex++; }
            }

            int insertIndex = endIndex;
            foreach (var setting in otherSettings) { lines.Insert(insertIndex, $"{setting.Key}={setting.Value}"); insertIndex++; }

            if (combinedAdminList.Count > 0)
            {
                int adminListStartIndex = lines.FindIndex(x => x.StartsWith("[/Script/TheIsle.TIGameStateBase]"));
                int adminListEndIndex = lines.FindIndex(adminListStartIndex, x => x.StartsWith("WhitelistIDs="));
                currentIndex = adminListStartIndex + 1;

                while (currentIndex < adminListEndIndex)
                {
                    if (lines[currentIndex].StartsWith("AdminsSteamIDs=")) { lines.RemoveAt(currentIndex); adminListEndIndex--; } else { currentIndex++; }
                }

                int adminInsertIndex = adminListEndIndex;
                foreach (string adminId in combinedAdminList) { lines.Insert(adminInsertIndex, $"AdminsSteamIDs={adminId}"); adminInsertIndex++; }
            }

            File.WriteAllLines(gameIniPath, lines);
        }
    }
}
