# Common Problems and Solutions

## Table of Contents

- [Common Problems and Solutions](#common-problems-and-solutions)
  - [Table of Contents](#table-of-contents)
  - [Play button is disabled/Cannot play this replay](#play-button-is-disabledcannot-play-this-replay)
  - [ReplayBook is not loading any replays](#replaybook-is-not-loading-any-replays)
    - [How to add a replay folder](#how-to-add-a-replay-folder)
  - [Settings are not saving](#settings-are-not-saving)
  - [ReplayBook will not open](#replaybook-will-not-open)
  - [Playing replays causes a Bugsplat/Language is not showing properly](#playing-replays-causes-a-bugsplatlanguage-is-not-showing-properly)
    - [How to change an executable's locale](#how-to-change-an-executables-locale)
  - [Playing replays crashes with missing stub.dll](#playing-replays-crashes-with-missing-stubdll)
  - [Playing replays crashes with critical error](#playing-replays-crashes-with-critical-error)
  - [League of Legends "Failed to open replay. The game will now exit." error](#league-of-legends-failed-to-open-replay-the-game-will-now-exit-error)
  - [Need Additional Help?](#need-additional-help)

## Play button is disabled/Cannot play this replay

![Can not play replay](../images/troubleshooting/5_can_not_play_replay.png)
![Unsupported replay](../images/troubleshooting/7_unsupported_replay.png)

Make sure you have registered your installation(s) of League of Legends with ReplayBook. **In order to play a replay from an old patch, you need a copy of that exact patch on your computer.** If you do not have a backup, try looking for a download online, or from someone you know.

> Double check if you **really** have a patch. In the **Registered Executables** list, each item has the name on the left and the patch on the right. You might have a situation where the executable is named "Patch 10.22" but the actual patch version is something else.

[Learn more about playing expired replays](../tutorial/4_export_data.md)

## ReplayBook is not loading any replays

Make sure you have your replay folder added to ReplayBook. Otherwise the the program won't know where to look.

> The default location for replays is "C:\Users\username\Documents\League of Legends\Replays".

### How to add a replay folder

1. Go to **Settings** (gear icon) -> **Replays** -> **Add** button under **Replay Source Folders**.
2. Close the settings window and ReplayBook will search your newly added folder.

![Add Replay Source Folder](../images/troubleshooting/0_add_replay_source_folder.png)

## Settings are not saving

Check if the ReplayBook folder is read-only. ReplayBook needs to be able to create and edit files in that folder in order to save its settings.

![Folder Read Only](../images/troubleshooting/1_folder_read_only.png)

## ReplayBook will not open

Go to where you saved ReplayBook and delete the **cache** folder. If that doesn't work try deleting **appsettings.json** and **executablesettings.json**. You will lose your settings, but it may fix the problem.

![Delete temporary files](../images/troubleshooting/2_delete_temporary.png)

## Playing replays causes a Bugsplat/Language is not showing properly

Check that you have the **locale** set correctly for your League of Legends executable. By default, ReplayBook tries its best to detect the correct locale. If you game is in a different locale, you need to tell ReplayBook:

### How to change an executable's locale

1. Go to **Settings** (gear icon) -> **Executables** -> **Edit** your executable under **Register Executables**
2. Correct the **Locale** value.

> If you downloaded a backup from replays.xyz, change the locale to 'EnglishUS (en_US)'

![Change Executable Locale](../images/troubleshooting/3_change_executable_locale.png)

## Playing replays crashes with missing stub.dll

In order to play replays, **ReplayBook needs the entire League of Legends installation, not just League of Legends.exe**. To make sure your installation is complete, you can try re-installing the game, or downloading someone else's backup.

![Missing Executable Files](../images/troubleshooting/4_missing_executable_files.png)

## Playing replays crashes with critical error

Double check the League of Legends executable you are using. This error is caused by problems with League of Legends, perhaps a missing file.

> If you downloaded a backup from replays.xyz, try extracting it again. If that doesn't work, try downloading it again.

![League Error screenshot](../images/troubleshooting/8_league_error.png)

## League of Legends "Failed to open replay. The game will now exit." error

This is a generic error from League of Legends that indicate something is wrong with the installation. Check the following:

1. Does your Windows username have accent marks (for example: â, ê, ô) or any other special characters?  
If you do, you will need to rename your Windows user folder, or create a new user that does not have any accent marks in the name.
2. Is the replay file corrupted? Try giving it to someone else and see if they can play it successfully.
3. Is the League of Legends installation incomplete? Try using a backup from replays.xyz and see if it works.

## Need Additional Help?

ReplayBook saves logs if an error occurs. Check the "ReplayBook\logs" folder and be sure to include it when asking for help.

[Ask about it in the GitHub Discussions page](https://github.com/fraxiinus/ReplayBook/discussions),  
[Or ask in the developer Discord](https://discord.gg/c33Rc5J).  
[Report an issue with this page](https://github.com/fraxiinus/ReplayBook/issues/new/choose).
