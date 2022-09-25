using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private Button m_createButton;
    [SerializeField] private Button m_joinButton;

    private InputField m_input;

    public void Start()
    {
        m_input = GetComponent<InputField>();
        m_input.onValueChanged.AddListener(delegate { ValidatePlayerName(); });

        m_createButton.onClick.AddListener(delegate { SetPlayerName(); });
        m_joinButton.onClick.AddListener(delegate { SetPlayerName(); });

        m_createButton.interactable = false;
        m_joinButton.interactable = false;
    }

    public void ValidatePlayerName()
    {
        m_createButton.interactable = m_joinButton.interactable = !string.IsNullOrEmpty(m_input.text);
    }

    public void SetPlayerName()
    {
        PhotonNetwork.NickName = m_input.text;
        Debug.Log("Set nickname to " + PhotonNetwork.NickName + ".");
    }
}