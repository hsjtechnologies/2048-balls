using System;
using UnityEngine;
using UnityEngine.Events;

public class ToggleSetActive : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<bool> Callback = null;

    public void ToggleGameObject(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);

        if(Callback != null)
        {
            Callback.Invoke(obj.activeSelf);
        }
    }
}
