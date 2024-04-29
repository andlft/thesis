import os

from datetime import datetime
from typing import Any, Dict

def create_dirs(agent_type: str) -> Dict[str, str]:
    timestamp = datetime.now().strftime('%Y%m%d%H%M%S')

    agents_dir = f"agents\\saves\\{agent_type}-{timestamp}"
    logs_dir = f"logs\\{agent_type}-{timestamp}"
    saves_configs_dir = "agents\\saves\\saved_configs"

    os.makedirs(agents_dir, exist_ok=True)
    os.makedirs(logs_dir, exist_ok=True)
    os.makedirs(saves_configs_dir, exist_ok=True)

    return {
        "agents_dir": agents_dir,
        "logs_dir": logs_dir
    }

def get_agent_config(agent_type: str, saves_path: str, **kwargs: Dict[str, Any]) -> Dict[str, Any]:
    module_name = f"agents.configs.{agent_type}"
    module = __import__(module_name, fromlist=[''])
    config = getattr(module, f"{agent_type}").get_args(kwargs)

    with open(f"agents\\saves\\saved_configs\\{saves_path.split('-')[-1]}.config", "w") as file:
        for key, value in config.items():
            if isinstance(value, str):
                file.write(f"\"{key}\": \"{value}\"\n")
            else:
                file.write(f"\"{key}\": {value}\n")
    return config

def get_run_config(agent_type: str) -> Dict[str, Any]:
    module_name = f"agents.configs.{agent_type}"
    module = __import__(module_name, fromlist=[''])
    return getattr(module, f"{agent_type}").get_run_args()