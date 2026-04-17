using FMODUnity;
using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance;

    public GameObject spongePrefab;
    public GameObject DustClean;
    public PlayerInteract playerInteract;

    [Header("SoundEffects")]
    [SerializeField] public EventReference SweepSound;
    [SerializeField] public EventReference SpongeSound;
    public float Music;
    public float cleaning; 



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
