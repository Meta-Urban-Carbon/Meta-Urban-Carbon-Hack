import csv
import json


def csv_to_json(csv_file_path, json_file_path):
    with open(csv_file_path, "r") as csv_file:
        csv_data = csv.DictReader(csv_file)

        json_data = []

        for row in csv_data:
            json_data.append(row)

    with open(json_file_path, "w") as json_file:
        json.dump(json_data, json_file, indent=4)

csv_file_path = "C:\Users\schuy\Documents\GitHub\Meta-Urban-Carbon-Hack\data\cambiumLMERMidCase.csv"
json_file_path = "C:\Users\schuy\Documents\GitHub\Meta-Urban-Carbon-Hack\data\csv_to_json.json"

a = csv_to_json(csv_file_path, json_file_path)