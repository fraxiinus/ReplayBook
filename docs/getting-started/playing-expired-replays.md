---
title: Playing Expired Replays
description: Getting started
---

It is still possible to play expired replays using ReplayBook. The catch is that it requires a backup of the exact patch the replay is from. **You cannot play a replay otherwise!**

## Getting a Backup

If you want full compatibility with your replays going forward, it is recommended to do full backups of your League of Legends installation whenever a new patch is released. Otherwise, you will have to find and download a 10 GB backup file from somewhere else.

!!! note
    When saving backups, only the 'Game' folder is required. The League Client is not required for playing back replays.

---

## Registering Additional Executables

ReplayBook first needs to be informed that backup patches exist:  
Open the **Settings Window** by clicking the gear icon next to the sort button. Then navigate to the 「Executables」 settings tab.

![Executable Settings Tab](../images/old_replays_0.png)

**Executable Source Folders** are locations ReplayBook will search to find League of Legends backups. It includes subfolders as well.  
**Registered Executables** are patches that ReplayBook knows about. The program checks this list in order to determine if a replay is playable.

Add the folder where you are keeping your backups, then press the 「Scan Folders for Executables」 button.

!!! note
    Network folders can also be added as an Executable Source Folder, however registering too many executables over the network can impact performance.

![Register Executables](../images/old_replays_1.png)

Whenever you add a new backup, you have to come back here and hit the 「Scan」 button again.

---

## Useful Tools and Websites

* [league-vcs](https://github.com/preyneyv/league-vcs) is a version control system for League of Legends. It automatically creates backups and saves storage space by only saving the difference between patches.  
Direct integration with ReplayBook is currently planned. ([#119](https://github.com/fraxiinus/ReplayBook/discussions/119))
* [replays.xyz](https://replays.xyz/old-clients) offers online backups of League of Legends patches. Be sure to follow the important instructions!

---

[Next: Exporting player data](../getting-started/exporting-player-data.md){ .md-button .md-button--primary }

---

## Need Help?

[Check the Troubleshooting page](../troubleshooting/index.md)

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
[Report an Issue :material-bug:](https://github.com/fraxiinus/ReplayBook/issues/new/choose){ .md-button .md-button }
