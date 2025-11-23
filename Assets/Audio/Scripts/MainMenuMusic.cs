using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private EventReference FireplaceEvent;
    private EventInstance FireplaceInstance;

    [SerializeField] private EventReference RainEvent;
    private EventInstance RainInstance;

    [SerializeField] private EventReference TypingEvent;
    private EventInstance TypingInstance;

    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;


    [Header("Volume")]
    public float Fire;
    public float Rain;
    public float Typing;
    public float Music;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FireplaceInstance = RuntimeManager.CreateInstance(FireplaceEvent);
        // FireplaceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        FireplaceInstance.setVolume(Fire); 
        FireplaceInstance.start();
        FireplaceInstance.release();

        RainInstance = RuntimeManager.CreateInstance(RainEvent);
        // FireplaceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RainInstance.setVolume(Rain);
        RainInstance.start();
        RainInstance.release();

        TypingInstance = RuntimeManager.CreateInstance(TypingEvent);
        // FireplaceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        TypingInstance.setVolume(Typing);
        TypingInstance.start();
        TypingInstance.release();

        MusicInstance = RuntimeManager.CreateInstance(MusicEvent);
        MusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        MusicInstance.setVolume(Music);
        MusicInstance.start();
        MusicInstance.release();


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
