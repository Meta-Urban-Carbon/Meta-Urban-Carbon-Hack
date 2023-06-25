import json

class project:
    def __init__(self, name, path, zipcode, moveInYear):
        self.name = name
        self.path = path
        self.zipcode = zipcode
        self.moveInYear = moveInYear
        self.operationalYears = 50
        self.unit = "ip"
        self.country = "USA",
        self.postalCode = "60190"
        self.city = "Seattle"
        self.state = "WA"
        self.projectRegion = "West"
        self.buildings = []
        self.landscape = None

    def add_building(self, building):
        self.buildings.append(building)

class building:
    def __init__(self, name):
        self.name = name
        self.programs = []

    def addProgramToBuilding(self, program):
        self.programs.append(program)
        return self.programs
    
    def totalElectricityConsumption(self):
        totalElectricityConsumption = 0
        for program in self.programs:
            totalElectricityConsumption += program.baselineElectricityConsumption()
        return totalElectricityConsumption
    
    def totalNaturalGasConsumption(self):
        totalNaturalGasConsumption = 0
        for program in self.programs:
            totalNaturalGasConsumption += program.baselineNaturalGasConsumption()
        return totalNaturalGasConsumption

class buildingProgram:
    def __init__(self, name, programName, area):
        self.name = name
        self.programName = programName
        self.area = area
        self.areaUnits = "ftSQ"
        self.reportingUnits = "us"
        # self.country = "USA",
        # self.postalCode: "60190"
        # self.state = "IL"
        # self.reportingUnits = "us"
        self.projectRegion = "West"
        self.baselineEnergy = self.baselineEnergyConsumption()
        self.baselineElectricity = self.baselineElectricityConsumption()


    # def __str__(self):
    #     return f"{self.name} {self.path}"

    # def __repr__(self):
    #     return f"{self.name} {self.path}"
    
    def areaForProgram (self):
        return print("area for " + self.programName + " is " + self.area)

    def baselineEUI (self):
        dummyEUI = int(100)
        return dummyEUI
    
    def baselineEnergyConsumption (self):
        programEnergyConsumption = int(self.area) * int(self.baselineEUI())
        return programEnergyConsumption
    
    def baselineElectricityConsumption (self):
        fp="data\\useTypes.json"
        with open(fp, 'r') as f:
            useTypes = json.load(f)
        adjustmentFactor = useTypes.get(self.programName, {}).get(self.projectRegion, 1)
        electricConsumption =  self.baselineEnergy * adjustmentFactor
        return electricConsumption

    def baselineNaturalGasConsumption (self):
        NGConsumption =  self.baselineEnergy - self.baselineElectricity
        return NGConsumption
    
    # def baselineEUI(self):
        
programOffice = buildingProgram("Workspace", "Office", "1000")
# print(programOffice.baselineElectricityConsumption())
# print(programOffice.baselineNaturalGasConsumption())

programHospital = buildingProgram("New Hospital", "Hospital", "5000")
# programTest.baselineEnergyConsumption()
# print(programHospital.baselineElectricityConsumption())
# print(programHospital.baselineNaturalGasConsumption())

programMixedUse = buildingProgram("Mixed Use", "Mixed Use Property", "60000")
# print(programMixedUse.baselineElectricityConsumption())
# print(programMixedUse.baselineNaturalGasConsumption())

mixedUseBuilding = building("Mixed Use Building")
hospitalBuilding = building("Hospital Building")

mixedUseBuilding.addProgramToBuilding(programOffice)
mixedUseBuilding.addProgramToBuilding(programMixedUse)
hospitalBuilding.addProgramToBuilding(programHospital)

print("Mixed Use Building office elec ", programOffice.baselineElectricityConsumption(), "mixed use elec ", programMixedUse.baselineElectricityConsumption(), mixedUseBuilding.totalElectricityConsumption())
print("Mixed Use Building Office NG ", programOffice.baselineNaturalGasConsumption(), "MU NG ", programMixedUse.baselineNaturalGasConsumption(), mixedUseBuilding.totalNaturalGasConsumption())
print("Hospital Building elec ", programHospital.baselineElectricityConsumption(), hospitalBuilding.totalElectricityConsumption())
print("Hospital Building NG ", programHospital.baselineNaturalGasConsumption(), hospitalBuilding.totalNaturalGasConsumption())

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


