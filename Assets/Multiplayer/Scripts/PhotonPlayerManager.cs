using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayerManager : MonoBehaviour
{
    public static PhotonPlayerManager instance { set; get; }
    private int m_id = 0;

    void Awake()
    {
        if (instance != null) GameObject.Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    public int GetId() => m_id++ % 2;

}
