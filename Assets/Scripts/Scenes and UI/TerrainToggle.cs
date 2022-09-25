using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainToggle : MonoBehaviour
{

    public void ToggleTerrain()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
