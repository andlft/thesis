from dataclasses import dataclass

@dataclass
class position_vector:
    playerX: float
    playerY: float
    goalX: float
    goalY: float
    bombX: float
    bombY: float
    rayDown: float
    rayUp: float
    rayLeft: float
    rayRight: float
    enemy1X: float
    enemy1Y: float
    enemy2X: float
    enemy2Y: float

@dataclass
class env_result:
    reward: float
    finished: bool
    obs: position_vector
