using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;

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
    float holdTimer;
    InteractableObject currentInteractable;

    [Header("Cleaning Stats")]
    public int AmountHeld;
    public int Maxamount;
    public bool Full;


    private void Start()
    {
        // Will need changing when saves get implemented
        Maxamount = 5;
        Full = false;
    }


    void OnEnable()
    {
        interactAction.action.Enable();
    }

    void OnDisable()
    {
        interactAction.action.Disable();
    }

     void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
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
                    ResetInteraction();
                }

                return;
            }


            if (hit.collider.TryGetComponent(out InteractableObject Bin) && interactable.Bin)
            {
                Debug.Log("Hit");

                currentInteractable = Bin;
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
                            Bin.PerformAction();
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

             if(currentInteractable.Wallpaper)
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
        TrashSlider.value = AmountHeld ;

    }


}
