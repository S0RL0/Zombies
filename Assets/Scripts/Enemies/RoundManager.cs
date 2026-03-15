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
    public float timeBetweenRounds = 15f;
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
    }

    void StartRound()
    {
        // Reset round timer and icrement round number
        roundNumber++;

        // If spawn points are not found, log a warning and return
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points found in the scene. Please add spawn points with the tag 'EnemySpawnPoint'.");
            return;
        }

        enemiesThisRound = Mathf.RoundToInt(0.08f * roundNumber * roundNumber + 1.5f * roundNumber + 5);

        //Envoke SpawnZombies() every 5 seconds until the number of enemies spawned is equal to the number of enemies for this round
        Debug.Log("Round " + roundNumber + " starting with " + enemiesThisRound + " enemies.");
        InvokeRepeating("SpawnEnemies", 0f, spawnInterval);

    }

    private void SpawnEnemies()
    {
        Debug.Log("Spawning zombies... Enemies spawned: " + enemiesSpawned + "/" + enemiesThisRound);
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
        Debug.Log("Using " + closestSpawnPoints.Count + " spawn points for this round.");

        // Spawn zombies at the closest spawn points until all spawn points have been used
        foreach (GameObject spawnPoint in closestSpawnPoints)
        {
            if (enemiesSpawned >= maxEnemiesAtOneTime)
            {
                Debug.Log("Maximum enemies at one time reached. Stopping spawn.");
                return;
            }
            if (enemiesSpawned >= enemiesThisRound)
            {
                Debug.Log("All enemies for this round spawned. Stopping spawn.");
                CancelInvoke("SpawnZombies");
                return;
            }

            EnemySpawnPoint spawnScript = spawnPoint.GetComponent<EnemySpawnPoint>();
            if (spawnScript != null)
            {
                GameObject enemy = spawnScript.Spawn();
                if (enemy != null)
                {
                    Debug.Log("Spawned enemy at " + spawnPoint.name);
                    enemies.Add(enemy);
                    enemiesSpawned++;
                }
            }
        }
    }
}
