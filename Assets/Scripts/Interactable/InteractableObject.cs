using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

public enum InteractableType
{
    TrashPickup, 
    Clean,
    Door,
    Window,
    Pickup,
    Dusting,
    Grass, 
    wallpaper,
    Bin, 
}
public class InteractableObject : MonoBehaviour

{
    [Header("Type")]

    public InteractableType interactType;

    [Header("Ignore, These auto fill")]
    public float holdTime = 0f;
    public string UImessage ;
    float PickupSpeed = 0.05f;
    bool FinisedTrash = false;
    public bool Cleanable = true;
    float cleanProgress;
    public bool Fade;

    [Header("Blood Decals")]
    public bool Wipeable = false; 
    public float wipeSpeed = 2f;
    public float wipeWidth = 3f;
    public float heightOffset = 0.0f;
    public Material decalMaterial;
    GameObject sponge; 

    [Header("Wallpaper")]
    public bool Wallpaper = false;
    public PlayerInteract playerInteract;
    DecalProjector decal;
  

    // event refs
    private EventInstance SweepInstance;
    private EventInstance SpongeInstance;


    public void Start()
    {
        SetText();
        SetSpeed();
        playerInteract = CleaningManager.Instance.playerInteract;
        
    }
    public void SetText()
    {
        switch (interactType)
        {
            case InteractableType.Clean:
                UImessage = "Hold E To Clean";
                break;

            case InteractableType.Door:
                UImessage = "Hold E To Clean";

                break;

            case InteractableType.Window:
                UImessage = "Hold E To Clean";
                break;

            case InteractableType.Pickup:
                UImessage = "Hold E To Clean";
                break;

            case InteractableType.TrashPickup:
                UImessage = "Hold E To Pick Up Trash";
                break;

            case InteractableType.Dusting:
                UImessage = "Hold E To Dust";
                break;

            case InteractableType.Grass:
                UImessage = "Hold E To Cut Grass";
                break;

            case InteractableType.wallpaper:
                UImessage = "Hold E To Patch";
                break;



        }


    }

    public void SetSpeed()
    {
        switch (interactType)
        {
            case InteractableType.Clean:
                holdTime = 4f;
                Fade = true;
                Wipeable = true; 
                decal = GetComponent<DecalProjector>();                
                break;

            case InteractableType.Door:
                holdTime = 2f;
                break;

            case InteractableType.Window:
                holdTime = 2f;
                break;

            case InteractableType.Pickup:
                holdTime = 2f;
                break;

            case InteractableType.TrashPickup:
                holdTime = 0.9f;
                break;

            case InteractableType.Dusting:
                holdTime = 2f; 
                break;

            case InteractableType.Grass:
                holdTime = 1.5f;
                break;

            case InteractableType.wallpaper:
                holdTime = 1.5f;
                Wallpaper = true;
                break;

            
        }


    }
    public void PerformAction()
    {
        switch (interactType)
        {
            case InteractableType.Clean:
                break;

            case InteractableType.Door:
                
                break;

            case InteractableType.Window:
                //Debug.Log("Window opened");
                break;

            case InteractableType.Pickup:
                //Debug.Log("Picked up item");
                break;

            case InteractableType.TrashPickup:
                FinisedTrash = true; 
                break;

            case InteractableType.Dusting:
                GameObject effect = Instantiate(CleaningManager.Instance.DustClean, transform.position, Quaternion.identity);
                Destroy(effect, 2f);

                Destroy(gameObject);
                playerInteract.Cleaned();

                break;

            case InteractableType.Grass:
                Destroy(gameObject);
                playerInteract.Cleaned();

                break;


            case InteractableType.Bin:
                playerInteract.AmountHeld = 0;
                playerInteract.UpdateSlider();
                playerInteract.Full = false;
                

                break;
        }
    }

   

    #region SpongeableDecals
    public void StartCleaning()
    {
        if (sponge != null) return;

      

        Vector3 spawnPos = transform.position + transform.forward * heightOffset;

        sponge = Instantiate(
           CleaningManager.Instance.spongePrefab,
            spawnPos,
            transform.rotation,
            transform
        ); 
    }

