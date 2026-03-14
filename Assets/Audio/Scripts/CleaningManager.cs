using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance;

    public GameObject spongePrefab;
    public GameObject DustClean;
    public PlayerInteract playerInteract;


    void Awake()
    {
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
    }
}
