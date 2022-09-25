using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountInput : MonoBehaviour
{
    private Slider m_slider;
    [SerializeField] private Text m_text;

    public int maxPlayers = 10;
    public int minPlayers = 2;
    public bool forceEvenAmount = true;

    public void Start()
    {
        m_slider = GetComponent<Slider>();
        m_slider.onValueChanged.AddListener(delegate { SetPlayerCount(); });
        m_slider.minValue = minPlayers;
        m_slider.maxValue = maxPlayers;
        m_slider.value = minPlayers;
    }

    public void SetPlayerCount()
    {
        m_text.text = "Maximum Players: " + m_slider.value;
    }

    void Update()
    {
        /* only allow an even amount of players */
        if(forceEvenAmount && m_slider.value %2 != 0) m_slider.value--;
    }
}