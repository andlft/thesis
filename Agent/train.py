import argparse
import stable_baselines3

from helpers.algorithms import RLAlgorithms
from helpers.callback import Callback
from helpers.read_write import create_dirs, get_agent_config, get_run_config
from helpers.environment import BombermanEnv
from peaceful_pie.unity_comms import UnityComms
from stable_baselines3.common.monitor import Monitor


def run(args: argparse.Namespace) -> None:
    dirs = create_dirs(args.type)
    unity_comms = UnityComms(port=args.port)
    env = BombermanEnv(unity_comms=unity_comms)
    env = Monitor(env)

    agent = getattr(stable_baselines3, args.type)(
        **get_agent_config(
            agent_type=args.type, 
            saves_path=dirs["agents_dir"]
            ),
        env=env,
        verbose=1,
        tensorboard_log=f"{dirs['logs_dir']}",
        device=args.device,
        )
    
    run_conf = get_run_config(args.type)
    total_steps = run_conf["total_steps"]
    save_interval = run_conf["save_interval"]
    done_steps = 0
    while(done_steps < total_steps):
        interval_steps = min(save_interval, total_steps-done_steps)
        agent.learn(
            total_timesteps=interval_steps,
            callback=Callback(port=args.port),
            reset_num_timesteps=True if done_steps == 0 else False,
            tb_log_name=args.type,
        )
        done_steps += interval_steps
        agent.save(f"{dirs['agents_dir']}/{done_steps}")


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--port", type=int, default=9000)
    parser.add_argument("--type", type=str, choices=RLAlgorithms(), required=True)
    parser.add_argument("--device", type=str, default="cpu")
    args = parser.parse_args()

    run(args)