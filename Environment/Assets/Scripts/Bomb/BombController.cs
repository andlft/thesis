using UnityEngine;
using UnityEngine.Tilemaps;
using Scripts.Misc;
using Assets.Scripts.Environment.Environment;

namespace Scripts.Bomb
{
    public class BombController : MonoBehaviour
    {
        [Header("Bomb")]
        public GameObject bombPrefab;
        public KeyCode inputKey = KeyCode.Space;
        public float bombFuseTime = 3.0f;
        public int bombAmount = 1;
        public int bombsRemaining;

        [Header("Explosion")]
        public Explosion explosionPrefab;
        public LayerMask explosionLayerMask;
        public float explosionDuration = 1f;
        public int explosionRadius = 1;

        [Header("Destructible")]
        public Tilemap destructibleTiles;
        public Destructible destructiblePrefab;

        [SerializeField] Simulation simulation;
        [SerializeField] Env env;

        private void OnEnable()
        {
            bombsRemaining = bombAmount;
        }

        private void Update()
        {
            if (Input.GetKeyDown(inputKey))
            {
                PlaceBomb();
            }
        }

        public void PlaceBomb()
        {
            if (bombsRemaining <= 0)
            {
                return;
            }
            Vector2 position = transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);

            GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
            BombScript bombScript = bomb.GetComponent<BombScript>();
            bombScript.env = env;
            bombScript.bombController = this;
            bombScript.explosionPrefab = explosionPrefab;
            bombScript.explosionLayerMask = explosionLayerMask;
            bombScript.destructiblePrefab = destructiblePrefab;
            bombScript.explosionRadius = explosionRadius;
            bombScript.explosionDuration = explosionDuration;
            bombsRemaining--;

        }

        public void Explode(Vector2 position, Vector2 direction, int length)
        {
            if (length == 0)
            {
                return;
            }

            position += direction;

            if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask))
            {
                ClearDestructible(position);
                return;
            }

            Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
            explosion.SetDirection(direction);
            Explosion explosionScript = explosion.GetComponent<Explosion>();
            explosionScript.env = env;
            Explode(position, direction, length - 1);

            if (env.walls == 0)
            {
                AttemptSpawnGoal(new Vector2(0.0f, 0.0f));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
            {
                other.isTrigger = false;
            }
        }

        private void ClearDestructible(Vector2 position)
        {
            Vector3Int cell = destructibleTiles.WorldToCell(position);
            TileBase tile = destructibleTiles.GetTile(cell);

            if (tile != null)
            {
                Destructible destructible = Instantiate(destructiblePrefab, position, Quaternion.identity);
                Destructible destructibleScript = destructible.GetComponent<Destructible>();
                destructibleScript.env = env;
                destructibleTiles.SetTile(cell, null);
                env.AddReward(0.5f);
                if (!env.goalPresent)
                {
                    AttemptSpawnGoal(position);
                }
                env.walls--;
            }
        }

        private void AttemptSpawnGoal(Vector2 position)
        {
            float goalSpawnChance = (float)(env.initalWalls - env.walls) / (float)env.initalWalls;
            float randVal = Random.value;
            Debug.Log("" + randVal + "<" + goalSpawnChance + "?");
            float destSpawnChance = (float)env.currentStepsCL / (float)env.totalStepsCL;
            if (randVal < goalSpawnChance * goalSpawnChance) // + 0.05 * (1 - Mathf.Clamp(destSpawnChance, 0.0f, 1.0f)))
            {
                env.goal.transform.position = new Vector3(position.x, position.y, 0);
                env.goalPresent = true;
                env.AddReward(1.0f);
            }
        }

        public void AddBomb()
        {
            bombAmount++;
            bombsRemaining++;
        }

        public void ReturnBomb()
        {
            bombsRemaining++;
        }

    }
}