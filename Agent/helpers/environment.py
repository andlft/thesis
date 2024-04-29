import numpy as np

from helpers.data_classes import env_result, position_vector
from gymnasium import Env, spaces
from numpy.typing import NDArray
from peaceful_pie.unity_comms import UnityComms
from typing import Any, Tuple


class BombermanEnv(Env):
    def __init__(self, unity_comms: UnityComms):
        self.unity_comms = unity_comms
        self.action_space = spaces.Discrete(5)
        self.observation_space = spaces.Box(low= -np.inf, high= np.inf, shape=(4,), dtype=np.float32)

    def step(self, action: NDArray[np.uint8]) -> Tuple[NDArray[np.float32], float, bool, dict[str, Any]]:
        action_str = [
            "up",
            "down",
            "left",
            "right",
            "stop",
        ][action]
        result: env_result = self.unity_comms.Step(action=action_str, ResultClass=env_result)
        info = {"finished": result.finished}
        return self._obs_vec2_to_np(result.obs), result.reward, result.finished, False, info

    def reset(self, seed = None) -> NDArray[np.float32]:
        obs_vec2: position_vector = self.unity_comms.Reset(ResultClass=position_vector)
        info = {"finished": True}
        return self._obs_vec2_to_np(obs_vec2), info

    def _obs_vec2_to_np(self, vec2: position_vector) -> NDArray[np.float32]:
        return np.array([vec2.playerX, vec2.playerY, vec2.goalX, vec2.goalY], dtype=np.float32)
