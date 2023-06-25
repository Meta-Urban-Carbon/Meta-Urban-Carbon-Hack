import json
import requests


class Project:
    def __init__(self, json_path):
        with open(json_path, 'r') as f:
            data = json.load(f)

        self.zipcode = data['zipcode']
        self.state = data['state']
        self.city = data['city']
        self.moveInYear = data['moveInYear']
        self.operationalYears = 50
        self.unit = "ip"
        self.projectRegion = self.assignProjectZone()
        self.data = data

        self.buildings = []
        self.add_buildings(data['Buildings'])

    def add_buildings(self, buildings_data):
        for building_data in buildings_data:
            building = Building(building_data, self)
            self.buildings.append(building)

    def assignProjectZone(self):
        fp="data\\zones.json"
        with open(fp, 'r') as f:
            zones = json.load(f)
        for zone, states in zones.items():
            if self.state in states:
                return zone
            else: None

class Building():
    def __init__(self, data, projectData):
        self.name = data['Name']
        self.uniqueID = data['UniqueID']
        self.programs = []
        self.add_programs(data['Programs'], projectData)
        self.buildingEnergyConsumption = self.buildingEnergyConsumption()
        self.buildingElectricityConsumption = self.buildingElectricityConsumption()
        self.buildingNaturalGasConsumption = self.buildingNaturalGasConsumption()
        self.operationalCarbonProjection = self.operationalCarbonProjection(projectData)
        self.buildingArea = self.totalBuildingArea()
        self.buildingEUI = self.buildingEUI()
        self.operationsCEI = self.operationalCarbonIntensity()
        # print("line 40 = ",projectData.projectRegion)

    def add_programs(self, programsData, projectData):
        for program_data in programsData:
            program = Program(program_data, projectData)
            self.programs.append(program)
    
    def totalBuildingArea(self):
        totalBuildingArea = 0
        for program in self.programs:
            totalBuildingArea += int(program.area)
        return totalBuildingArea

    def buildingElectricityConsumption(self):
        buildingElectricityConsumption = 0
        for program in self.programs:
            buildingElectricityConsumption += program.baselineElectricity
        return buildingElectricityConsumption
    
    def buildingNaturalGasConsumption(self):
        buildingNaturalGasConsumption = 0
        for program in self.programs:
            buildingNaturalGasConsumption += program.baselineNaturalGas
        return buildingNaturalGasConsumption
    
    def buildingEnergyConsumption(self):
        buildingEnergyConsumption = 0
        for program in self.programs:
            buildingEnergyConsumption += program.baselineEnergy
            # buildingEnergyConsumption is in kBTU
        return buildingEnergyConsumption
    
    def buildingEUI(self):
        buildingEUI = self.buildingEnergyConsumption / int(self.buildingArea)
        return buildingEUI
    
    def operationalCarbonProjection(self, projectData):
        cambiumFactor = self.cambiumFactor(projectData.state, projectData.moveInYear)
        # print("cambiumFactor = ", cambiumFactor)
        # print("buildingEnergyConsumption = ", self.buildingEnergyConsumption)
        buildingElectricEmissions = roundNumbers(self.buildingElectricityConsumption * cambiumFactor * 0.000293015)
        buildingNGEmissions = roundNumbers(self.buildingNaturalGasConsumption/1000*117)
        buildingOperationalCarbon = roundNumbers(buildingElectricEmissions + buildingNGEmissions)
        return (buildingElectricEmissions,  buildingNGEmissions, buildingOperationalCarbon)
    
    def operationalCarbonIntensity(self):
        buildingOperationalCarbonIntensity = self.operationalCarbonProjection[-1] / self.buildingArea
        return buildingOperationalCarbonIntensity
    

    def cambiumFactor(self, state, year):
        with open('data\\cambiumLMERMidCase.json', 'r') as f:
            data = json.load(f)
        for entry in data:
            if entry['state'] == state and entry['year'] == year:
                return entry['factor']
        return 1

