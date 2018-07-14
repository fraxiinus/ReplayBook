# ROFL-Player

![Screenshot](https://i.imgur.com/dP3Q2To.png)

ROFL Player is a simple Windows program that allows you to view replay files from League of Legends.

Features include: 
* Play replay files from other people or games no longer viewable from the client
* View match information without an internet connection or having to open League of Legends
* Double click a replay file to instantly start playing, or view match information

## Setup

1. Simply download the application and extract it to where you want it to be.
2. Launch "ROFLPlayer.exe", you should see the following window appear.

![SettingsWindow](https://i.imgur.com/yrsUL2y.png)

3. Browse for your "League of Legends.exe" file. It is usually at something like "C:\Riot Games\League of Legends\RADS\solutions\lol_game_client_sln\releases\x.x.x.xxx\deploy\League of Legends.exe". ROFL Player will automatically check if you chose the correct file.
3.b (Optional) Select what you want for the default double click behavior and also your player name and region.
4. Close the settings window.
5. Right click a ".rofl" file and select **Properties**. If you downloaded a replay from the game client, replays are stored in your Documents folder.
6. Change the **Opens with** field to "ROFLPlayer.exe"
7. You're done! ROFL Player will now be used when opening League of Legends replay files!

Please let me know about any bugs or issues

## FAQ

**Why does this exist?**

I couldn't find any tools that let you view the information in a replay file, so I made one!

**Will this get me banned?**

It does not do anything except copy files and launch the game executable. Nothing is altered, so a ban is not likely.

## ROFL Parser

ROFL Parser is a class library that is capable of parsing ROFL files. Simple point a file path at it and it will return an ReplayHeader object containing all information inside the replay header.
