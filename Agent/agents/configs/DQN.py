class DQN():
    @staticmethod
    def get_args(new_args = None):
        if new_args is None:
            new_args = {}
        args ={
            "policy": "MlpPolicy",
            "learning_rate": 0.0001,
            "buffer_size": 1000000,
            "learning_starts": 100,
            "batch_size": 64,
            "tau": 1.0,
            "gamma": 0.95,
            "train_freq": 4,
            "gradient_steps": 1,
            "replay_buffer_class": None,
            "replay_buffer_kwargs": None,
            "optimize_memory_usage": False,
            "target_update_interval": 10000,
            "exploration_fraction": 1.0,
            "exploration_initial_eps": 1,
            "exploration_final_eps": 1,
            "max_grad_norm": 10,
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
            "save_interval": 300000,
            "total_steps": 300000
        }
        return args
