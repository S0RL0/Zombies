using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AsylumMusic : MonoBehaviour
{
    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;


    [Header("Volume")]
    public float Music;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
