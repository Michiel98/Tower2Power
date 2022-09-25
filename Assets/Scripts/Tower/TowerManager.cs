using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;

    [Header("Towers")]
    public Tower redTower;
    public Tower blueTower;

    void Awake()
    {
        if (instance != null) GameObject.Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this);
    }
}
