using UnityEngine;

public class SpawnLocation : MonoBehaviour
{

    public Transform Goal;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    { 
        Debug.Log("Zomb"); 
        Moveto zombie = other.GetComponent<Moveto>();
        if (other.CompareTag("Zombie"))
        {
           

            zombie.goal = Goal;
            zombie.Spawn(); 

            
        }
    }
}
