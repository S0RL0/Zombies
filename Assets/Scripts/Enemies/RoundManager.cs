using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    // Round management variables
    public int roundNumber = 0;
    public int enemiesThisRound = 5;
    public int enemiesSpawned = 0;
    public int maxEnemiesAtOneTime = 48;
    public float spawnInterval = 5f;
    public float timeBetweenRounds = 5f;
    public float initialDelay = 5f;
    public int usableSpawnPointCount = 5;
    public float roundTimer = 0f;

    // References
    private GameObject player;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private List<GameObject> closestSpawnPoints;
    [SerializeField] private List<GameObject> enemies;

    // Enemy Stat management variables



    void Start()
    {
        player = FindFirstObjectByType<PlayerController>().gameObject;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint").ToList();
        closestSpawnPoints = new List<GameObject>();
        Invoke("StartRound", initialDelay);
    }

    void Update()
    {
        roundTimer += Time.deltaTime;
        // If there are no enemies left, start the next round
        if (enemies.Count == 0 && roundTimer > 30)
        {
            roundTimer = 0f;
            Invoke("StartRound", timeBetweenRounds);
        }

        foreach (GameObject enemy in enemies.ToList())
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }

    void StartRound()
    {
        // Reset round timer and icrement round number
        roundNumber++;
        enemiesSpawned = 0;
        spawnInterval = Mathf.Max(2f, 10f - 0.1f * roundNumber);
        CancelInvoke("SpawnEnemies");


        // If spawn points are not found, log a warning and return
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points found in the scene. Please add spawn points with the tag 'EnemySpawnPoint'.");
            return;
        }

        enemiesThisRound = Mathf.RoundToInt(0.08f * roundNumber * roundNumber + 1.5f * roundNumber + 5);

        //Envoke SpawnZombies() every 5 seconds until the number of enemies spawned is equal to the number of enemies for this round
        InvokeRepeating("SpawnEnemies", 0f, spawnInterval);

    }

    private void SpawnEnemies()
    {
        // Check if there are more than 5 spawn points available, if so use the 5 closest spawn points to the player
        if (spawnPoints.Count > usableSpawnPointCount)
        {
            // Sort spawn points by distance to player and take the closest 5
            closestSpawnPoints = spawnPoints.OrderBy(sp => Vector3.Distance(sp.transform.position, player.transform.position)).Take(usableSpawnPointCount).ToList();
        }
        else
        {
            closestSpawnPoints = spawnPoints;
        }

        // Spawn zombies at the closest spawn points until all spawn points have been used
        foreach (GameObject spawnPoint in closestSpawnPoints)
        {
            if (enemiesSpawned >= maxEnemiesAtOneTime)
            {
                return;
            }
            if (enemiesSpawned >= enemiesThisRound)
            {
                CancelInvoke("SpawnEnemies");
                return;
            }

            EnemySpawnPoint spawnScript = spawnPoint.GetComponent<EnemySpawnPoint>();
            if (spawnScript != null)
            {
                int s = (int)(2f + 0.5f * roundNumber);
                float speed = Mathf.Min(s, 12);
                int h = (int)(roundNumber * roundNumber + 2 * roundNumber + 40);
                float health = Mathf.Min(h, 1000);
                GameObject enemy = spawnScript.Spawn(speed, health);
                if (enemy != null)
                {
                    enemies.Add(enemy);
                    enemiesSpawned++;
                }
            }
        }
    }
}
