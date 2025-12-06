using System;
using UnityEngine;

[Serializable]
public class InteractionResult
{
    public bool success;
    public object item; // optional; can be null
    public GameObject sourceObject; // optional; can be null
    public InteractionType interactionType;

    public InteractionResult(bool s, object i = null, GameObject g = null, InteractionType it = default)
    {
        success = s;
        item = i;
        sourceObject = g;
        this.interactionType = it;
    }

    public T GetItem<T>() where T : class => item as T;
}

public enum InteractionType
{
    None,
    Weapon,
    Door,
    Buy
}
