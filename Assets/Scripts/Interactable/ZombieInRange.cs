using UnityEngine;

public class ZombieInRange : MonoBehaviour
{
    private DoorFix doorscript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        doorscript = GetComponentInParent<DoorFix>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Nn");
        if (other.CompareTag("Zombie"))
        {
            Debug.Log("Dork");
            doorscript.ZombieinRange = true;
            doorscript.Fixable = false; 

        }

        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            Debug.Log("Dorker");
            doorscript.ZombieinRange = false;
            doorscript.Fixable = true; 
        }
    }
}
