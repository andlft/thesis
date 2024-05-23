using Assets.Scripts.Environment.Environment;
using UnityEngine;

namespace Scripts.Misc
{
    public class Destructible : MonoBehaviour
    {
        public float destructionTime = 1f;
        [Range(0f, 1f)]
        public float itemSpawnChance = 0f;
        public GameObject[] spawnableItems;

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

        private void OnDestroy()
        {
            if (spawnableItems.Length > 0 && Random.value < itemSpawnChance)
            {
                int randomIndex = Random.Range(0, spawnableItems.Length);
                Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
            }
        }

    }
}
