using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class SoundEffectTrIgger : MonoBehaviour
{
    [SerializeField] private EventReference SoundEvent;
    private EventInstance SoundInstance;


    [Header("Volume")]
    public Transform musicposition;
    int fXVolume;
    bool ready;
    public int timer; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int fXVolume = AudioSettingsManager.Instance.FXVolume;
        ready = true; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ready)
            {
                int fXVolume = AudioSettingsManager.Instance.FXVolume;
                SoundInstance = RuntimeManager.CreateInstance(SoundEvent);
                SoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(musicposition.position));
                SoundInstance.setVolume(fXVolume);
                SoundInstance.start();
                SoundInstance.release();
                ready = false; 
                StartCoroutine(Timer());

            }

        }
    }

    IEnumerator Timer()
    {

        
        yield return new WaitForSeconds(timer);
        ready = true; 
    }
}
