using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField] private GameObject m_playerListingPrefab;

    private List<PlayerListing> m_playerListings = new List<PlayerListing>();

    private void PlayerJoinedRoom(Player player)
    {
        if(player == null) return;

        PlayerLeftRoom(player);

        GameObject playerListing = Instantiate(m_playerListingPrefab);
        playerListing.transform.SetParent(transform, false);

        PlayerListing listing = playerListing.GetComponent<PlayerListing>();
        listing.SetPlayerName(player);

        m_playerListings.Add(listing);
    }

    private void PlayerLeftRoom(Player player)
    {
        int index = m_playerListings.FindIndex(x => x.player == player);

        if(index != -1)
        {
            Destroy(m_playerListings[index].gameObject);
            m_playerListings.RemoveAt(index);
        }
    }

    private void OnJoinedRoom()
    {
        /* get player list */
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            PlayerJoinedRoom(players[i]);
        }
    }

    private void OnPlayerEnteredRoom(Player player)
    {
        PlayerJoinedRoom(player);
    }

    private void OnPlayerLeftRoom(Player player)
    {
        PlayerLeftRoom(player);
    }
}
