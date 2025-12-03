using System;
using UnityEngine;

[Serializable]
public class InteractionResult
{
    public bool success;
    public object item; // optional; can be null
    public GameObject sourceObject; // optional; can be null

    public InteractionResult(bool s, object i = null, GameObject g = null)
    {
        success = s;
        item = i;
        sourceObject = g;
    }

    public T GetItem<T>() where T : class => item as T;
}
