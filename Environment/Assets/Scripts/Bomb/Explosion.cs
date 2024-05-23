using UnityEngine;
using Scripts.Misc;
using Assets.Scripts.Environment.Environment;

namespace Scripts.Bomb
{
    public class Explosion : MonoBehaviour
    {
        public AnimatedSpriteRenderer start;
        public AnimatedSpriteRenderer middle;
        public AnimatedSpriteRenderer end;
        public Env env;

        private float fuse;
        [SerializeField] private float timer;

        private void FixedUpdate()
        {
            if (fuse > timer)
            {
                Destroy(gameObject);
            }

            fuse += env.timeMultiplier * env.GetSimulationStep();
        }
        public void SetActiveRenderer(AnimatedSpriteRenderer renderer)
        {
            start.enabled = renderer == start;
            middle.enabled = renderer == middle;
            end.enabled = renderer == end;
        }

        public void SetDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }

    }
}