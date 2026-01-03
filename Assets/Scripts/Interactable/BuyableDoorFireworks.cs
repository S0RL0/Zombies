using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BuyableDoorFireworks : MonoBehaviour
{

    [Header("What to move")]
    public GameObject Effect;
    public Transform emitter;              // drag your particle system object here

    [Header("Path")]
    public Transform[] pathPoints;         // drag waypoints here (in order)

    [Header("Movement")]
    public float speed = 5f;
    public bool loop = false;

    [Header("Objects To Destroy")]
    public GameObject Parent;

    [Header("FireWorks")]
    public GameObject[] Fireworks;

    [Header("Explosion")]
    public GameObject ExplosionParent; 

    [Header("Sound")]
    [SerializeField] private EventReference BuyEvent;
    private EventInstance BuyInstance;

    [SerializeField] private EventReference FuzeEvent;
    private EventInstance FuzeInstance;

    [SerializeField] private EventReference FireworksEvent;
    private EventInstance FireworksInstance;

    [SerializeField] private EventReference ExplosionEvent;
    private EventInstance ExplosionInstance;


    int currentIndex = 0;
    int fXVolume;

    void Start()
    {
        


        if (emitter == null)
        {
            Debug.LogError("Emitter is not assigned.");
            return;
        }

        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("Need at least 2 path points.");
            return;
        }

        //Setting Audio Para
        fXVolume = AudioSettingsManager.Instance.FXVolume;
        FuzeInstance = RuntimeManager.CreateInstance(FuzeEvent);
       // RuntimeManager.AttachInstanceToGameObject(FuzeInstance, Effect);
        emitter.position = pathPoints[0].position;
        
       

       
    }

    public void Buy() 
    {

        BuyInstance = RuntimeManager.CreateInstance(BuyEvent);
        BuyInstance.setVolume(fXVolume);
        BuyInstance.start();
        BuyInstance.release();

        StartCoroutine(FollowPath());


    }
        

    IEnumerator FollowPath()
    { 
        Effect.SetActive(true);
        FuzeInstance.start();
        while (true)
        {
           

            Transform target = pathPoints[currentIndex];

            while ((emitter.position - target.position).sqrMagnitude > 0.01f * 0.01f)
            {
                emitter.position = Vector3.MoveTowards(
                    emitter.position,
                    target.position,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            currentIndex++;

            if (currentIndex >= pathPoints.Length)
            {
                FuzeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                FuzeInstance.release();
                FireworkEvent();
                    yield break;
            }
        }
    }

    public void FireworkEvent() 
    {
        // Trigger Explosion
        ExplosionParent.SetActive(true);
        ExplosionInstance = RuntimeManager.CreateInstance(ExplosionEvent);
        ExplosionInstance.setVolume(fXVolume);
        ExplosionInstance.start();
        ExplosionInstance.release();
        //Remove Block
        Parent.SetActive(false);
        //Remove Fuze Effect
        Effect.SetActive(false);

        //Fireworks
        FireworksInstance = RuntimeManager.CreateInstance(FireworksEvent);
        FireworksInstance.setVolume(fXVolume);
        

        for (int i = 0; i < Fireworks.Length; i++)
        {
            if (Fireworks[i] != null)
            {
                FireworksInstance.start();
                FireworksInstance.release();
                Fireworks[i].SetActive(true);
            }
        }






    }
}
