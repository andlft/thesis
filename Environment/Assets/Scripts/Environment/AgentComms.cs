using AustinHarris.JsonRpc;
using UnityEngine;
using Scripts.Player;
using Assets.Scripts.Environment.Environment;

namespace Assets.Scripts.Environment.AgentComms
{ 
    public class AgentComms : MonoBehaviour
    {
        Rpc rpc;
        Simulation simulation;
        Env environment;
        public int fixedUpdateCounter;
        public int fixedUpdateWaitCycles;
        float initialStepSize;

        private void Start()
        {
            simulation = GetComponent<Simulation>();
            environment = GetComponent<Env>();
            fixedUpdateCounter = 0;
            initialStepSize = simulation.SimulationStepSize;
            rpc = new Rpc(this);
        }


        class Rpc : JsonRpcService
        {
            AgentComms comms;

            public Rpc(AgentComms comms)
            {
                this.comms = comms;
                comms.initialStepSize = comms.simulation.SimulationStepSize;
            }

            [JsonRpcMethod]
            StepResult Step(string action)
            {
                return comms.environment.EnvStep(action);
            }

            [JsonRpcMethod]
            PositionVector Reset()
            {
                return comms.environment.ResetEnvironment();
            }

            [JsonRpcMethod]
            bool GetFixedUpdateCounter()
            {
                return comms.fixedUpdateCounter >= comms.fixedUpdateWaitCycles;
            }

            [JsonRpcMethod]
            void ResetFixedUpdateCounter()
            {
                comms.fixedUpdateCounter = 0;
                comms.simulation.SimulationStepSize = comms.initialStepSize;
            }
        }

    ;


        private void FixedUpdate()
        {
            if (!environment.manualControl)
            {
                fixedUpdateCounter++;
                if (fixedUpdateCounter > fixedUpdateWaitCycles + 3)
                {
                    simulation.SimulationStepSize = 0.000001f;
                }
            }
            else
            {
                fixedUpdateCounter = 0;
            }
        }

    }

    public class PositionVector
    {
        public float playerX, playerY, goalX, goalY, bombX, bombY, rayDown, rayUp, rayLeft, rayRight;
        public float enemy1X, enemy1Y, enemy2X, enemy2Y;

        public PositionVector(
            Vector2 playerPos,
            Vector2 goalPos,
            Vector2 bombPos,
            float[] rays,
            Vector2 enemy1,
            Vector2 enemy2
            )
        {
            this.playerX = playerPos.x;
            this.playerY = playerPos.y;
            this.goalX = goalPos.x;
            this.goalY = goalPos.y;
            this.bombX = bombPos.x;
            this.bombY = bombPos.y;
            this.rayDown = rays[0];
            this.rayUp = rays[1];
            this.rayLeft = rays[2];
            this.rayRight = rays[3];
            this.enemy1X = enemy1.x;
            this.enemy1Y = enemy1.y;
            this.enemy2X = enemy2.x;
            this.enemy2Y = enemy2.y;
        }

    }

    public class StepResult
    {
        public float reward;
        public bool finished;
        public PositionVector obs;

        public StepResult(float reward, bool finished, PositionVector obs)
        {
            this.reward = reward;
            this.finished = finished;
            this.obs = obs;
        }
    }

}