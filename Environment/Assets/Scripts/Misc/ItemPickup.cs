using UnityEngine;
using Scripts.Bomb;

namespace Scripts.Misc
{
    public class ItemPickup : MonoBehaviour
    {
        public enum ItemType
        {
            ExtraBomb,
            BlastRadius,
        }

        public ItemType Type;

        private void OnItemPickup(GameObject player)
        {
            switch (Type)
            {
                case ItemType.ExtraBomb:
                    player.GetComponent<BombController>().AddBomb();
                    break;
                case ItemType.BlastRadius:
                    player.GetComponent<BombController>().explosionRadius++;
                    break;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnItemPickup(other.gameObject);
            }
        }
    }
}