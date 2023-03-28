---
title: Advanced search techniques
description: Getting started
---

ReplayBook utilizes [Lucene.NET](https://lucenenet.apache.org/) for its search functionality.
The full query syntax can be found [here](https://lucenenet.apache.org/docs/4.8.0-beta00016/api/queryparser/Lucene.Net.QueryParsers.Classic.html).

!!! failure
    This feature has not been released yet! Provide your feedback at the links below!

## Available Fields

| Name         | Description                                                                                          | Example                                                                |
| ------------ | ---------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| id           | The full file path of a replay - Only supports full text matches                                     | E:\User\Documents\League of Legends\Replays\NA1-999999.rofl            |
| baseKeywords | The default field used for basic searches - A comma separated list of all player names and champions | wifienyabledcat Nidalee, redmagemorgan Akali, Shining Hope Ezreal, ... |
| red          | A comma separated list of player names and champions on the red team                                 | -                                                                      |
| blue         | A comma separated list of player names and champions on the blue team                                | -                                                                      |
| replayName   | The name of the replay - Only supports full text matches                                             | NA1-999999.rofl                                                        |
| createdDate  | Replay file created date, in ticks                                                                   | -                                                                      |
| fileSize     | Replay file size, in bytes                                                                           | -                                                                      |

!!! note
    Champion names use the internal game names. Usually it is enough to remove spaces and special characters. For example, "Miss Fortune" becomes "MissFortune" and "Wukong" becomes "MonkeyKing"

## Search Strictness

Search results are filtered by a score determined by how well it matches the query.
This filter can be adjusted to a lower value if the results are being constrained, or to a higher value if non-relevant results are returning.

![Search Strictness](../images/search_0.png)

## Example Queries

### Query player and champion

```plaintext
"wifienyabledcat Yuumi"
```

The quotes ensure that only replays where `wifienyabledcat` is playing `Yuumi` are returned.

Use the `AND` operator to query two players:

```plaintext
"wifienyabledcat Nami" AND Shavisi
```

And to specify champions:

```plaintext
"wifienyabledcat Yuumi" AND "Shavisi Zeri"
```

### Query matchups

Look for replays that have Nami against Senna:

```plaintext
(blue:Nami AND red:Senna) OR (red:Nami AND blue:Senna)
```

Player names can also be specified:

```plaintext
(blue:"Shavisi Nami" AND red:Senna) OR (red:"Shavisi Nami" AND blue:Senna)
```

Search only player matchups:

```plaintext
(blue:Shavisi AND red:Etirps) OR (red:Shavisi AND blue:Etirps)
```

---

## Need Help?

[Check the Troubleshooting page](../../troubleshooting)

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
[Report an Issue :material-bug:](https://github.com/fraxiinus/ReplayBook/issues/new/choose){ .md-button .md-button }
