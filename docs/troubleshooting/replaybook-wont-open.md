---
title: ReplayBook will not open
description: Troubleshooting
---

## Ensure .NET is installed

ReplayBook will not start at all if .NET 8 is not installed properly. Please download and install from the link below, then try ReplayBook again.

[.NET Desktop Runtime 8.0.x (x64)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

![Download Screenshot](../images/downloads_netRequirements.png)

## Completely reset configuration

Go to where you saved ReplayBook and delete the "cache" folder. If that doesn't work try deleting "appsettings.json" and "executablesettings.json". You will lose your settings, but it may fix the problem.

![Delete temporary files](../images/troubleshooting/2_delete_temporary.png)

---

## Need Additional Help?

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
[Report an Issue :material-bug:](https://github.com/fraxiinus/ReplayBook/issues/new/choose){ .md-button .md-button }
