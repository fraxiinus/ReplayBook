---
title: About and FAQ
description: Disclaimers, Special Thanks, and FAQ
---

ReplayBook is not endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc.

---

## Special Thanks

[CommunityDragon](https://www.communitydragon.org/) for providing a great resource of League of Legends data.

[Robert Wang](https://github.com/robertabcd) for documenting the ROFL container.

[Community Translators](https://github.com/fraxiinus/ReplayBook/wiki/Translating) for translating ReplayBook to other languages.

[Code Contributors](https://github.com/fraxiinus/ReplayBook/graphs/contributors) for providing improvements to ReplayBook.

---

## Frequently Asked Questions

### Q: Is it possible to see time based data like CS at 15 minutes or positional data?

**A:** No. It is not possible. The only data that is readable is the "metadata" portion of the replay file. The actual "payload" or guts of the replay is completely unreadable.

Riot Games decided that replay files will just be a dump of the a game's packet data. Packet data is obfuscated in order to deter cheaters. No progress has been made to decode this data because obfuscation is updated every patch. Also Riot would probably send you a cease and desist.

### Q: Are there plans to support other operating systems like MacOS or Linux?

**A:** No. Unfortunately, ReplayBook was created using Microsoft exclusive technologies like WPF. A port to another operating system would require essentially a full UI rewrite.

I also do not own a Mac to do development on.

### Q: Does ReplayBook support means of automating data exporting?

**A:** ReplayBook provides [presets](getting-started/exporting-player-data.md#taking-advantage-of-presets) to make data exporting easier, however there is no support for exporting bulk replays. I do provide parsers that you can use to create your own solution for batch exporting replay files: [roflxd](https://github.com/fraxiinus/roflxd).

---

## Additional Questions

Have a question that is not answered here? Contact me at one of the following:

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
