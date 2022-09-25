using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    MeshRenderer rend;

    void Start() => rend = gameObject.GetComponent<MeshRenderer>();

     private bool pulsating = true;

    public void EnableOutline()
    {
        rend.renderingLayerMask = 1u << 4 - 1;
    }

    public void DisableOutline()
    {
        rend.renderingLayerMask = 1u << 45- 1;
    }
}
