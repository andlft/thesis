using UnityEngine;
using Scripts.Misc;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Environment.Environment;

namespace Assets.Scripts.Enemy.EnemyScript
{
    public class EnemyS : MonoBehaviour
    {

        [SerializeField] private GameObject upRay;
        [SerializeField] private GameObject downRay;
        [SerializeField] private GameObject leftRay;
        [SerializeField] private GameObject rightRay;
        [SerializeField] private Env env;
        
        public float speed = 5f;
        public AnimatedSpriteRenderer spriteRendererUp;
        public AnimatedSpriteRenderer spriteRendererDown;
        public AnimatedSpriteRenderer spriteRendererLeft;
        public AnimatedSpriteRenderer spriteRendererRight;
        public AnimatedSpriteRenderer activeSpriteRenderer;

        private Vector2 direction = Vector2.down;
        public new Rigidbody2D rigidbody { get; private set; }
        private int stageLayer;
        private Simulation simulation;
        public bool isActive = true;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            activeSpriteRenderer = spriteRendererDown;
            SetDirection(Vector2.zero, activeSpriteRenderer);
            stageLayer = LayerMask.GetMask("Stage", "Bomb", "Goal");
            simulation = env.GetComponent<Simulation>();
        }

        void FixedUpdate()
        {
            Vector2 position = rigidbody.position;
            Vector2 traslation = direction * speed * simulation.SimulationStepSize;

            rigidbody.MovePosition(position + traslation);
            EnemyControl();
        }

        private void EnemyControl()
        {
            float[] distances = CastRays();
            // Enemy has walls on all sides
            if (distances.Sum() < 1.0f)
            {
                SetDirection(Vector2.zero, activeSpriteRenderer);
            }
            // Start enemy if one of the walls is broken
            else if (distances.Sum() > 1.0f && direction == Vector2.zero)
            {
                SetDirection(Vector2.down, spriteRendererDown);
            }

            // If the enemy hits a wall, change its moving direction
            List<int> availableDirectionIndices = new List<int>();
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] > 0.5f)
                {
                    availableDirectionIndices.Add(i);
                }
            }
            if (availableDirectionIndices.Count == 0)
            {
                SetDirection(Vector2.zero, activeSpriteRenderer);
            }
            else if (
                (direction == Vector2.down && distances[0] < 0.2f) ||
                (direction == Vector2.up && distances[1] < 0.2f) ||
                (direction == Vector2.left && distances[2] < 0.2f) ||
                (direction == Vector2.right && distances[3] < 0.2f)
            )
                {
                    int rndIndex = Random.Range(0, availableDirectionIndices.Count);
                    if (availableDirectionIndices[rndIndex] == 0)
                    {
                        SetDirection(Vector2.down, spriteRendererDown);
                    }
                    else if (availableDirectionIndices[rndIndex] == 1)
                    {
                        SetDirection(Vector2.up, spriteRendererUp);
                    }
                    else if (availableDirectionIndices[rndIndex] == 2)
                    {
                        SetDirection(Vector2.left, spriteRendererLeft);
                    }
                    else if (availableDirectionIndices[rndIndex] == 3)
                    {
                        SetDirection(Vector2.right, spriteRendererRight);
                    }
                }
        }


        public float[] CastRays()
        {
            RaycastHit2D hitDown = Physics2D.Raycast(
                downRay.transform.position,
                -Vector2.up,
                Mathf.Infinity,
                stageLayer
                );
            RaycastHit2D hitUp = Physics2D.Raycast(
                upRay.transform.position,
                Vector2.up,
                Mathf.Infinity,
                stageLayer
                );
            RaycastHit2D hitLeft = Physics2D.Raycast(
                leftRay.transform.position,
                -Vector2.right,
                Mathf.Infinity,
                stageLayer
                );
            RaycastHit2D hitRight = Physics2D.Raycast(
                rightRay.transform.position,
                Vector2.right,
                Mathf.Infinity,
                stageLayer
                );

            float[] result = new float[4];
            result[0] = hitDown.distance;
            result[1] = hitUp.distance;
            result[2] = hitLeft.distance;
            result[3] = hitRight.distance;

            return result;
        }

        public void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
        {
            direction = newDirection;

            spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
            spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
            spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
            spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

            activeSpriteRenderer = spriteRenderer;
            activeSpriteRenderer.idle = direction == Vector2.zero;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
            {
                if (isActive)
                {
                    gameObject.transform.position = new Vector3(20.0f, 20.0f, 20.0f);
                    speed = 0.0f;
                    env.activeEnemies--;
                    env.AddReward(5.0f);
                    isActive = false;
                }
            }
        }
    }
}
