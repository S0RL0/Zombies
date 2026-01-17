using UnityEngine;

public class Door : Interactable
{
    public int doorID;
    public int doorCost;
    protected PlayerController player;

    protected virtual void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    public override InteractionResult Interact()
    {
        if (player == null)
        {
            return new InteractionResult(false, null, null, InteractionType.Door);
        }

        if (player.money < doorCost)
        {
            return new InteractionResult(false, null, null, InteractionType.Door);
        }
        else
        {
            Open();
            return new InteractionResult(true, doorCost, null, InteractionType.Door);
        }
    }

    public virtual void Open()
    {

    }

    public override string GetInteractionText()
    {
        string str = "to Buy Door [Cost: " + doorCost + "]";
        return str;
    }

    public override Sprite GetInteractionIcon()
    {
        return null;
    }
}
