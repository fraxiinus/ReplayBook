# Replay Book

![Screenshot](.github/screenshots/overview_light.png "Preview Image")

[![Build](https://img.shields.io/github/workflow/status/fraxiinus/ReplayBook/Build?style=flat-square)](https://github.com/fraxiinus/ReplayBook/actions?query=workflow%3ABuild)
[![License](https://img.shields.io/github/license/fraxiinus/ReplayBook?style=flat-square)](https://github.com/fraxiinus/ReplayBook/blob/master/LICENSE)
[![Discord](https://img.shields.io/discord/606263917211156501?logo=discord&style=flat-square)](https://discord.gg/c33Rc5J)

Replay Book is a replay management tool that is currently in pre-release alpha.

**Join the Discord for progress updates and pre-release builds.**

<br>

## Frequently Asked Questions
* How do I download ReplayBook?
    * ReplayBook is currently in closed alpha. If you'd like to test the alpha, join the Discord server and ask!
* Can you add \*this\* feature?
    * Join the Discord and ask! I'm open to feature requests.
* Can I get \*this\* information?
    * All the information we know how to read from a replay is displayed in the stats view. Stuff like summoner spells, icons, and time based information are not readable from a replay.
* ReplayBook is suddenly having trouble/won't start
    * Go to where you saved ReplayBook and delete the 'cache' folder. If that doesn't work try deleting appsettings.json and executablesettings.json. You will lose your settings, but may resolve your problem.
* I accidentally deleted a replay from within ReplayBook. How do I get it back?
    * Files are only deleted when ReplayBook closes. If you accidentally deleted a file and you didn't close ReplayBook, go to where you saved ReplayBook and open 'cache' then your replay file should be in the 'deletedReplays' folder.
* ReplayBook crashed! What do I do?
    * Send me a message on Discord, and include the log file written in the 'logs' folder where you saved ReplayBook.

## Current Features

![DarkMode](.github/screenshots/overview_dark.png "Dark Mode")

#### Windows 10 dark mode supported!

<br>

![MarkerFeature](.github/screenshots/notes_tooltip.png "Player Markers")

#### Assign player markers to easily spot players in the replay list. For example, you could assign your team different colors to easily see the champions they are playing.

<br>

![StatsFeature](.github/screenshots/statistics_view.gif "View statistics")

#### View every single statistic that is readable from replay files. This includes some weird ones like spell cast counts and mute information.

#### Export data to CSV or JSON file formats to use in data analysis

<br>

![Executables](.github/screenshots/multi_executables.png "Executables")

#### Keep old League of Legends patches and Replay Book will automatically detect and match compatible replay files.

#### Supports all regions! You can assign locale on an executable level, supporting different region installs of League of Legends.
