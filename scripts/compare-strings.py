import argparse
import os
import xml.etree.ElementTree as ET

def dir_path(string):
    if os.path.isdir(string):
        return string
    else:
        raise NotADirectoryError(string)

parser = argparse.ArgumentParser(description="Copies missing strings from a main resource file to others")
parser.add_argument("directory", metavar="DIR", type=dir_path, help="directory where all resource files are")
parser.add_argument("main", metavar="MAIN_FILE", help="file to compare all other files to")

args = parser.parse_args()

if not os.path.exists(args.main):
    parser.print_help()
    exit()

allStringRes = []
mainRoot = ET.parse(args.main).getroot()
for stringRes in mainRoot:
    # print(stringRes.attrib["{http://schemas.microsoft.com/winfx/2006/xaml}Key"])
    # print(stringRes.tag)
    allStringRes.append(stringRes)

print(f"loaded {len(allStringRes)} strings from main file")

for fileName in os.listdir(args.directory):
    resFile = os.path.join(args.directory, fileName)
    # do not edit main file
    if resFile == args.main:
        continue

    print(f"--- reading {fileName} ---")
    resTree = ET.parse(resFile)
    resRoot = resTree.getroot()
    for mainString in allStringRes:
        stringKey = mainString.attrib['{http://schemas.microsoft.com/winfx/2006/xaml}Key']
        xpath = ".//*[@{http://schemas.microsoft.com/winfx/2006/xaml}Key='" + stringKey + "']"
        if resRoot.find(xpath) is None:
            print(f"{stringKey} is missing from {fileName}")