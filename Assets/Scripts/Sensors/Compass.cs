using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class Compass : MonoBehaviour
{
    public static Compass Instance { set; get; }

    private float m_trueHeading; // heading in degrees relative to geographic north

    /* awake */
    void Awake()
    {
        if (Instance != null) GameObject.Destroy(Instance);
        else Instance = this;
        DontDestroyOnLoad(this);

        /* enable compass */
        Input.compass.enabled = true;
    }

    /* called every frame */
    void Update()
    {
        m_trueHeading = Input.compass.trueHeading;
    }

    /* public getters */
    public float f_trueHeading { get { return m_trueHeading; } private set { m_trueHeading = value; } }
}
