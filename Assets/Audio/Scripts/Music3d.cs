using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Music3d : MonoBehaviour
{
    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;


    [Header("Volume")]
    public float Volume;
    public Transform musicposition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicInstance = RuntimeManager.CreateInstance(MusicEvent);
        MusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(musicposition.position));
        MusicInstance.setVolume(Volume);
        MusicInstance.start();
        MusicInstance.release();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
