using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform obstacle;
    private RoundManager roundManager;
    private float timer = 0f;

    private void Update()
    {
        //spawn enemy every 10 seconds for testing
        timer += Time.deltaTime;
        if (timer >= 10f)
        {
            //Spawn();
            timer = 0f;
        }

    }
    public GameObject Spawn(float speed, float health)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefabs list is empty.");
            return null;
        }
        GameObject enemyInstance = Instantiate(enemyPrefab, transform.position, transform.rotation);
        Enemy enemyComponent = enemyInstance.GetComponent<Enemy>();
        if (enemyComponent != null && target != null)
        {
            if (obstacle == null)
            {
                enemyComponent.init(target);
            }
            enemyComponent.init(target, obstacle);
            enemyComponent.initStats(speed, health);
            return enemyInstance;
        }
        else
        {
            Debug.LogWarning("Enemy component or target is missing.");
            return null;
        }
    }


}
