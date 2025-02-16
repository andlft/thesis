using UnityEngine;
using Scripts.Player.MovementController;
using Assets.Scripts.Environment.AgentComms;
using Scripts.Bomb;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Assets.Scripts.Enemy.EnemyScript;

namespace Assets.Scripts.Environment.Environment
{
    public class Env : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public Transform goal;
        [SerializeField] private float minRewardReset;
        [SerializeField] private float reward;
        [SerializeField] private float totalReward = 0;
        [SerializeField] private bool finished;
        public bool manualControl;
        private Vector2 initalPlayerPos;
        private Vector2 lastPlayerPos;
        private Simulation simulation;
        public int currentStepsCL = 1;
        public int totalStepsCL = 300000;
        public Tilemap destructibles;
        private TileBase[,] initialDestructibles;
        public float timeMultiplier;
        private MovementController playerController;
        private BombController bombController;
        private List<Vector3> initialEnemyPos = new List<Vector3>();
        private GameObject[] enemies;
        private float initialSpeed;
        public int walls;
        public int initalWalls;
        public bool goalPresent = false;
        public int initialEnemies;
        public int activeEnemies;

        void Start()
        {
            initalPlayerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            playerController = player.GetComponent<MovementController>();
            bombController = player.GetComponent<BombController>();

            reward = 0f;
            finished = false;
            lastPlayerPos = new Vector2(1000f, 1000f);
            simulation = GetComponent<Simulation>();
            StoreOriginalDestructiblesConfig();

            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            initialSpeed = enemies[0].GetComponent<EnemyS>().speed;
            foreach (GameObject obj in enemies)
            {
                initialEnemyPos.Add(obj.transform.position);
            }
        }

        public PositionVector ResetEnvironment()
        {
            // Bring the player to its starting point and state
            Transform playerTransform = player.GetComponent<Transform>();
            playerTransform.position = initalPlayerPos;

            totalReward = 0f;
            SetReward(0f);
            finished = false;

            playerController.SetDirection(Vector2.zero, playerController.activeSpriteRenderer);

            //Reset trail
            TrailRenderer trailRenderer = player.GetComponent<TrailRenderer>();
            trailRenderer.Clear();


            goal.position = new Vector3(20.0f, 20.0f, 0.0f);


            // Move the goal at a random intersection on the map
            //int goal_x = Mathf.RoundToInt(UnityEngine.Random.Range(-3, 4));
            //int goal_y = Mathf.RoundToInt(UnityEngine.Random.Range(-3, 4));
            //goal.position = new Vector3(2 * goal_x, 2 * goal_y, 0f);


            // Destroy all remaining bombs
            GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
            foreach (GameObject obj in bombs)
            {
                Destroy(obj);
            }

            // Destroy all remaining explosions
            GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");
            foreach (GameObject obj in explosions)
            {
                Destroy(obj);
            }

            bombController.bombsRemaining = bombController.bombAmount;

            // Reset the tilemap
            ResetDestructibles();

            // Reset the enemy pos
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = initialEnemyPos[i];
                enemies[i].GetComponent<EnemyS>().speed = initialSpeed;
            }

            goalPresent = false;
            activeEnemies = initialEnemies;

            return new PositionVector(
                playerTransform.position,
                goal.position,
                new Vector2(10f, 10f),
                playerController.CastRays(),
                enemies[0].transform.position,
                enemies[1].transform.position
                );
        }

        public StepResult EnvStep(string action)
        {
            // Take action
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
                case "placeBomb":
                    bombController.PlaceBomb();
                    break;
                default:
                    playerController.SetDirection(Vector2.zero, playerController.activeSpriteRenderer);
                    break;
            }
            simulation.Simulate();

            GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
            if (goalPresent)
            {
                if (Vector2.Distance(lastPlayerPos, goal.position) > Vector2.Distance(player.transform.position, goal.position))
                {
                    AddReward(0.1f);
                }
                else
                {
                    AddReward(-0.1f);
                }
            }
            else if (bombs.Length >= 1)
            {
                if (Vector2.Distance(
                    new Vector2(bombs[0].transform.position.x, bombs[0].transform.position.y),
                    new Vector2(player.transform.position.x, player.transform.position.y)
                    ) < 2.0f)
                {
                    AddReward(-0.02f);
                }
            }
            else if (bombs.Length == 0)
            {
                AddReward(-0.05f);
            }
            AddReward(-0.003f);

            lastPlayerPos = new Vector2 (player.transform.position.x, player.transform.position.y);

            // Reset env if reward is lower that threshold
            if(totalReward < minRewardReset)
            {
                finished = true;
            }

            currentStepsCL++;

            float rewardCopy = reward;
            SetReward(0.0f);

            if (bombs.Length > 0)
            {
                return new StepResult(
                    rewardCopy,
                    finished,
                    new PositionVector(
                        player.transform.position,
                        goal.position,
                        bombs[0].transform.position,
                        playerController.CastRays(),
                        enemies[0].transform.position,
                        enemies[1].transform.position
                        )
                    );
            }
            else
            {
                return new StepResult(
                    rewardCopy,
                    finished,
                    new PositionVector(
                        player.transform.position,
                        goal.position,
                        new Vector2(10f, 10f),
                        playerController.CastRays(),
                        enemies[0].transform.position,
                        enemies[1].transform.position
                        )
                    );
            }
        }
        private void StoreOriginalDestructiblesConfig()
        {
            walls = 0;
            BoundsInt bounds = destructibles.cellBounds;
            initialDestructibles = new TileBase[bounds.size.x, bounds.size.y];

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                initialDestructibles[pos.x - bounds.xMin, pos.y - bounds.yMin] = destructibles.GetTile(pos);
                if (destructibles.GetTile(pos) != null)
                {
                    walls++;
                }
            }

            initalWalls = walls;
        }

        private void ResetDestructibles()
        {
            walls = 0;
            BoundsInt bounds = destructibles.cellBounds;
            float destSpawnChance = (float)currentStepsCL / (float)totalStepsCL;

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                float randVal = Random.value;
                if (randVal < destSpawnChance)
                {
                    destructibles.SetTile(pos, initialDestructibles[pos.x - bounds.xMin, pos.y - bounds.yMin]);
                }

                if (destructibles.GetTile(pos) != null)
                {
                    walls++;
                }
            }
            initalWalls = walls;
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

        public float GetSimulationStep()
        {
            return simulation.SimulationStepSize;
        }


    }
}