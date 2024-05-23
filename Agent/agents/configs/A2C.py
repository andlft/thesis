import torch.nn as nn

class A2C():
    @staticmethod
    def get_args(new_args = None):
        if new_args is None:
            new_args = {}
        args ={
            "policy": "MlpPolicy",
            "learning_rate": 0.0001,
            "n_steps": 60,
            "gamma": 0.99,
            "gae_lambda": 1.0,
            "ent_coef": 0.001,
            "vf_coef": 0.5,
            "max_grad_norm": 0.5,
            "rms_prop_eps": 1e-05,
            "use_rms_prop": True,
            "use_sde": False,
            "sde_sample_freq": -1,
            "rollout_buffer_class": None,
            "rollout_buffer_kwargs": None,
            "normalize_advantage": False,
            "stats_window_size": 100,
            "policy_kwargs":  dict(
                activation_fn=nn.ReLU,
                net_arch=dict(
                    pi=[256, 256],
                    vf=[256, 256]
                    )
                ),
            "seed": None, 
            "_init_setup_model": True
        }
        args.update(new_args)
        return args
    
    @staticmethod
    def get_run_args():
        args ={
            "save_interval": 60000,
            "total_steps": 2000000
        }
        return args
