# German:

Ich habe mich hingesetzt und das Plugin von ksduster https://github.com/ksduster/WindowsGSM.TheIsle für die Version vom 23.12.2025 mit der aktuellen Game.ini und Engine.ini angepasst. Beschreibung übernommen von [@ksduster](https://github.com/ksduster) 

# WindowsGSM.TheIsleEvrima 

🧩WindowsGSM-Plugin, das TheIsle Evrima Dedicated Server-Unterstützung bietet! Aktualisiert für die neueste Evrima-Version. 

- Eine modifizierte Version von [@menix1337](https://www.github.com/menix1337)'s [Legacy](https://github.com/menix1337/WindowsGSM.TheIsleLegacy)
- Eine modifizierte Version von [@ksduster](https://github.com/ksduster) (https://github.com/ksduster/WindowsGSM.TheIsle)

   Problemberichte mit dem WindowsGSM-Plugin bitte auf diesem GitHub melden. Siehe [New issue](https://github.com/Voosjey/WindowsGSM.TheIsleEvrima/issues/new)

  # Diese Version fügt Support für:

  - Automatischer Download von Game.ini und Engine.ini bei Serverfehlern von Evrima, sofern die Dateien beim Stoppen gelöscht wurden.
  - Die Einstellungen in Game.ini unter [/Script/TheIsle.TIGameSession] werden von den Startparametern aktualisiert. Aktualisiere den Servernamen oder die maximale Spieleranzahl nicht in den Startparametern.
  - Admin-Listen*

*Admin-Listen sind eine optionale Möglichkeit, eine oder mehrere Textdateien zu verwenden (Admin-Listen sind Textdateien mit Zeilen von Steam-IDs) und die Möglichkeit, eine oder mehrere pro Server hinzuzufügen, sodass die Serverbesitzer nur eine einzige Textdatei aktualisieren müssen, um alle gewünschten Server mit ihren Admins zu aktualisieren.

# Das Spiel

https://store.steampowered.com/app/376210/The_Isle/ (Beta - Evrima!)

# Anforderungen

WindowsGSM >= 1.23.1

# Installation

1. Lade die neueste Version von WindowsGSM herunter
2. Verschiebe den TheIsle.cs-Ordner in den Plugins-Ordner
3. Klicke auf den Button [RELOAD PLUGINS] oder starte WindowsGSM neu

# Start Parameteroptionen (Optional) 
Hier ist eine Standard-[Game.ini](https://github.com/ksduster/The-Isle-Evrima-ini/blob/main/Game.ini). 
Nur Werte im Abschnitt `/Script/TheIsle.TIGameSession` können geändert werden. Nach der Installation des Servers wählen Sie die Option "Konfiguration bearbeiten". Unter "Serverstart-Parameter" geben Sie die ConfigOption=true/false ein und trennen sie mit einem Semikolon (`;`). 

Beispiel: `bEnableHumans=true;bQueueEnabled=true;QueuePort=9999` 

Beim nächsten Start oder Neustart Ihres Servers werden die Werte im Abschnitt `/Script/TheIsle.TIGameSession` der game.ini hinzugefügt. Fügen Sie keine `Servername`- oder `MaxPlayerCount`-Werte hinzu, da diese aus den Serverkonfigurationswerten im Bearbeitungsfenster aktualisiert werden.

;ögliche Werte:

`bEnableHumans=true/false`

`bQueueEnabled=true/false`

`QueuePort=portnumber`

`bServerPassword=true/false`

`ServerPassword="password"` // Keep the quotations. Change the `password`

`bRconEnabled=true/false` // Remote console commands

`RconPassword="password"`  // Do Not keep as `password` - Do not make this public

`RconPort=portnumber`

`bServerDynamicWeather=true/false` // Evrima update 0.15.191 - Temporarily disabled - has no effect

`MinWeatherVariationInterval=600` // Set in seconds how often to switch weather - added v0.17.54

`MaxWeatherVariationInterval=900` // Set in seconds how often to switch weather - added v0.17.54

`ServerDayLengthMinutes=45`  // Value in minutes

`ServerNightLengthMinutes=20` // Value in minutes

`bServerWhitelist=true/false` // Enable/Disable whitelist - You must manually add steamIDs in the Game.ini `/Script/TheIsle.TIGameStateBase` section.  1 steam ID per line `WhitelistIDs=<steamid64>`  (future project)

`bEnableGlobalChat=true/false` // Turn on/off Global Chat. Disabled by default.

`bSpawnPlants=true/false` // Turn on/off Plants for herbivors. Enabled by default

`PlantSpawnMultiplier=1` //Multiplies how many plants to spawn, rise this value to increase plant spawn amount - Added v0.17.54

`bSpawnAI=true/false` // Turn on/off AI for players to eat. Enabled by default

`AIDensity=1` // Added v0.17.54 - No notes from developers

`AISpawnInterval=40` // Value in seconds to check if players are hungry and spawn if needed

`bEnableMigration=true/false` // Enabled by default. Makes players move across the map for a well balanced meal. Allows more likely chances of PvP encounters.

`MaxMigrationTime=5400` // Value in seconds. Default=5400 which is 90 minutes.  Math is seconds / 60 = minutes.  so 5400 / 60 = 90 minutes.

`GrowthMultiplier=1` // Value is 1 by default. Per developers: Universal multiplier for growth, putting this number too high will stop it to work (stay below ~20)

`bEnableMutations=true/false` // Enable/disable all mutations.  See Game.ini to enable/disable specific mutation types.

`bUseRegionSpawning=false` //Enable region spawn

`bUseRegionSpawnCooldown=true` //Enable region cooldown

`RegionSpawnCooldownTimeSeconds=600` //Region cooldown max time.

`CorpseDecayMultiplier=1` //Multiplies how fast corpses decay to despawn - Reduce value to be faster Added v0.17.54

`bAllowRecordingReplay=true` //Enable replays.

`bEnableDiets=true` //Enable/Disable diets, no more buffs

`bEnableMigration=true` //Disable/Enable Migrations, include Patrols and Mass

`SpeciesMigrationTime=10800` //Max time a species MZ stays active

`bEnableMassMigration=true` //Enable/Disable Mass migration

`MassMigrationTime=43200` //Value is in seconds 12h - How often new mass migration is set

`MassMigrationDisableTime=7200` // Value is in seconds 2h - How long a mass migration last once set

`bEnablePatrolZones=true` //Enable/Disable PatrolZone system

// Add the names of each AI class that should be disabled, one line for each.
`DisallowedAIClasses=Compsognathus`
`DisallowedAIClasses=Pterodactylus`
`DisallowedAIClasses=Boar`
`DisallowedAIClasses=Deer`
`DisallowedAIClasses=Goat`
`DisallowedAIClasses=Seaturtle`

`Discord=""` // Example: Discord="https://discord.gg/abc1234" - Stelle sicher, dass der Link zum Discord nicht abläuft, sonst klicken die Benutzer im Spielmenü auf die Discord-Schaltfläche und die sich öffnende Website führt ins Leere. Aktuell ist Discord in der Game.ini von mir auskommentiert.

# Admin-Listen (Nicht erforderlich!) 
Credits: [@menix1337](https://www.github.com/menix1337)'s [Legacy](https://github.com/menix1337/WindowsGSM.TheIsleLegacy) 
Dieses Plugin ermöglicht es Serverhostern, eine oder mehrere Textdateien mit SteamID-Listen anzugeben (derzeit nur mit RAW-Github-Repo-Dateien getestet) und diese automatisch in die Game.ini-Datei des Servers einzufügen. Das bedeutet, wenn Sie ein Hoster mit mehreren Servern sind und es vermeiden möchten, stundenlang jede Game.ini-Datei für Hinzufügen oder Entfernen anzupassen, könnte dies eine geeignete Option für Sie sein. 
1. Fügen Sie im Feld für die Startparameter des Servers Folgendes ein (*Beispiele mit meinen Testdateien, verwenden Sie Ihre eigenen!): `adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt`

Sie können mehrere Listen hinzufügen, indem Sie ein Semikolon (`;`) mit einem neuen Listeneintrag hinzufügen, wie zum Beispiel: 
- `adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt;adminListTwo=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlisttwo.txt` --

Und wenn Sie möchten, können Sie diese Liste mit AdminListThree, AdminListFour erweitern – gefolgt von den Links wie in den obigen Beispielen ... usw. (Theoretisch gibt es keine Begrenzung) ## Was passiert also mit diesen Listen? WindowsGSM wird die Textdatei öffnen und jede Steam-ID in der Liste zu einer kombinierten Liste zusammenführen & `ServerAdmin=` davor hinzufügen und Ihre Game.ini ändern, indem sie dort eingetragen werden. Stellen Sie also sicher, dass alle Administratoren, die Sie im Spiel als Admin haben möchten, in diesen Listen enthalten sind, falls Sie sich entscheiden, sie zu verwenden.



# English:

I sat down and adapted the plugin from ksduster https://github.com/ksduster/WindowsGSM.TheIsle for the version from 12/23/2025 with the current Game.ini and Engine.ini.
Description taken from [@ksduster](https://github.com/ksduster) 

# WindowsGSM.TheIsleEvrima

🧩WindowsGSM plugin that provides TheIsle Evrima Dedicated server support! Updated for the newest Evrima version.

- A modified version of [@menix1337](https://www.github.com/menix1337)'s [Legacy](https://github.com/menix1337/WindowsGSM.TheIsleLegacy)
- A modified version of [@ksduster](https://github.com/ksduster)  (https://github.com/ksduster/WindowsGSM.TheIsle)

- Report issues with the WindowsGSM plugin on this github please. See [New issue](https://github.com/Voosjey/WindowsGSM.TheIsleEvrima/issues/new)
# This version adds in support for:
- Automatic download of Game.ini and Engine.ini upon server bug from Evrima where the files are deleted when stopped.
- Game.ini settings under [/Script/TheIsle.TIGameSession] are updated from start params.  Do not update servername or maxplayercount in start params.
- Admin lists\*

\*Admin lists is a optional way for you to support having one or multiple text files (Admin lists are textfiles with lines of Steam IDs) and the ability to add one or multiple per server, leaving the server owners only having to update a a single text file - to update all the servers you want, with your admins

# The Game

https://store.steampowered.com/app/376210/The_Isle/ (Beta - Evrima!)

# Requirements

WindowsGSM >= 1.23.1

# Installation

1. Download the latest release
2. Move TheIsle.cs folder to plugins folder
3. Click [RELOAD PLUGINS] button or restart WindowsGSM

# Start Paramater Options (Optional)

Here is a default [Game.ini](https://github.com/ksduster/The-Isle-Evrima-ini/blob/main/Game.ini). 
Only values in the `/Script/TheIsle.TIGameSession` section can be modified.

After installing the server, select the "Edit Config" option. 
Under the "Server Start Param" enter the ConfigOption=true/false, and seperate with semicolon (`;`)

Example: `bEnableHumans=true;bQueueEnabled=true;QueuePort=9999`

The next time you start or restart your server the values will be added to the game.ini in the `/Script/TheIsle.TIGameSession` section

Do not add `Servername` or `MaxPlayerCount` values, as these are updated from the server config values in the edit window.

Availabe values:

`bEnableHumans=true/false`

`bQueueEnabled=true/false`

`QueuePort=portnumber`

`bServerPassword=true/false`

`ServerPassword="password"` // Keep the quotations. Change the `password`

`bRconEnabled=true/false` // Remote console commands

`RconPassword="password"`  // Do Not keep as `password` - Do not make this public

`RconPort=portnumber`

`bServerDynamicWeather=true/false` // Evrima update 0.15.191 - Temporarily disabled - has no effect

`MinWeatherVariationInterval=600` // Set in seconds how often to switch weather - added v0.17.54

`MaxWeatherVariationInterval=900` // Set in seconds how often to switch weather - added v0.17.54

`ServerDayLengthMinutes=45`  // Value in minutes

`ServerNightLengthMinutes=20` // Value in minutes

`bServerWhitelist=true/false` // Enable/Disable whitelist - You must manually add steamIDs in the Game.ini `/Script/TheIsle.TIGameStateBase` section.  1 steam ID per line `WhitelistIDs=<steamid64>`  (future project)

`bEnableGlobalChat=true/false` // Turn on/off Global Chat. Disabled by default.

`bSpawnPlants=true/false` // Turn on/off Plants for herbivors. Enabled by default

`PlantSpawnMultiplier=1` //Multiplies how many plants to spawn, rise this value to increase plant spawn amount - Added v0.17.54

`bSpawnAI=true/false` // Turn on/off AI for players to eat. Enabled by default

`AIDensity=1` // Added v0.17.54 - No notes from developers

`AISpawnInterval=40` // Value in seconds to check if players are hungry and spawn if needed

`bEnableMigration=true/false` // Enabled by default. Makes players move across the map for a well balanced meal. Allows more likely chances of PvP encounters.

`MaxMigrationTime=5400` // Value in seconds. Default=5400 which is 90 minutes.  Math is seconds / 60 = minutes.  so 5400 / 60 = 90 minutes.

`GrowthMultiplier=1` // Value is 1 by default. Per developers: Universal multiplier for growth, putting this number too high will stop it to work (stay below ~20)

`bEnableMutations=true/false` // Enable/disable all mutations.  See Game.ini to enable/disable specific mutation types.

`bUseRegionSpawning=false` //Enable region spawn

`bUseRegionSpawnCooldown=true` //Enable region cooldown

`RegionSpawnCooldownTimeSeconds=600` //Region cooldown max time.

`CorpseDecayMultiplier=1` //Multiplies how fast corpses decay to despawn - Reduce value to be faster Added v0.17.54

`bAllowRecordingReplay=true` //Enable replays.

`bEnableDiets=true` //Enable/Disable diets, no more buffs

`bEnableMigration=true` //Disable/Enable Migrations, include Patrols and Mass

`SpeciesMigrationTime=10800` //Max time a species MZ stays active

`bEnableMassMigration=true` //Enable/Disable Mass migration

`MassMigrationTime=43200` //Value is in seconds 12h - How often new mass migration is set

`MassMigrationDisableTime=7200` // Value is in seconds 2h - How long a mass migration last once set

`bEnablePatrolZones=true` //Enable/Disable PatrolZone system

// Add the names of each AI class that should be disabled, one line for each.
`DisallowedAIClasses=Compsognathus`
`DisallowedAIClasses=Pterodactylus`
`DisallowedAIClasses=Boar`
`DisallowedAIClasses=Deer`
`DisallowedAIClasses=Goat`
`DisallowedAIClasses=Seaturtle`

`Discord=""` // Example: Discord="https://discord.gg/abc1234" - Make sure the link does not expire from your discord otherwise users will click the discord button in game menu and the website that opens will go no where.




# Admin Lists (Not required!)   Credit: [@menix1337](https://www.github.com/menix1337)'s [Legacy](https://github.com/menix1337/WindowsGSM.TheIsleLegacy)

This plugin has the ability for the server hosters to specify one or multiple text files with lists of SteamIDs (Currently only tested with RAW Github Repo files) adding them automatically to the servers Game.ini file.
This means if you are a hoster having multiple servers, and you dislike having to spend hours on adjusting each Game.ini file for adding/removings - this might be an option for you.

1. In the servers Start Param option field add in the following (\*Examples with my test files, use your own!)

- `adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt`

You can add multiple lists by adding a semi-colon (`;`)with a new list entry such as:

- `adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt;adminListTwo=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlisttwo.txt`

-- And if you wish you can expand to this list with AdminListThree, AdminListFour - followed by the links as the examples above... etc (There should be no limit in theory)

## So what happens with these lists?

WindowsGSM will open the text file and merge each Steam ID on the list into a combined list & add `ServerAdmin=` in front & modify your Game.ini by adding them in there.
So make sure all admins you want to have as admin in game, are added to these lists, if you use decide it.

So lets say if you have 2 steamids in adminListOne and 1 steamid in adminListTwo, it will combine them into 3 Steam IDs, getting added as admin on your server.
(This gives you an option to add seperate lists for lets say';' Deathmatch, Event servers where you maybe need more people to be admin, that you don't want to have admin on the other servers)

So in theory adminListOne could be your main admins
adminListTwo could be trial admins
adminListThree could eventually be DM/Event related admins

- and combined they will make 1 admin list in your server.

**OBS: Currently only supporting text file, laying online in places such as GitHub etc. (Raw text files)**
**- In case your source for admin lists textfiles goes down, or you do not apply one - it will just keep using the Game.Ini you already have**

# So how could a final Server Start Param look? (With and Without the usage of admin lists)

`bEnableHumans=true;bQueueEnabled=true;QueuePort=9999` or
`bEnableHumans=true;bQueueEnabled=true;QueuePort=9999;adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt` or
`bEnableHumans=true;bQueueEnabled=true;QueuePort=9999;adminListOne=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlist.txt;adminListTwo=https://raw.githubusercontent.com/menix1337/WindowsGSM.configs/main/Other/adminlisttwo.txt`

**OBS: Remember if you use Admin Lists to adjust them into your own Steam IDs. The Steam IDs & lists provided in the examples are only for an example purpose**

# License

This project is licensed under the MIT License - see the <a href="https://raw.githubusercontent.com/Voosjey/WindowsGSM.TheIsleEvrima/main/LICENSE">LICENSE.md</a> file for details
