import json

class Program:
    def __init__(self, data):
        self.programName = data['Program Name']
        self.area = data['Area']

class Building:
    def __init__(self, data):
        self.name = data['Name']
        self.uniqueID = data['UniqueID']
        self.programs = []
        self.add_programs(data['Programs'])

    def add_programs(self, programs_data):
        for program_data in programs_data:
            program = Program(program_data)
            self.programs.append(program)

class Project:
    def __init__(self, json_path):
        with open(json_path, 'r') as f:
            data = json.load(f)

        self.zipcode = data['zipcode']
        self.state = data['state']
        self.city = data['city']
        self.operationalYears = 50
        self.unit = "ip"
        self.projectRegion = self.assignProjectZone()

        self.buildings = []
        self.add_buildings(data['Buildings'])

    def add_buildings(self, buildings_data):
        for building_data in buildings_data:
            building = Building(building_data)
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


newProject = Project('data\\dummyProjectData.json')
print(newProject.zipcode)
print(newProject.projectRegion)
print(newProject.buildings[0].name)
print(newProject.buildings[0].programs[0].programName)