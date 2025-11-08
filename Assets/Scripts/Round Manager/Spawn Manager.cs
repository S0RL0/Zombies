using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public GameObject[] Zombies;
    public Transform[] Spawnpoints;
    public int Rounds;
    public int Timer;
    public float zombiespawns;
    public float RoundIncrease; 

    [Header("Zombies Left")]
    public int ZombiesLeft;
    public float checktimer = 2f;

    [Header("Round End")]

    public bool AllZombiesSpawned;
    public float RoundEndTimer; 
    
    




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Rounds = 1;
        Timer = 30;
        StartCoroutine(Spawn());
        StartCoroutine(RemaningZombies());
    }

    // Update is called once per frame
    IEnumerator Spawn()
    {
        for (int i = 0; i < zombiespawns; i++)
        {

            GameObject zombie = (GameObject)Instantiate(Zombies[Random.Range(0,Zombies.Length)], Spawnpoints[Random.Range(0,Spawnpoints.Length)].position, transform.rotation);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            if (i == zombiespawns-1)
            {
                AllZombiesSpawned = true;
            }
        }
    }

    IEnumerator RemaningZombies()
    {
        while (true)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Zombie");
            ZombiesLeft = taggedObjects.Length;

            

            yield return new WaitForSeconds(checktimer);

            if (AllZombiesSpawned && ZombiesLeft ==0)
            {
                StartCoroutine(RoundEnded());
                AllZombiesSpawned = false;
            }


        }
    }

    IEnumerator RoundEnded()
    {

        yield return new WaitForSeconds(RoundEndTimer);
        Debug.Log("Finished");
        Rounds++;
        zombiespawns += RoundIncrease;
        StartCoroutine(Spawn());

    }



}
