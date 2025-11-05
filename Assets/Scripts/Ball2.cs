using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Ball2 : MonoBehaviour
{
    [SerializeField]
    private int setObjType;

    private void Start()
    {
        this.gameObject.GetComponent<Animator>().SetInteger("type", setObjType);
    }
}