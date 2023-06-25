import json
import requests

class project:
    def __init__(self, path):
        self.name = name
        self.zipcode = zipcode
        self.moveInYear = moveInYear
        self.operationalYears = 50
        self.unit = "ip"
        self.postalCode = zipcode
        self.city = "Seattle"
        self.state = "WA"
        self.country = "USA"
        self.projectRegion = self.assignProjectZone()
        self.buildings = []
        self.landscape = None
    
    

    def addBuilding(self, building):
        self.buildings.append(building)
    
    def assignProjectZone(self):
        fp="data\\zones.json"
        with open(fp, 'r') as f:
            zones = json.load(f)
        for zone, states in zones.items():
            # print(zone)
            # print(states)
            if self.state in states:
                return zone
            else: None

class Site_features:
    def __init__(self, gfa, lpd):
        self.gfa = gfa
        self.lpd = lpd
        self.total_energy = self.gfa * self.lpd

class building:
    def __init__(self, name):
        self.name = name
        self.programs = []
        self.buildingArea = 0
        self.baselineElectricityConsumption = 0
        self.baselineNaturalGasConsumption = 0
        self.BaselineEnergyConsumption = 0
        self.baselineEUI = 0

    def addProgramToBuilding(self, program):
        self.programs.append(program)
        self.baselineElectricityConsumption += int(program.baselineElectricity)
        self.baselineNaturalGasConsumption += int(program.baselineNaturalGas)
        self.BaselineEnergyConsumption += int(program.baselineEnergy)
        self.buildingArea += int(program.area)
        self.buildingEUI = self.BaselineEnergyConsumption / self.buildingArea
        return self.programs
    
    def operationalCarbonProjection(self):
        cambiumFactor = self.cambiumFactor()
        return self.BaselineEnergyConsumption * 0.000000272
    
    import json

    def cambiumFactor(state, year):
        with open('cambiumLMERMidCase.json', 'r') as f:
            data = json.load(f)
        for entry in data:
            if entry['state'] == state and entry['year'] == year:
                return entry['factor']
        return 1


class buildingProgram:
    def __init__(self, name, programName, area):
        self.name = name
        self.programName = programName
        self.area = area
        self.areaUnits = "ftSQ"
        self.reportingUnits = "us"
        self.projectRegion = "West"
        self.baselineEnergy = self.baselineEnergyConsumption()
        self.baselineElectricity = self.baselineElectricityConsumption()
        self.baselineNaturalGas = self.baselineNaturalGasConsumption()
    
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
programHospital = buildingProgram("New Hospital", "Hospital", "5000")
programMixedUse = buildingProgram("Mixed Use", "Mixed Use Property", "60000")

mixedUseBuilding = building("Mixed Use Building")
hospitalBuilding = building("Hospital Building")

mixedUseBuilding.addProgramToBuilding(programOffice)
mixedUseBuilding.addProgramToBuilding(programMixedUse)
hospitalBuilding.addProgramToBuilding(programHospital)

newProject = project("New Project",  "98115", "2025")
newProject.addBuilding(mixedUseBuilding)
newProject.addBuilding(hospitalBuilding)

print("MU Building elec ", mixedUseBuilding.baselineElectricityConsumption, "MU Building NG ", mixedUseBuilding.naturalGasConsumption)
print("Hospital Building elec ", hospitalBuilding.baselineElectricityConsumption, "Hospital Building NG ", hospitalBuilding.naturalGasConsumption)
print("project = ", newProject.name, "zone = ", newProject.projectRegion, "buildings = ", newProject.buildings, "details = ", newProject.buildings[0].baselineElectricityConsumption, newProject.buildings[1].baselineElectricityConsumption)


def buildZeroToolDataInput():
    return ""
    
    data_input = {
        'buildingType' = project.building.program.programName,
        'GFA': project.building.program.area,
        'areaUnits': 'ftSQ',
        'country': 'USA',
        'postalCode': project.zipcode,
        'state': project.state,
        'city': project.city,
        'reportingUnits': 'us'
    }

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


