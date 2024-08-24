using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeniu : MonoBehaviour
{
    public event Action onClosed;
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        onClosed?.Invoke();
        gameObject.SetActive(false);
    }
}