    public void StopCleaning()
    {
        if (sponge != null)
        {
            Destroy(sponge);
            sponge = null;
        }
    }

    public void UpdateCleaning(float progress)
    {
        cleanProgress = progress;

        if (sponge == null) return;

      

        float width = decal.size.x * 0.5f;
        float height = decal.size.y * 0.5f;

        int rows = 6; // how many wipe passes
        float rowProgress = progress * rows;

        int currentRow = Mathf.FloorToInt(rowProgress);
        float rowT = rowProgress - currentRow;

        // reverse direction every row
        if (currentRow % 2 == 1)
            rowT = 1f - rowT; 


        float wipeX = Mathf.Lerp(-width, width, rowT);
        float wipeY = Mathf.Lerp(height, -height, (float)currentRow / rows);

        wipeX += Mathf.Sin(Time.time * 20f) * 0.01f;
        wipeY += Mathf.Cos(Time.time * 18f) * 0.01f;

        Vector3 offset =
            transform.right * wipeX +
            transform.up * wipeY +
            transform.forward * heightOffset;

        sponge.transform.position = transform.position + offset;


        sponge.transform.rotation =
     Quaternion.LookRotation(transform.forward, transform.up) *
     Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 15f) * 10f);

        // fade decal
        decal.fadeFactor = 1f - progress;

        if (progress >= 1f)
        {
            Destroy(sponge);
            Destroy(gameObject);
            playerInteract.Cleaned();

        }
    }
    #endregion


    #region WallpaperDecals
    public void StartWallPaper()
    {
       
    }

    public void StopWallPaper()
    {
        

        decalMaterial.SetFloat("_CutHeight", -0.5f);
    }

    public void UpdateWallPaper(float progress)
    {
        cleanProgress = progress;

        UnityEngine.Rendering.Universal.DecalProjector projector;
        Material decalMaterial;



        projector = GetComponent<UnityEngine.Rendering.Universal.DecalProjector>();

        if (projector == null)
        {
            Debug.LogError("No DecalProjector found on this object.");
            return;
        }

        if (projector.material == null)
        {
            Debug.LogError("DecalProjector has no material assigned.");
            return;
        }

        // Create a unique material instance
        decalMaterial = new Material(projector.material);
        projector.material = decalMaterial;


        decalMaterial.SetFloat("_CutHeight", Mathf.Lerp(-0.5f, 0.5f, progress));



        if (progress >= 1f)
        {
            //Destroy(sponge);
            Destroy(gameObject);
            playerInteract.Cleaned();


        }
    }
    #endregion


  
    private void Update()
    {
        if (FinisedTrash)
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, PickupSpeed);
            if (Vector3.Distance(transform.position, Camera.main.transform.position) < 0.001f)
            {

                playerInteract.Cleaned();
                Destroy(gameObject);

            }
        }
    }

    public void playsound()
    {

        switch (interactType)
        {
            case InteractableType.Clean:

                SpongeInstance = RuntimeManager.CreateInstance(CleaningManager.Instance.SpongeSound);
                SpongeInstance.setVolume(CleaningManager.Instance.cleaning);
                SpongeInstance.start();
                SpongeInstance.release();
                break;

            case InteractableType.Door:

                break;

            case InteractableType.Window:
                
                break;

            case InteractableType.Pickup:

                break;

            case InteractableType.TrashPickup:

                break;

            case InteractableType.Dusting:

                SweepInstance = RuntimeManager.CreateInstance(CleaningManager.Instance.SweepSound);
                SweepInstance.setVolume(CleaningManager.Instance.cleaning);
                SweepInstance.start();
                SweepInstance.release();

                break;

            case InteractableType.Grass:

                break;


            case InteractableType.Bin:
                
                break;
        }

    }

    public void stopsound()
    {
        switch (interactType)
        {
            case InteractableType.Clean:
                SpongeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                SpongeInstance.release();
                break;

            case InteractableType.Door:

                break;

            case InteractableType.Window:

                break;

            case InteractableType.Pickup:

                break;

            case InteractableType.TrashPickup:

                break;

            case InteractableType.Dusting:
                SweepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                SweepInstance.release(); 
                break;

            case InteractableType.Grass:

                break;


            case InteractableType.Bin:

                break;
        }
    }

}


