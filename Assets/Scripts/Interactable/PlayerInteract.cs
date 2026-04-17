using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;
    public bool CleaningMode;

    [Header("UI")]
    public Slider progressBar;
    public GameObject TextHolder;
    public TextMeshProUGUI PressE;
    public Slider TrashSlider;
    public GameObject SorryFull;
    public GameObject Empty;

    [Header("Refs")]
    public LayerMask interactLayer;
    public InputActionReference interactAction;
    public InputActionReference CleaningAction;
    float holdTimer;
    InteractableObject currentInteractable;
    BinArea CurrentBin;

    [Header("Cleaning Stats")]
    public int AmountHeld;
    public int Maxamount;
    public bool Full;
    public bool Soundplaying;
    private bool timer; 

    [Header("Audio")]
    private EventInstance MusicInstance;
    [SerializeField] public EventReference MusicInstanceEvent;



    private void Start()
    {
        // Will need changing when saves get implemented
        Maxamount = 5;
        Full = false;
        CleaningMode = false;
        timer = false;


    }


    void OnEnable()
    {
        interactAction.action.Enable();
        CleaningAction.action.Enable();

    }

    void OnDisable()
    {
        interactAction.action.Disable();
        CleaningAction.action.Disable();

    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (!CleaningMode)
            if (CleaningAction.action.IsPressed() && !timer)
            {
                PlayMusic();
                CleaningMode = true;
                timer = true; 
                StartCoroutine(CleaningToggle());

            }


        if (CleaningMode)
        {

            if (CleaningAction.action.IsPressed() && !timer)
            {
                CleaningMode = false;
                StopMusic();
                timer = true;
                StartCoroutine(CleaningToggle());
                ResetInteraction();

                if (currentInteractable != null)
                {
                    currentInteractable.GetComponent<InteractableObject>()?.stopsound();
                }

            }



            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            {
                // Cleanable item
                if (hit.collider.TryGetComponent(out InteractableObject interactable) && interactable.Cleanable)
                {
                    Debug.Log("Hit");

                    currentInteractable = interactable;
                    PressE.text = interactable.UImessage;
                    TextHolder.SetActive(true);


                    if (interactAction.action.IsPressed())
                    {
                        if (!Full)
                        {

                            // Generic
                            TextHolder.SetActive(false);
                            holdTimer += Time.deltaTime;
                            progressBar.gameObject.SetActive(true);
                            progressBar.value = holdTimer / interactable.holdTime;
                            float progress = holdTimer / currentInteractable.holdTime;
                            progressBar.value = progress;

                            //Sound

                            if (!Soundplaying)
                            {
                                currentInteractable.GetComponent<InteractableObject>()?.playsound();
                                Soundplaying = true;
                            }

                            //Wipeable
                            if (interactable.Wipeable)
                            {
                                interactable.StartCleaning();
                                currentInteractable.GetComponent<InteractableObject>()?.UpdateCleaning(progress);

                            }

                            if (interactable.Wallpaper)
                            {
                                currentInteractable.GetComponent<InteractableObject>()?.UpdateWallPaper(progress);
                            }



                            if (holdTimer >= interactable.holdTime)
                            {
                                interactable.PerformAction();
                                interactable.Cleanable = false;
                                ResetInteraction();
                                Soundplaying = false;
                                currentInteractable.GetComponent<InteractableObject>()?.stopsound();



                            }
                        }


                        if (Full)
                        {
                            TextHolder.SetActive(false);
                            SorryFull.SetActive(true);
                        }
                    }
                    else if (holdTimer > 0)
                    {
                        currentInteractable.GetComponent<InteractableObject>()?.stopsound();
                        Soundplaying = false;
                        ResetInteraction();
                    }

                    return;
                }

                // Bin Logic
                if (hit.collider.TryGetComponent(out BinArea Bin))
                {
                    CurrentBin = Bin;
                    PressE.text = Bin.UImessage;
                    TextHolder.SetActive(true);

                    if (interactAction.action.IsPressed())
                    {
                        if (AmountHeld > 0)
                        {
                            TextHolder.SetActive(false);
                            holdTimer += Time.deltaTime;
                            progressBar.gameObject.SetActive(true);
                            progressBar.value = holdTimer / Bin.holdTime;
                            float progress = holdTimer / currentInteractable.holdTime;
                            progressBar.value = progress;


                            if (holdTimer >= Bin.holdTime)
                            {
                                Bin.PreformAction();
                                ResetInteraction();
                            }
                        }

                        if (AmountHeld == 0)
                        {
                            Empty.SetActive(true);

                        }


                    }
                    else if (holdTimer > 0)
                    {
                        ResetInteraction();

                    }
                    return;
                }
            }

            
        }

        ResetInteraction();
    }

    void ResetInteraction()
    {
        holdTimer = 0;
        progressBar.value = 0;
        progressBar.gameObject.SetActive(false);
        TextHolder.SetActive(false);
        SorryFull.SetActive(false);
        Empty.SetActive(false);







        //Wipeable
        if (currentInteractable != null)
        {
            currentInteractable.GetComponent<InteractableObject>()?.UpdateCleaning(0);
            currentInteractable.GetComponent<InteractableObject>()?.StopCleaning();

            if (currentInteractable.Wallpaper)
            {
                currentInteractable.GetComponent<InteractableObject>()?.UpdateWallPaper(0);
                currentInteractable.GetComponent<InteractableObject>()?.StopWallPaper();

            }
        }

    }

    public void Cleaned()
    {

        if (!Full)
        {
            AmountHeld = AmountHeld + 1;
        }

        if (AmountHeld == Maxamount)
        {
            Full = true;
        }

        UpdateSlider();
    }

    public void UpdateSlider()
    {
        TrashSlider.maxValue = Maxamount;
        TrashSlider.value = AmountHeld;

    }

    public void PlayMusic()
    {
        MusicInstance = RuntimeManager.CreateInstance(MusicInstanceEvent);
        MusicInstance.setVolume(CleaningManager.Instance.Music);
        MusicInstance.start();
        MusicInstance.release();

    }

    public void StopMusic()
    {
        MusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MusicInstance.release();

    }

    IEnumerator CleaningToggle()
    {
         yield return new WaitForSeconds(2f); // Wait 2 seconds
        timer = false; 

    }
}
