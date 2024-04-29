using UnityEngine;
using Scripts.Player.MovementController;
using Assets.Scripts.Environment.AgentComms;

namespace Assets.Scripts.Environment.Environment
{
    public class Env : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Transform goal;
        [SerializeField] private float minRewardReset;
        [SerializeField] private float reward;
        [SerializeField] private float totalReward = 0;
        [SerializeField] private bool finished;
        public bool manualControl;
        private Vector2 initalPlayerPos;
        private Vector2 lastPlayerPos;
        private string lastPlayerAction;
        private Simulation simulation;

        void Start()
        {
            initalPlayerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            reward = 0f;
            finished = false;
            lastPlayerPos = new Vector2(1000f, 1000f);
            lastPlayerAction = "down";
            simulation = GetComponent<Simulation>();
        }

        public PositionVector ResetEnvironment()
        {
            // Bring the player to its starting point and state
            Transform playerTransform = player.GetComponent<Transform>();
            playerTransform.position = initalPlayerPos;

            totalReward = 0f;
            SetReward(0f);
            finished = false;
            lastPlayerPos = new Vector2(1000f, 1000f);
            lastPlayerAction = "down";

            MovementController playerController = player.GetComponent<MovementController>();
            playerController.SetDirection(Vector2.zero, playerController.activeSpriteRenderer);

            TrailRenderer trailRenderer = player.GetComponent<TrailRenderer>();
            trailRenderer.Clear();


            // Move the goal at a random intersection on the map
            int goal_x = Mathf.RoundToInt(UnityEngine.Random.Range(-3, 4));
            int goal_y = Mathf.RoundToInt(UnityEngine.Random.Range(-3, 4));
            goal.position = new Vector3(2 * goal_x, 2 * goal_y, 0f);

            return new PositionVector(playerTransform.position, goal.position);
        }

        public StepResult EnvStep(string action)
        {
            // Take action
            MovementController playerController = player.GetComponent<MovementController>();
            switch (action)
            {
                case "up":
                    playerController.SetDirection(Vector2.up, playerController.spriteRendererUp);
                    break;
                case "down":
                    playerController.SetDirection(Vector2.down, playerController.spriteRendererDown);
                    break;
                case "left":
                    playerController.SetDirection(Vector2.left, playerController.spriteRendererLeft);
                    break;
                case "right":
                    playerController.SetDirection(Vector2.right, playerController.spriteRendererRight);
                    break;
                default:
                    playerController.SetDirection(Vector2.zero, playerController.activeSpriteRenderer);
                    break;
            }
            simulation.Simulate();

            // Add/Subtract reward
            if (lastPlayerAction != action)
            {
                lastPlayerAction = new string(action);
                AddReward(-0.1f);
            }

            if (Vector2.Distance(lastPlayerPos, goal.position) > Vector2.Distance(player.transform.position, goal.position))
            {
                AddReward(0.1f);
            }
            else
            {
                AddReward(-0.2f);
            }

            lastPlayerPos = new Vector2 (player.transform.position.x, player.transform.position.y);

            // Reset env if reward is lower that threshold
            if(totalReward < minRewardReset)
            {
                finished = true;
            }

            float rewardCopy = reward;
            SetReward(0.0f);

            return new StepResult(rewardCopy, finished, new PositionVector(player.transform.position, goal.position));
        }

        public void AddReward(float value)
        {
            reward += value;
            totalReward += value;
        }

        public void SetReward(float value)
        {
            reward = value;
        }

        public void SetFinished(bool value)
        {
            finished = value;
        }

    }
}