using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public abstract InteractionResult Interact();
    public abstract string GetInteactionText();
}
