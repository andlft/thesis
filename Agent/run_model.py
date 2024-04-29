import argparse
import torch
import stable_baselines3

from helpers.algorithms import RLAlgorithms
from helpers.environment import BombermanEnv
from peaceful_pie.unity_comms import UnityComms

def run(args: argparse.Namespace) -> None:

    unity_comms = UnityComms(port=args.port)
    env = BombermanEnv(unity_comms=unity_comms)
    agent = getattr(stable_baselines3, args.type).load(args.path, env=env)
    
    for _ in range(args.episodes):
        obs = torch.tensor(env.reset()[0], dtype=torch.float32)
        done = False
        while not done:
            if unity_comms.GetFixedUpdateCounter():
                action, _ = agent.predict(obs)
                obs, _ , done, _ , _ = env.step(action)
                unity_comms.ResetFixedUpdateCounter()
    
    env.reset()

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--port", type=int, default=9000)
    parser.add_argument("--type", type=str, choices=RLAlgorithms(), required=True)
    parser.add_argument("--path", type=str, required=True)
    parser.add_argument("--episodes", type=int, default=10)
    args = parser.parse_args()
    run(args)