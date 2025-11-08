using UnityEngine;

public class PlayerInRange : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            Debug.Log("ween");
            doorscript.PlayerinRange = true; 
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("weenor");
            doorscript.PlayerinRange = false;
        }
    }

}