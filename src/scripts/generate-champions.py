# Load champion data from data dragon to file

import os
from urllib import response
import requests
import json
import shutil

# add additional languages here:
# see https://ddragon.leagueoflegends.com/cdn/languages.json for valid languages
languages = ["en_US", "de_DE", "zh_CN", "es_ES", "fr_FR", "pt_BR"]

dataDragonBaseUrl = "http://ddragon.leagueoflegends.com/"

print("requesting latest version of data dragon...")
versionsReq = requests.get(url=dataDragonBaseUrl+"api/versions.json")
versionsReq.raise_for_status()

latestVersion = (versionsReq.json())[0]

# make destination folder
if not os.path.exists("champions"):
    os.makedirs("champions")

# download all sprites
# will never loop to 9 but i need a counter and im lazy
for atlasCounter in range(10):
    print("requesting sprite atlas " + str(atlasCounter))
    # get sprite sheet
    atlasReq = requests.get(url=dataDragonBaseUrl+"cdn/" +
                            latestVersion+"/img/sprite/champion"+str(atlasCounter)+".png", stream=True)
    # stop loop if request doesn't exist
    if atlasReq.status_code != 200:
        print("failed, stopping here")
        break
    # otherwise save results to file
    with open("champions/champion"+str(atlasCounter)+".png", "wb") as out_file:
        atlasReq.raw.decode_content = True
        shutil.copyfileobj(atlasReq.raw, out_file)
    del atlasReq

for lang in languages:
    print("requesting datadragon for " + lang)
    # get item data
    itemReq = requests.get(url=dataDragonBaseUrl+"cdn/" +
                           latestVersion+"/data/" + lang + "/champion.json")
    itemReq.raise_for_status()

    # parse json
    itemJson = itemReq.json()

    # results list
    items = []
    for id in itemJson["data"]:
        idJson = itemJson["data"][id]
        # save data we want
        x = {
            "id": id,
            "name": idJson["name"],
            "image": {
                "source": idJson["image"]["sprite"],
                "x": idJson["image"]["x"],
                "y": idJson["image"]["y"],
                "w": idJson["image"]["w"],
                "h": idJson["image"]["h"]
            }
        }
        # add to results
        items.append(x)
    
    outputJson = {
        "atlasCount": atlasCounter,
        "data": items
    }
    # save results to file
    with open("champions/" + lang + ".data.json", 'w', encoding="utf-8") as outfile:
        json.dump(outputJson, outfile, indent=4, ensure_ascii=False)
