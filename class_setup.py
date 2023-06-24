class project:
    def __init__(self, name, path, zipcode, moveInYear):
        self.name = name
        self.path = path
        self.zipcode = zipcode
        self.moveInYear = moveInYear
        self.operationalYears = 50
        self.unit = "ip"


    def __str__(self):
        return f"{self.name} {self.path}"

    def __repr__(self):
        return f"{self.name} {self.path}"
    
    def cityState (self, zipcode):
        return print("My zip code is " + zipcode)
    

class buildingProgram:
    def __init__(self, name, type, area):
        self.name = name
        self.type = type
        self.area = area
        self.areaUnits = "ftSQ"
        self.country = "USA",
        self.postalCode: "60190"
        self.state = "IL"
        self.reportingUnits = "us"


    def __str__(self):
        return f"{self.name} {self.path}"

    def __repr__(self):
        return f"{self.name} {self.path}"
    
    def areaForProgram (self):
        return print("area for " + self.type + " is " + self.area)

    def baselineEUI (self):
        dummyEUI = int(100)
        return dummyEUI
    
    def baselineEnergyConsumption (self):
        programEnergyConsumption = int(self.area) * int(self.baselineEUI())
        return print("baseline energy consumption for " + self.type + " is", programEnergyConsumption)
    
    

    
    # def baselineEUI(self):
        

programTest = buildingProgram("Workspace", "Office", "1000")
programTest.baselineEnergyConsumption()

# import requests
# import json

def buildZeroToolDataInput():
    return ""
    
    # data_input = {
    #     'buildingType' = program.Type,
    #     'GFA': above_ip + below_ip,
    #     'areaUnits': 'ftSQ',
    #     'country': 'USA',
    #     'postalCode': project_zipcode,
    #     'state': project_state_code,
    #     'city': project_city,
    #     'reportingUnits': 'us'
    # }

    # {"buildingType": "Office","GFA": 123123,"areaUnits": "ftSQ","country": "USA","postalCode": "60190","state": "IL","reportingUnits": "us"}
    # return data_input

# def getZeroToolData(project_basics, project_details):
#     data_input_objects = []
#     for i in range(len(project_details['buildingFloors'])):
#         data_input_objects.append(buildZeroToolDataInputObject(project_basics, project_details, i))

#     requests_responses = []
#     for data in data_input_objects:
#         response = requests.post('https://tool.zerotool.org/baseline', 
#                                  headers={'Accept': 'application/json', 'Content-Type': 'application/json'},
#                                  data=json.dumps([data]))
#         requests_responses.append(response.json())
        
#     return requests_responses


