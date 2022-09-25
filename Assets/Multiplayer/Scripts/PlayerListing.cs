using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    public Player player { get; private set; }

    [SerializeField] private Text m_playerName;

    private Image image;

    public void SetPlayerName(Player player)
    {
        m_playerName.text = player.NickName;
        image = GetComponent<Image>();
    }

    public void SetColor(Color color) => image.color = color;
    
    public string GetName() => m_playerName.text;
}
