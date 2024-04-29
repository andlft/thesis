using UnityEngine;

namespace Scripts.Misc
{
    public class Destructible : MonoBehaviour
    {
        public float destructionTime = 1f;
        [Range(0f, 1f)]
        public float itemSpawnChance = 0.1f;
        public GameObject[] spawnableItems;

        private void Start()
        {
            Destroy(gameObject, destructionTime);
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
