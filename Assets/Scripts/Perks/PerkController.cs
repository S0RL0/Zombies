using UnityEngine;
using StarterAssets;

public class PerkController : MonoBehaviour
{
    public Transform Needle;
    public bool HealthPerk;
    public bool SpeedPerk; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          Needle.Rotate(new Vector3(1,0,0) * Time.deltaTime * 100);
    }

    void OnTriggerEnter(Collider other)
    {
        FirstPersonController first = other.GetComponent<FirstPersonController>();
        if (other.CompareTag("Player"))
        {
            if (HealthPerk) {first.HealthPerk();}
            if (SpeedPerk) { first.SpeedPerk(); }
            
            
            
            
            Destroy(gameObject); 
        }
    }
}
