# Issues Playing Replays

There are a lot of potential issues that can cause issues.

## Incorrect Locale

An incorrect **locale** can cause bugsplat errors and text issues.

Check that you have the **locale** set correctly for your League of Legends executable. By default, ReplayBook tries its best to detect the correct locale. If you game is in a different locale, you need to tell ReplayBook:

### How to change an executable's locale

1. Go to **Settings** (gear icon) -> **Executables** -> **Edit** your executable under **Register Executables**
2. Correct the **Locale** value.

!!! note
    If you downloaded a backup from replays.xyz, change the locale to 'EnglishUS (en_US)'

![Change Executable Locale](../images/troubleshooting/3_change_executable_locale.png)

## Playing Replays Crashes With Missing stub.dll

In order to play replays, **ReplayBook needs the entire League of Legends installation, not just League of Legends.exe**. To make sure your installation is complete, you can try re-installing the game, or downloading someone else's backup.

![Missing Executable Files](../images/troubleshooting/4_missing_executable_files.png)

## Playing Replays Crashes With Critical Error

Double check the League of Legends executable you are using. This error is caused by problems with League of Legends, perhaps a missing file.

!!! note
    If you downloaded a backup from replays.xyz, try extracting it again. If that doesn't work, try downloading it again.

![League Error screenshot](../images/troubleshooting/8_league_error.png)

## League of Legends "Failed to open replay. The game will now exit." Error

This is a generic error from League of Legends that indicate something is wrong with the installation. Check the following:

1. Does your Windows username have accent marks (for example: â, ê, ô) or any other special characters?  
If you do, you will need to rename your Windows user folder, or create a new user that does not have any accent marks in the name.
2. Is the replay file corrupted? Try giving it to someone else and see if they can play it successfully.
3. Is the League of Legends installation incomplete? Try using a backup from replays.xyz and see if it works.

