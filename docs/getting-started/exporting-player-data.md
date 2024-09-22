---
title: Exporting Player Data
description: Getting started
---

All readable data in a replay is available for exporting through ReplayBook.

!!! tip
    Exporting data from replays is one of the only ways of getting player data from custom games!

## Using the Export Data Wizard

Open the Export Data Wizard by right-clicking or clicking the three-dot 「More」 button on any replay.

![Export Data Wizard Home](../images/export_0.png)

### 1 - Select players

Click the 「Start wizard」 button. The next screen is where you select which players' data you want to export. You have the option of selecting players manually, only including those with [player markers](using-player-markers.md), or select all.

![Export Data Wizard Players](../images/export_1.png)

### 2 - Select attributes

After selecting your players, the next screen will ask you which attributes to export. Selected items are automatically sorted to the top of the list.

!!! warning
    Keep in mind that this list uses the **original attribute names** that are in the replay. Some things may be named strangely. For example, inhibitors are called 'Barracks' and the Nexus is called 'HQ'.

!!! note
    The square on the right of each item is a preview using the data one of the previously selected players.

![Export Data Wizard Attributes](../images/export_2.png)

### 3 - Final steps

On the last screen, you are given other options for the export as well as a data preview.

!!! note
    Some attributes can only be included if the output format is JSON. This is a limitation of the CSV format.

![Export Data Wizard Final](../images/export_3.png)

---

## Taking Advantage of Presets

Presets are a way to save a set of selected options that can be automatically applied. Players (including 'marker' and 'all' options), attributes, and all other options are included in a preset.

Create a preset by clicking the 「Save as preset」 button on the last screen of the Export Data Wizard. A prompt will appear containing a summary of the preset and a text box to input a name for the preset.

!!! note
    Presets are saved in the folder 'ReplayBook\cache\export_presets'

![Export Data Wizard Preset](../images/export_4.png)

To use a preset, select the 「Export using preset」 option when exporting. A selection window will appear that will also preview the contents of preset. After selecting a preset, clicking the 「Load」 button will load the preset, apply it to the replay, and prompt you where to save the export data file.

---

## Using the Advanced Mode

The Export Data Wizard includes an **Advanced Mode**. This mode shows all exporter options on a single screen, making it ideal for power users. It also includes a live data preview if the window is resized wider to accommodate.

!!! note
    Loading a preset while in **Advanced Mode** does not immediately export the data. It simply applies the saved options.

![Export Data Wizard Advanced](../images/export_5.png)

---

[Next: Get static data bundles](../getting-started/get-static-data-bundles.md){ .md-button .md-button--primary }

---

## Table of available player properties

