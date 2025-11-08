using UnityEngine;
using UnityEngine.AI;

public class Moveto : MonoBehaviour
{ 
    
    public Transform goal;
    public bool follow;
    private NavMeshAgent agent;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().destination = player.transform.position;

    }

    public void Spawn()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;

    }
}
