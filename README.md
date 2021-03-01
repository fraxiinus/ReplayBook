![Banner](.github/logo/banner_rev2_fullsize.png "ReplayBook Banner")
![DarkMode](.github/screenshots/overview_dark.png "Screenshot")

[![Build](https://img.shields.io/github/workflow/status/fraxiinus/ReplayBook/Build?style=flat-square)](https://github.com/fraxiinus/ReplayBook/actions?query=workflow%3ABuild)
[![License](https://img.shields.io/github/license/fraxiinus/ReplayBook?style=flat-square)](https://github.com/fraxiinus/ReplayBook/blob/master/LICENSE)
[![Donate](https://shields.io/badge/ko--fi-support%20me-green?logo=ko-fi&style=flat-square)](https://ko-fi.com/fraxiinus)
### ReplayBook is a free open-source tool that helps you organize and manage replays.

### This project is currently looking for translators! [View the wiki for more information](https://github.com/fraxiinus/ReplayBook/wiki/Translating).

## Join the Discord if you need help or have any suggestions

[![Discord Banner](https://discordapp.com/api/guilds/606263917211156501/widget.png?style=banner2)](https://discord.gg/c33Rc5J)


## Features
* Keep old League of Legends patches and ReplayBook will automatically detect and match compatible replay files
* Assign player markers to easily spot players in the replay list
* View all information available in replay files
* Export data to CSV or JSON file formats
* Supports all regions!
* Windows 10 dark/light themes supported

## Getting Started
### [YouTube Tutorial](https://youtu.be/mOpoyZuVyzs)
### Tutorial
1. Get the latest release of ReplayBook ([download link](https://github.com/fraxiinus/ReplayBook/releases))
2. Extract the program and run ReplayBook.exe, you will be prompted with a first time setup.
3. Follow the on-screen instructions. It is *highly recommended* to download all images as it makes ReplayBook more responsive.
5. *(optional)* Add any other folders where you keep backups for old League of Legends versions by opening the settings window (Gear icon) -> Executables -> **Add** button under Executable Source Folders. Once you have added your folders, press the **Scan Folders** button and ReplayBook will automatically add your backups.
6. *(optional)* Add any other folders where you keep replay files by going to the settings menu (Gear icon), and adding folders under Replays -> Replay Source Folders.

## Troubleshooting

**Playing replays causes a Bugsplat**

Check that you have the locale set correctly for your League of Legends executable. By default, ReplayBook uses the locale you selected during the setup process. If you game is in a different locale, you need to tell ReplayBook by going to the settings (Gear icon) -> Executables -> Edit your executable under `Register Executables` and change the `Locale` value to your language.

**Playing replays crashes with missing stub.dll**

In order to play replays, ReplayBook needs the entire League of Legends installation, not just `League of Legends.exe`. To make sure your installation is complete, you can try re-installing the game, or downloading someone else's backup.

[View full troubleshooting document](https://github.com/fraxiinus/ReplayBook/wiki/Troubleshooting)

## Building
### Requirements
* Visual Studio 2019
* .NET Framework v4.7.2

Use the provided solution file.
