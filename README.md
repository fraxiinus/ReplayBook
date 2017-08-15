# ROFL-Player

![Screenshot](http://i.imgur.com/CjOu3ot.png)


This application is very simple. Follow these steps to set it up:

- Launch the program and direct it to the League of Legends.exe file. Something like C:\Riot Games\League of Legends\RADS\solutions\lol_game_client_sln\releases\0.0.1.179\deploy\League of Legends.exe
- That's it, you can now choose a replay to run. The application also remembers where the League of Legends executable is next time you open it.
- If you tell Windows to open .rofl files with this application, it will automatically start playing if the League of Legends.exe is valid!
- If you run the program as an administrator, ROFL-Player will use symlinks instead of copying the replays.

Please let me know about any bugs or issues

## FAQ

**Why does this exist?**

"You can just copy the replay into the folder containing League of Legends.exe and drag the replay onto the executable! What's the point of this?"

That is exactly what this program does, I'm lazy.

**What is 'instance.tmp'?**

That is the file created by ROFL-Player to pass information from attempts to open new instances. It is not important and can be deleted.

**Will this get me banned?**

It does not do anything except copy files and launch the game executable. Nothing is altered, so a ban is not likely.
