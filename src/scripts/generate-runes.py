# Load rune data from data dragon into file

import os
import requests
import json

# add additional languages here:
# see https://ddragon.leagueoflegends.com/cdn/languages.json for valid languages
languages = ["en_US", "de_DE", "zh_CN", "es_ES", "fr_FR", "pt_BR"]

# stats runes are not included in Riot's rune list, code them here
# these are found in the CDragon rune list, search for 'statmods'
statsRunes = ["5001", "5002", "5003", "5005", "5007", "5008"]

dataDragonBaseUrl = "http://ddragon.leagueoflegends.com/"
cDragonBaseUrl = "https://raw.communitydragon.org/latest/"

print("requesting latest version of data dragon...")
versionsReq = requests.get(url=dataDragonBaseUrl+"api/versions.json")
versionsReq.raise_for_status()

latestVersion = (versionsReq.json())[0]

for lang in languages:
    print("requesting datadragon for " + lang)
    runeReq = requests.get(url=dataDragonBaseUrl+"cdn/"+latestVersion+"/data/"+lang+"/runesReforged.json")
    runeReq.raise_for_status()

    print("requesting cdragon for " + lang)
    cDragonLang = lang.lower()          # cdragon uses default for en_US
    if lang == "en_US":
        cDragonLang = "default"
    eogReq = requests.get(url=cDragonBaseUrl+"plugins/rcp-be-lol-game-data/global/"+cDragonLang+"/v1/perks.json")
    eogReq.raise_for_status()

    eogJson = eogReq.json()
    runeJson = runeReq.json()

    # load main rune data from ddragon and descriptions from cdragon
    runes = []
    for p in runeJson:
        for s in p["slots"]:
            for r in s["runes"]:
                x = {
                    "id": r["id"],
                    "name": r["name"],
                    "key": r["key"],
                    "icon": r["icon"]
                }
                
                x["endOfGameStatDescs"] = []

                for eogData in eogJson:
                    if (eogData["id"] == r["id"]):
                        # German descriptions have weird artifacts that must be removed
                        for eogDesc in eogData["endOfGameStatDescs"]:
                            desc = eogDesc.replace("&nbsp;", " ")
                            desc = desc.replace("<speed>", "")
                            desc = desc.replace("</speed>", "")
                            x["endOfGameStatDescs"].append(desc)

                runes.append(x)

    # load stats runes from cdragon...
    for eogData in eogJson:
        if str(eogData["id"]) in statsRunes:
            x = {
                "id": eogData["id"],
                "key": eogData["name"],     # key is set as the same as name
                "icon": eogData["iconPath"].replace("/lol-game-data/assets/v1/", ""),
                "endOfGameStatDescs": []
            }

            # theres a bunch of weird symbols in the tooltip
            name = eogData["tooltip"].replace("<scaleAD>", "")
            name = name.replace("</scaleAD>", "")
            name = name.replace("@f1@", "")
            name = name.replace("@f2@", "")
            name = name.replace("&nbsp;", " ")
            x["name"] = name

            runes.append(x)

    if not os.path.exists("runes"):
        os.makedirs("runes")
    with open("runes/"+lang+".data.json", 'w', encoding="utf-8") as outfile:
        json.dump(runes, outfile, indent=4, ensure_ascii=False)
