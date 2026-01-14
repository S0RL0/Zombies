using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform obstacle;
    private float timer = 0f;

    private void Update()
    {
        //spawn enemy every 10 seconds for testing
        timer += Time.deltaTime;
        if (timer >= 10f)
        {
            Spawn();
            timer = 0f;
        }


    }
    public void Spawn()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefabs list is empty.");
            return;
        }
        GameObject enemyInstance = Instantiate(enemyPrefab, transform.position, transform.rotation);
        Enemy enemyComponent = enemyInstance.GetComponent<Enemy>();
        if (enemyComponent != null && target != null)
        {
            if (obstacle == null)
            {
                enemyComponent.init(target);
                return;
            }
            enemyComponent.init(target, obstacle);
        }
        else
        {
            Debug.LogWarning("Enemy component or target is missing.");
        }
    }
}
