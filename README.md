# ROFLPlayer

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/leeanchu/ROFL-Player/blob/master/LICENSE)
[![Version](https://img.shields.io/badge/version-0.9c--beta-blue.svg)](https://github.com/leeanchu/ROFL-Player/releases)

ROFLPlayer is a simple Windows program for viewing and playing replay files from League of Legends.

**[Download ROFLPlayer](https://github.com/andrew1421lee/ROFL-Player/releases)**

![Screenshot](https://i.imgur.com/vW562kM.png)

## Features
* View match information for replay files before playing
* Play replay files from other people or games no longer viewable from the client
* Supports multiple installs of League of Legends, keep old installations to play replays from older patches
* View the metadata for old LoLReplay (.LRF) files!
* Save all replay metadata to JSON

## How To Use

1. Download ROFLPlayer and extract the folder to wherever you like.
2. Double-click **ROFLPlayer.exe** to launch the program. ROFLPlayer will attempt to automatically find your League of Legends install folder. You may encounter errors if your League of Legends install is in an odd location.
3. The following window will then appear, Set the player name field so ROFLPlayer can highlight you name when viewing replay details. Set the region field so the program can properly load the match history website when using the "View online" button.

![SettingsWindow](https://i.imgur.com/CA1EHqW.png)

4. (Optional) Add any other League of Legends installations you may have. You can name entries and set if ROFLPlayer should automatically update the entry along with the game. The default executable will be used on play button press and, if set, double click action.

If you have multiple entries, ROFLPlayer will display a drop down menu for the play button.

![AddEntry](https://i.imgur.com/He4htTt.png)

5. Make ROFLPlayer the default app for *.rofl files by right clicking a *.rofl file and setting "Open with" in properties.

6. You're done! ROFLPlayer will now be used when opening League of Legends replay files!

Please let me know about any bugs or issues

## FAQ

**Why does this exist?**

I couldn't find any tools that let you view the information in a replay file, so I made one!

**Does this require an internet connection?**

Yes, and no. ROFLPlayer will run without one, but it won't be able to download champion and item images. ROFLPlayer will cache all the images it downloads, so it won't download twice.

**Will this get me banned?**

It does not do anything except copy files and launch the game executable. Nothing is altered, so a ban is not likely.

**Can ROFLPlayer play replays from older patches?**

Unfortunately that is something that ROFLPlayer can't do without additional work. You can keep older patches around by copy-pasting the League of Legends installation folder before you patch. You can then set that old version as an option in the settings menu for ROFLPlayer.

**What kind of information can you get from a replay?**

You can get nearly everything you can find in the post-match screen. There are some extra information like how many times a spell was cast. However, some data is missing, like summoner spell information. There is also no way to view information about the match at a particular time. To view all the data, use the "Dump JSON" to save all the usable information into a file. 

## ROFLParser

ROFLParser is a class library that is capable of parsing ROFL files. Simply point a file path at it and it will return an ReplayHeader object containing all information inside the replay header.
