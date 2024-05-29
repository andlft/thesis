using UnityEngine;
using Scripts.Misc;
using Assets.Scripts.Environment.Environment;

namespace Scripts.Player.MovementController
{
    public class MovementController : MonoBehaviour
    {
        public new Rigidbody2D rigidbody { get; private set; }
        private Vector2 direction = Vector2.down;
        public float speed = 5f;

        public KeyCode inputUp = KeyCode.W;
        public KeyCode inputDown = KeyCode.S;
        public KeyCode inputLeft = KeyCode.A;
        public KeyCode inputRight = KeyCode.D;


        public AnimatedSpriteRenderer spriteRendererUp;
        public AnimatedSpriteRenderer spriteRendererDown;
        public AnimatedSpriteRenderer spriteRendererLeft;
        public AnimatedSpriteRenderer spriteRendererRight;
        public AnimatedSpriteRenderer activeSpriteRenderer;
        [SerializeField] private GameObject upRay;
        [SerializeField] private GameObject downRay;
        [SerializeField] private GameObject leftRay;
        [SerializeField] private GameObject rightRay;

        public Simulation simulation;
        public Env env;

        private int stageLayer;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            activeSpriteRenderer = spriteRendererDown;
            SetDirection(Vector2.zero, activeSpriteRenderer);
            stageLayer = LayerMask.GetMask("Stage", "Bomb", "Enemy");
        }

        private void Update()
        {
            if (env.manualControl)
            {
                ManualControl();
            }
        }

        private void ManualControl()
        {
            if (Input.GetKey(inputUp))
            {
                SetDirection(Vector2.up, spriteRendererUp);
            }
            else if (Input.GetKey(inputDown))
            {
                SetDirection(Vector2.down, spriteRendererDown);
            }
            else if (Input.GetKey(inputLeft))
            {
                SetDirection(Vector2.left, spriteRendererLeft);
            }
            else if (Input.GetKey(inputRight))
            {
                SetDirection(Vector2.right, spriteRendererRight);
            }
            else
            {
                SetDirection(Vector2.zero, activeSpriteRenderer);
            }
        }

        private void FixedUpdate()
        {
            Vector2 position = rigidbody.position;
            Vector2 traslation = direction * speed * simulation.SimulationStepSize;

            rigidbody.MovePosition(position + traslation);
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

            //Debug.DrawRay(downRay.transform.position, -Vector2.up * hitDown.distance, Color.red);
            //Debug.DrawRay(upRay.transform.position, Vector2.up * hitUp.distance, Color.red);
            //Debug.DrawRay(leftRay.transform.position, -Vector2.right * hitLeft.distance, Color.red);
            //Debug.DrawRay(rightRay.transform.position, Vector2.right * hitRight.distance, Color.red);

            float[] result = new float[4];
            result[0] = hitDown.distance;
            result[1] = hitUp.distance;
            result[2] = hitLeft.distance;
            result[3] = hitRight.distance;

            return result;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
            {
                env.AddReward(-1);
                env.SetFinished(true);
            }
            if (other.CompareTag("Goal"))
            {
                env.AddReward(10);
                env.SetFinished(true);
            }
            if (other.CompareTag("Enemy"))
            {
                env.AddReward(-1);
                env.SetFinished(true);
            }
        }
    }
}