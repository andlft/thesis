from dataclasses import dataclass

@dataclass
class position_vector:
    goalX: float
    goalY: float
    playerX: float
    playerY: float

@dataclass
class env_result:
    reward: float
    finished: bool
    obs: position_vector
