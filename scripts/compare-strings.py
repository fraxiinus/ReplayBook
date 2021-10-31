import argparse
import os
import json
import xml.etree.ElementTree as ET

def dir_path(string):
    if os.path.isdir(string):
        return string
    else:
        raise NotADirectoryError(string)

parser = argparse.ArgumentParser(prog="compare-strings.py", description="Compares strings from a main resource file to others")
parser.add_argument("directory", metavar="<resource directory>", type=dir_path, help="directory where all resource files are")
parser.add_argument("main", metavar="<reference file>", help="file to compare all other files to")
parser.add_argument("-format", metavar="<choice>", choices=["summary", "json", "csv"], default="summary", help="result format choices: 'summary', 'json', 'csv' (default: 'summary')")

args = parser.parse_args()

# check if reference file exists
if not os.path.exists(args.main):
    print("reference file does not exist")
    parser.print_help()
    exit()

# raw strings from reference file
allStringRes = []
mainRoot = ET.parse(args.main).getroot()
for stringRes in mainRoot:
    allStringRes.append(stringRes)

print(f"loaded {len(allStringRes)} strings from main file")

# store results
results = {}
# get all files in resource directory
for fileName in os.listdir(args.directory):
    resFile = os.path.join(args.directory, fileName)
    # skip reference file
    if resFile == args.main:
        continue

    print(f"reading {fileName}...")
    results[fileName] = []

    # start comparing strings
    resTree = ET.parse(resFile)
    resRoot = resTree.getroot()
    for mainString in allStringRes:
        # find every string attribute (it's weird because of how python opens the file)
        stringKey = mainString.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key']
        xpath = ".//*[@{http://schemas.microsoft.com/winfx/2006/xaml}Key='" + stringKey + "']"
        # if reference key does not exist in file
        if resRoot.find(xpath) is None:
            results[fileName].append(stringKey)
            #print(f"{stringKey} is missing from {fileName}")
print("- - - - - - - - - - - - - - -")
if args.format == "summary":
    # output percentages
    print("")
    print("Current strings status:")
    totalStringCount = len(allStringRes)
    columnKey = os.path.basename(args.main)
    columnData = str(totalStringCount) + "/" + str(totalStringCount)
    for resFile in results:
        missingCount = len(results[resFile])
        columnKey += "\t" + resFile
        columnData += "\t" + str(totalStringCount - missingCount) + "/" + str(totalStringCount)
    print(columnKey)
    print(columnData)
elif args.format == "json":
    # output json
    json_object = json.dumps(results, indent=4)
    print(json_object)
elif args.format == "csv":
    # output csv
    csvLines = []
    # construct table output
    firstLine = "key, " + os.path.basename(args.main)
    for fileName in results:
        firstLine += ", " + fileName
    csvLines.append(firstLine)

    for refString in allStringRes:
        key = refString.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key']
        newLine = key + ", ✔"

        # loop over all files in results
        for resFile in results:
            missingStrings = results[resFile]
            if key in missingStrings:
                newLine += ", ❌"
            else:
                newLine += ", ✔"
        csvLines.append(newLine)

    for line in csvLines:
        print(line)