using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundEffect3d : MonoBehaviour
{
    [SerializeField] private EventReference SoundEvent;
    private EventInstance SoundInstance;


    [Header("Volume")]
    public Transform musicposition;
    int fXVolume; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int fXVolume = AudioSettingsManager.Instance.FXVolume;
        SoundInstance = RuntimeManager.CreateInstance(SoundEvent);
        SoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(musicposition.position));
        SoundInstance.setVolume(fXVolume);
        SoundInstance.start();
        SoundInstance.release();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
