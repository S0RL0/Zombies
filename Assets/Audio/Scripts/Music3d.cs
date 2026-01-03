using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Music3d : MonoBehaviour
{
    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;


    [Header("Volume")]
    public Transform musicposition;
    private int musicVolume; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int musicVolume = AudioSettingsManager.Instance.MusicVolume;
        MusicInstance = RuntimeManager.CreateInstance(MusicEvent);
        MusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(musicposition.position));
        MusicInstance.setVolume(musicVolume);
        MusicInstance.start();
        MusicInstance.release();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
