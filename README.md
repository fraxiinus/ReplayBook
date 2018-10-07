# ROFLPlayer

![Screenshot](https://i.imgur.com/69LOd0x.png)

ROFLPlayer is a simple Windows program that allows you to view replay files from League of Legends.

Features include: 
* Play replay files from other people or games no longer viewable from the client
* View match information without an internet connection or having to open League of Legends
* Dump all replay information into JSON, allowing you to view every single statistic stored
* Double click a replay file to instantly start playing, or view match information
* Super easy to use, no installation necessary

## Download
Download the latest release here
https://github.com/andrew1421lee/ROFL-Player/releases

## Setup

1. Download the application and extract it to where you want it to be.
2. Launch "ROFLPlayer.exe", ROFLPlayer will attempt to automatically find your League of Legends install folder. If it cannot, then the program will prompt you for help.
3. The following window will then appear, Set the player name field so ROFLPlayer can highlight you name when viewing replay details. Set the region field so the program can properly load the match history website when using the "view online" button.

![SettingsWindow](https://i.imgur.com/yrsUL2y.png)

4. Right click a ".rofl" file and select **Properties**. If you downloaded a replay from the game client, replays are stored in Documents\League of Legends\Replays.
5. Change the **Opens with** field to "ROFLPlayer.exe"
6. You're done! ROFLPlayer will now be used when opening League of Legends replay files!

Please let me know about any bugs or issues

## FAQ

**Why does this exist?**

I couldn't find any tools that let you view the information in a replay file, so I made one!

**Will this get me banned?**

It does not do anything except copy files and launch the game executable. Nothing is altered, so a ban is not likely.

**Can ROFLPlayer play replays from older patches?**

Unfortunately that is something that ROFLPlayer can't do. With some extra effort it is possible, but it requires you to keep an old installation of the game that is the correct version as the replay.

## ROFLParser

ROFLParser is a class library that is capable of parsing ROFL files. Simply point a file path at it and it will return an ReplayHeader object containing all information inside the replay header.
