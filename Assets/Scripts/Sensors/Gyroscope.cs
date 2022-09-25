using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public static Gyroscope Instance;

    /* private gyroscope data */
    private Quaternion m_attitude; // ortientation in space of device
    private Vector3 m_gravity; // gravity acceleration vector
    private Vector3 m_rotationRate; // rotation rate
    private Vector3 m_rotationRateUnbiased; // unbiased rotation rate
    private Vector3 m_userAcceleration; // acceleration that user is giving to device

    /* awake */
    void Awake()
    {
        if (Instance != null) GameObject.Destroy(Instance);
        else Instance = this;
        DontDestroyOnLoad(this);

        /* enable gyroscope */
        Input.gyro.enabled = true;
    }

    /* called every frame */
    void Update()
    {
        attitude = Input.gyro.attitude;
        gravity = Input.gyro.gravity;
        rotationRate = Input.gyro.rotationRate;
        rotationRateUnbiased = Input.gyro.rotationRateUnbiased;
        userAcceleration = Input.gyro.userAcceleration;
    }

    /* public getters */
    public Quaternion attitude { get { return m_attitude; } private set { m_attitude = value; } }
    public Vector3 gravity { get { return m_gravity; } private set { m_gravity = value; } }
    public Vector3 rotationRate { get { return m_rotationRate; } private set { m_rotationRate = value; } }
    public Vector3 rotationRateUnbiased { get { return m_rotationRateUnbiased; } private set { m_rotationRateUnbiased = value; } }
    public Vector3 userAcceleration { get { return m_userAcceleration; } private set { m_userAcceleration = value; } }
}
