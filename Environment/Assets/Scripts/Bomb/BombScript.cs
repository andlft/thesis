using UnityEngine;
using Assets.Scripts.Environment.Environment;
using Scripts.Bomb;
using Scripts.Misc;
using UnityEngine.Tilemaps;

public class BombScript : MonoBehaviour
{
    public float timer;
    private float fuse;
    public Env env;
    public BombController bombController;
    private Vector2 position;
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    private void Start()
    {
        fuse = 0;
    }

    private void FixedUpdate()
    {
        if (fuse > timer)
        {
            Destroy(gameObject);

            position = transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);

            Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            explosion.SetActiveRenderer(explosion.start);
            Explosion explosionScript = explosion.GetComponent<Explosion>();
            explosionScript.env = env;

            bombController.Explode(position, Vector2.up, explosionRadius);
            bombController.Explode(position, Vector2.down, explosionRadius);
            bombController.Explode(position, Vector2.left, explosionRadius);
            bombController.Explode(position, Vector2.right, explosionRadius);

            bombController.ReturnBomb();

        }
        fuse += env.timeMultiplier * env.GetSimulationStep();
    }

   
}
