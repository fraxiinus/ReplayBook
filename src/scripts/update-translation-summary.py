import argparse
import os
import json
import re
import datetime

parser = argparse.ArgumentParser(prog="compare-strings.py", description="Compares strings from a main resource file to others")
parser.add_argument("status", metavar="<status file>", help="contains json of translation status")
parser.add_argument("target", metavar="<wiki page>", help="wiki page to update")

args = parser.parse_args()

# check if files exists
if not os.path.exists(args.status):
    print("status file does not exist")
    parser.print_help()
    exit(1)
if not os.path.exists(args.target):
    print("target file does not exist")
    parser.print_help()
    exit(1)

# load status data from compare-strings.py
statusData = {}
with open(args.status) as statusFile:
    statusData = json.load(statusFile)

# load wiki page as string
targetData = ""
with open(args.target, encoding="utf-8") as targetFile:
    targetData = targetFile.read()

# find and replace value between language tags
for lang in statusData:
    targetData = re.sub("(?<=<fx-" + lang + ">)(.*?)(?=</fx-" + lang + ">)", str(statusData[lang]), targetData)
# update last updated value
targetData = re.sub("(?<=<updated>)(.*?)(?=</updated>)", datetime.datetime.utcnow().isoformat() + "Z", targetData)

# write updated page
with open("translationUpdatedSummary.md", "w", encoding="utf-8") as outFile:
    outFile.write(targetData)