class Site_features:
    def __init__(self, gfa, lpd):
        self.gfa = gfa
        self.lpd = lpd
        self.total_energy = self.gfa * self.lpd