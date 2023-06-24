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

    def __str__(self):
        return f"{self.name} {self.path}"

    def __repr__(self):
        return f"{self.name} {self.path}"