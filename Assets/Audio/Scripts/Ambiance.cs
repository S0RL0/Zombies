using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;

public class Ambiance : MonoBehaviour
{
    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;

    [SerializeField] private EventReference RandomEvent;
    private EventInstance RandomInstance;


    [Header("Volume")]
    public int random;
    





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        int musicVolume = AudioSettingsManager.Instance.MusicVolume;

        MusicInstance = RuntimeManager.CreateInstance(MusicEvent);
        MusicInstance.setVolume(musicVolume);
        MusicInstance.start();
        MusicInstance.release();

        StartCoroutine(RandomSounds());


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RandomSounds()
    {   
        int fXVolume = AudioSettingsManager.Instance.FXVolume;
        RandomInstance = RuntimeManager.CreateInstance(RandomEvent);
        RandomInstance.setVolume(fXVolume);
        RandomInstance.start();
        RandomInstance.release();
        random = (Random.Range(10, 12)); 

        yield return new WaitForSeconds(random);
        StartCoroutine(RandomSounds());

    }


}
