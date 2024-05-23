import torch.nn as nn
class PPO():
    @staticmethod
    def get_args(new_args = None):
        if new_args is None:
            new_args = {}
        args ={
            "policy": "MlpPolicy",
            "learning_rate": 0.0003,
            "n_steps": 4096,
            "batch_size": 64,
            "n_epochs": 10,
            "gamma": 0.99,
            "gae_lambda": 0.95,
            "clip_range":0.2,
            "clip_range_vf":None,
            "normalize_advantage": True,
            "ent_coef": 0.01,
            "vf_coef": 0.5,
            "max_grad_norm": 0.5,
            "use_sde": False,
            "sde_sample_freq": -1,
            "rollout_buffer_class": None,
            "rollout_buffer_kwargs": None,
            "target_kl": None,
            "stats_window_size": 100,
            "policy_kwargs": None,
            "seed": None, 
            "_init_setup_model": True
        }
        args.update(new_args)
        return args
    
    @staticmethod
    def get_run_args():
        args ={
            "save_interval": 163840,
            "total_steps": 2097152
        }
        return args