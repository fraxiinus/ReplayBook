# Load rune data from data dragon into file

import os
import requests
import json

# add additional languages here:
# see https://ddragon.leagueoflegends.com/cdn/languages.json for valid languages
languages = ["en_US", "de_DE", "zh_CN"]

baseUrl = "http://ddragon.leagueoflegends.com/"

print("requesting latest version of data dragon...")
versionsReq = requests.get(url=baseUrl+"api/versions.json")
versionsReq.raise_for_status()

latestVersion = (versionsReq.json())[0]

for lang in languages:
    print("requesting runes for " + lang)
    runeReq = requests.get(url=baseUrl+"cdn/"+latestVersion+"/data/"+lang+"/runesReforged.json")
    runeReq.raise_for_status()

    runeJson = runeReq.json()

    runes = []
    for p in runeJson:
        for s in p["slots"]:
            for r in s["runes"]:
                x = {
                    "id": r["id"],
                    "name": r["name"],
                    "icon": r["icon"]
                }
                runes.append(x)

    if not os.path.exists("runes"):
        os.makedirs("runes")
    with open("runes/"+lang+".data.json", 'w', encoding="utf-8") as outfile:
        json.dump(runes, outfile, indent=4, ensure_ascii=False)
