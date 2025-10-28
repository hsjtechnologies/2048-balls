using UnityEngine;

public class ToggleSetActive : MonoBehaviour
{

    public void ToggleGameObject(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
