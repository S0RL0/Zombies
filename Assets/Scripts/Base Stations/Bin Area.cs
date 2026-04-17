using UnityEngine;



public enum BinLevel
{
    Level1,
    Level1Upgrading,
    Level2,
    Level2Upgrading,
    Level3,
}

public class BinArea : MonoBehaviour
{
    public string UImessage;
    public float holdTime;
    public PlayerInteract playerInteract;
    public BinLevel Binlevel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetText();
        SetSpeed();
        playerInteract = CleaningManager.Instance.playerInteract;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText()
    {

     UImessage = "Hold E To Dump Trash";

    }

    public void SetSpeed()
    {

     holdTime = 1.5f;

    }

    public void PreformAction()
    {
        playerInteract.AmountHeld = 0;
        playerInteract.UpdateSlider();
        playerInteract.Full = false;

    }

    public void Active()
    {

    }

    public void Inactive()
    {

    }
}
