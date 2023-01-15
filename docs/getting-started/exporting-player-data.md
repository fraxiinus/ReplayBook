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

## Need Help?

[Check the Troubleshooting page](../../troubleshooting)

[Ask in GitHub :material-github:](https://github.com/fraxiinus/ReplayBook/discussions){ .md-button .md-button }
[Join the Discord :material-chat:](https://discord.gg/c33Rc5J){ .md-button .md-button }
[Report an Issue :material-bug:](https://github.com/fraxiinus/ReplayBook/issues/new/choose){ .md-button .md-button }