| Property Name | Description |
| --- | --- |
| AllInPings | |
| Assists | |
| AssistMePings | |
| BaitPings | |
| BaronKills | |
| BarracksKilled | Inhibitors killed |
| BarracksTakedowns | Inhibitor takedowns |
| BasicPings | |
| BountyLevel | |
| ChampionsKilled | |
| ChampionMissionStat0 | Unknown |
| ChampionMissionStat1 | Unknown |
| ChampionMissionStat2 | Unknown |
| ChampionMissionStat3 | Unknown |
| ChampionTransform | Unknown |
| CommandPings | |
| ConsumablesPurchased | |
| DangerPings | |
| DoubleKills | |
| DragonKills | |
| EnemyMissingPings | |
| EnemyVisionPings | |
| Exp | Total experience earned|
| FriendlyDampenLost | Unknown |
| FriendlyHQLost | Friendly Nexus lost |
| FriendlyTurretLost | |
| GameEndedInEarlySurrender | |
| GameEndedInSurrender | |
| GetBackPings | |
| GoldEarned | |
| GoldSpent | |
| HoldPings | |
| HordeKills | Unknown |
| HQKilled | Nexus killed |
| HQTakedowns | Nexus takedowns |
| Id | |
| IndividualPosition | Lane position |
| Item0 | |
| Item1 | |
| Item2 | |
| Item3 | |
| Item4 | |
| Item5 | |
| Item6 | |
| ItemsPurchased | |
| KeystoneId | |
| KillingSprees | |
| LargestAbilityDamage | |
| LargestAttackDamage | |
| LargestCriticalStrike | |
| LargestKillingSpree | |
| LargestMultiKill | |
| LastTakedownTime | |
| Level | |
| LongestTimeSpentLiving | In seconds |
| MagicDamageDealtPlayer | |
| MagicDamageDealtToChampions | |
| MagicDamageTaken | |
| MinionsKilled | |
| MissionsChampionsKilled | |
| MissionsCreepScore | |
| MissionsGoldFromStructuresDestroyed | |
| MissionsGoldFromTurretPlatesTaken | |
| MissionsHealingFromLevelObjects | |
| MissionsMinionsKilled | |
| MissionsTurretPlatesDestroyed | |
| MutedAll | |
| Name | Player name |
| NeedVisionPings | |
| NeutralMinionsKilled | |
| NeutralMinionsKilledEnemyJungle | |
| NeutralMinionsKilledYourJungle | |
| NodeCapture | Unknown, perhaps Dominion or Twisted Treeline |
| NodeCaptureAssist | Unknown, perhaps Dominion or Twisted Treeline |
| NodeNeutralize | Unknown, perhaps Dominion or Twisted Treeline |
| NodeNeutralizeAssist | Unknown, perhaps Dominion or Twisted Treeline |
| NumDeaths | |
| ObjectivesStolen | |
| ObjectivesStolenAssists | |
| OnMyWayPings | |
| PentaKills | |
| Perk0 | Rune 0 Id |
| Perk0Var1 | |
| Perk0Var2 | |
| Perk0Var3 | |
| Perk1 | Rune 1 Id |
| Perk1Var1 | |
| Perk1Var2 | |
| Perk1Var3 | |
| Perk2 | Rune 2 Id |
| Perk2Var1 | |
| Perk2Var2 | |
| Perk2Var3 | |
| Perk3 | Rune 3 Id |
| Perk3Var1 | |
| Perk3Var2 | |
| Perk3Var3 | |
| Perk4 | Rune 4 Id |
| Perk4Var1 | |
| Perk4Var2 | |
| Perk4Var3 | |
| Perk5 | Rune 5 Id |
| Perk5Var1 | |
| Perk5Var2 | |
| Perk5Var3 | |
| PerkPrimaryStyle | |
| PerkSubStyle | |
| PhysicalDamageDealtPlayer | |
| PhysicalDamageDealtToChampions | |
| PhysicalDamageTaken | |
| Ping | Average(?) ping to server |
| PlayersIMuted | |
| PlayersThatMutedMe | |
| PlayerAugment1 | Unknown, Perhaps Area |
| PlayerAugment2 | Unknown, Perhaps Area |
| PlayerAugment3 | Unknown, Perhaps Area |
| PlayerAugment4 | Unknown, Perhaps Area |
| PlayerPosition | |
| PlayerRole | |
| PlayerScore0 | Unknown |
| PlayerScore1 | Unknown |
| PlayerScore10 | Unknown |
| PlayerScore11 | Unknown |
| PlayerScore2 | Unknown |
| PlayerScore3 | Unknown |
| PlayerScore4 | Unknown |
| PlayerScore5 | Unknown |
| PlayerScore6 | Unknown |
| PlayerScore7 | Unknown |
| PlayerScore8 | Unknown |
| PlayerScore9 | Unknown |
| PlayerSubteam | Unknown |
| PlayerSubteamPlacement | Unknown |
| PushPings | |
| PUUID | |
| QuadraKills | |
| RetreatPings | |
| RiftHeraldKills | |
| SightWardsBoughtInGame | |
| Skin | Name of champion |
| Spell1Cast | |
| Spell2Cast | |
| Spell3Cast | |
| Spell4Cast | |
| StatPerk0 | Stat rune |
| StatPerk1 | Stat rune |
| StatPerk2 | Stat rune |
| SummonSpell1Cast | |
| SummonSpell2Cast | |
| Team | |
| TeamEarlySurrendered | |
| TeamObjective | |
| TeamPosition | |
| TimeCCingOthers | Seconds |
| TimeOfFromLastDisconnect | Seconds |
| TimePlayed | Seconds |
| TimeSpentDisconnected | Seconds |
| TotalDamageDealt | |
| TotalDamageDealtToBuildings | |
| TotalDamageDealtToChampions | |
| TotalDamageDealtToObjectives | |
| TotalDamageDealtToTurrets | |
| TotalDamageSelfMitigated | |
| TotalDamageShieldedOnTeammates | |
| TotalDamageTaken | |
| TotalHeal | |
| TotalHealOnTeammates | |
| TotalTimeCrowdControlDealt | |
| TotalTimeCrowdControlDealtToChampions | |
| TotalTimeSpentDead | |
| TotalUnitsHealed | |
| TripleKills | |
| TrueDamageDealtPlayer | |
| TrueDamageDealtToChampions | |
| TrueDamageTaken | |
| TurretsKilled | |
| TurretTakedowns | |
| UnrealKills | |
| VictoryPointTotal | Unknown |
| VisionClearedPings | |
| VisionScore | |
| VisionWardsBoughtInGame | |
| WardKilled | |
| WardPlaced | |
| WardPlacedDetector | Pink wards |
| WasAfk | |
| WasAfkAfterFailedSurrender | |
| WasEarlySurrenderAccomplice | |
| WasLeaver | |
| WasSurrenderDueToAfk | |
| Win | |

## Need Help?

[Check the Troubleshooting page](../troubleshooting/index.md)

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
[Report an Issue :material-bug:](https://github.com/fraxiinus/ReplayBook/issues/new/choose){ .md-button .md-button }
