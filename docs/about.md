# About and FAQ

## About

## Frequently Asked Questions

### Q: Is it possible to see time based data like CS at 15 minutes or positional data?

**A:** No. It is not possible. The only data that is readable is the "metadata" portion of the replay file. The actual "payload" or guts of the replay is completely unreadable.

Riot Games decided that replay files will just be a dump of the a game's packet data. Packet data is obfuscated in order to deter cheaters. No progress has been made to decode this data because obfuscation is updated every patch. Also Riot would probably send you a cease and desist.

### Q: Are there plans to support other operating systems like MacOS or Linux?

**A:** No. Unfortunately, ReplayBook was created using Microsoft exclusive technologies like WPF. A port to another operating system would require essentially a full UI rewrite.

I also do not own a Mac to do development on.

### Q: Does ReplayBook support means of automating data exporting?

**A:** No. However, I do provide parsers that you can use to create your own solution for batch exporting replay files: [roflxd](https://github.com/fraxiinus/roflxd).

## Additional Questions

Have a question that is not answered here? Contact me at one of the following:

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
