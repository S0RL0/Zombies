using UnityEngine;
using UnityEngine.AI;

public class Moveto : MonoBehaviour
{ 
    
    public Transform goal;
    public bool follow;
    private NavMeshAgent agent;
    private GameObject player;
    private bool reachedGoal = false;
    public Transform TEMPTESTGOAL;
    public float distanceToGoal = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;


        player = GameObject.FindGameObjectWithTag("Player");
        Spawn(TEMPTESTGOAL);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,goal.position)< distanceToGoal)
        {
            reachedGoal = true;
        }
        if (reachedGoal)
            GetComponent<NavMeshAgent>().destination = player.transform.position;
        else
            GetComponent<NavMeshAgent>().destination = goal.position;
           
       
    }

    public void Spawn(Transform goalToSet)
    {
        goal = goalToSet;
    }
}
