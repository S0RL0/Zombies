using UnityEngine;

public class DoorBuy : MonoBehaviour
{

    private BuyableDoorFireworks Script;
    public GameObject Self; 
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
        if (other.CompareTag("Player"))
        {
           Script = GetComponentInParent<BuyableDoorFireworks>();
           Script.Buy();
            Self.SetActive(false); 
        }
    }
}