class Program():
    def __init__(self, data, projectData):
        self.programName = data['Program Name']
        self.area = data['Area']
        self.medianSiteEUI = self.siteEUIFromZT(projectData)
        self.baselineEnergy = self.baselineEnergyConsumption()
        self.baselineElectricity = self.baselineElectricityConsumption(projectData.projectRegion)
        self.baselineNaturalGas = self.baselineNaturalGasConsumption()
        

    def areaForProgram (self):
        return print("area for " + self.programName + " is " + self.area, self.zipcode)

    # def baselineEUI (self):
    #     dummyEUI = int(100)
    #     return dummyEUI
    
    def baselineEnergyConsumption (self):
        print("useType = ", self.programName , "medianSiteEUI = ", self.medianSiteEUI)
        programEnergyConsumption = int(self.area) * int(self.medianSiteEUI)
        # programEnergyConsumption is in kBTU
        return programEnergyConsumption
    
    def baselineElectricityConsumption (self, projectRegion):
        fp="data\\useTypes.json"
        with open(fp, 'r') as f:
            useTypes = json.load(f)
        adjustmentFactor = useTypes.get(self.programName, {}).get(projectRegion, 1)
        electricConsumption =  self.baselineEnergy * adjustmentFactor
        # electricConsumption is in kBTU
        return electricConsumption

    def baselineNaturalGasConsumption (self):
        NGConsumption =  self.baselineEnergy - self.baselineElectricity
        # NGConsumption is in kBTU
        return NGConsumption
       

    def buildZeroToolDataInput(self, projectData):
    
        data_input = {
            'buildingType': self.programName,
            'GFA': self.area,
            'areaUnits': 'ftSQ',
            'country': 'USA',
            'postalCode': projectData.zipcode,
            'state': projectData.state,
            'city': projectData.city,
            'reportingUnits': 'us'
            }
        return data_input
    
    def siteEUIFromZT(self, projectData):
        data = self.buildZeroToolDataInput(projectData)
        # print(data)
        zeroTooloutput = self.getZeroToolData(data)
        zeroTooloutputJSON = zeroTooloutput.json()
        medianSiteEUI = self.extractMedianSiteEUI(zeroTooloutputJSON)
        # print(medianSiteEUI)
        return medianSiteEUI

    def getZeroToolData(self, data):
        response = requests.post('https://tool.zerotool.org/baseline', 
                                    headers={'Accept': 'application/json', 'Content-Type': 'application/json'},
                                    data=json.dumps([data]))
            
        return response
    
    def extractMedianSiteEUI(self, zeroTooloutput):
        medianSiteEUI1 = 0
        for value in zeroTooloutput['values']:
            if 'medianSiteEUI' in value:
                medianSiteEUI1 = value['medianSiteEUI']
                break   
        return medianSiteEUI1

def printJSON(project):

    json_data = {"Buildings": []}

    for building in project.buildings:
        building_dict = {
            "UniqueID": building.uniqueID,
            "buildingName": building.name,
            "buildingEnergyConsumption": roundNumbers(building.buildingEnergyConsumption),
            "buildingElectricityConsumption": roundNumbers(building.buildingElectricityConsumption),
            "buildingNaturalGasConsumption": roundNumbers(building.buildingNaturalGasConsumption),
            "operationalCarbonProjection": building.operationalCarbonProjection[-1],
            "buildingEUI": roundNumbers(building.buildingEUI),
            "operationsCEI": roundNumbers(building.operationsCEI),
        }

        json_data["Buildings"].append(building_dict)

    # json_str = json.dumps(json_data, indent=4)
    # print(json_str)


    with open('data/sampleOutput.json', 'w') as json_file:
        json.dump(json_data, json_file, indent=4)

def roundNumbers(number):
    if number > 100000:
        return int(round(number, -5))
    elif number > 10000:
        return int(round(number, -4))
    elif number > 1000:
        return int(round(number, -3))
    else: return round(number, 1)
        
    
newProject = Project('data\\dummyProjectData.json')
printJSON(newProject)