using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class EntryBuyableDoor : MonoBehaviour
{
    [Header("Effects")]

    public GameObject[] LazerEffect;
    public GameObject Explosion;
    public GameObject[] Ballons;

    [Header("SoundEffects")]

    int fXVolume;
    [SerializeField] private EventReference BuyEvent;
    private EventInstance BuyInstance;
    [SerializeField] private EventReference NotEnoughMoneyEvent;
    private EventInstance NotEnoughMoneyInstance;
    [SerializeField] private EventReference LazerEvent;
    private EventInstance LazerInstance;
    [SerializeField] private EventReference ExplosionEvent;
    private EventInstance ExplosionInstance;
    [SerializeField] private EventReference LightsEvent;
    private EventInstance LightsInstance;
    [SerializeField] private EventReference MusicEvent;
    private EventInstance MusicInstance;

    [Header("Lights")]
    public GameObject[] Lights;
    

    [Header("Objects to destroy")]
    public GameObject[] ItemsToDestroy;
    public GameObject Ticket; 

    [Header("Objects to Turn On")]
    public GameObject[] ItemstoTurnOn; 

    [Header("Emission")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float emissionIntensity = 2f;

    private bool Triggered; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        fXVolume = AudioSettingsManager.Instance.FXVolume;
        targetMaterial.SetColor("_EmissionColor", Color.black);
        targetMaterial.DisableKeyword("_EMISSION");

        for (int i = 0; i < Lights.Length; i++)
        {
            if (Lights[i] != null)
            {

                Lights[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!Triggered)
        {
            if (other.CompareTag("Player"))
            {

                Open();
                Triggered = true; 
            }
        }
    }
    void Open() 
    {
        
        BuyInstance = RuntimeManager.CreateInstance(BuyEvent);
        BuyInstance.setVolume(fXVolume);
        BuyInstance.start();
        BuyInstance.release();
        Ticket.SetActive(false);
        for (int i = 0; i < Lights.Length; i++)
        {
            if (Lights[i] != null)
            {

                Lights[i].SetActive(true);
            }
        }
        StartCoroutine(Music());

    }

    IEnumerator Music()
    {


        yield return new WaitForSeconds(2);
        MusicInstance = RuntimeManager.CreateInstance(MusicEvent);
        MusicInstance.setVolume(fXVolume);
        MusicInstance.start();
        MusicInstance.release();
        StartCoroutine(Startup());


    }

    IEnumerator Startup()
    {


        yield return new WaitForSeconds(19);
        LightsInstance = RuntimeManager.CreateInstance(LightsEvent);
        LightsInstance.setVolume(fXVolume);
        LightsInstance.start();
        LightsInstance.release();
        targetMaterial.EnableKeyword("_EMISSION");
        targetMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        
        StartCoroutine(LazerTimer());

    }

    IEnumerator LazerTimer() 
    {

        yield return new WaitForSeconds(2);

        for (int i = 0; i < LazerEffect.Length; i++)
        {
            if (LazerEffect[i] != null)
            {

                LazerEffect[i].SetActive(true);
            }
        }

        LazerInstance = RuntimeManager.CreateInstance(LazerEvent);
        LazerInstance.setVolume(fXVolume);
        LazerInstance.start();
        LazerInstance.release();
        StartCoroutine(ExplosionTimer());



    }

    IEnumerator ExplosionTimer()
    {

        yield return new WaitForSeconds(12);

        for (int i = 0; i < LazerEffect.Length; i++)
        {
            if (LazerEffect[i] != null)
            {

                LazerEffect[i].SetActive(false);
            }
        }

        ExplosionInstance = RuntimeManager.CreateInstance(ExplosionEvent);
        ExplosionInstance.setVolume(fXVolume);
        ExplosionInstance.start();
        ExplosionInstance.release();

        for (int i = 0; i < ItemsToDestroy.Length; i++)
        {
            if (ItemsToDestroy[i] != null)
            {

                ItemsToDestroy[i].SetActive(false);
            }
        }
        Explosion.SetActive(true);

        for (int i = 0; i < ItemstoTurnOn.Length; i++)
        {
            if (ItemstoTurnOn[i] != null)
            {

                ItemstoTurnOn[i].SetActive(false);
            }
        }


    }



}

