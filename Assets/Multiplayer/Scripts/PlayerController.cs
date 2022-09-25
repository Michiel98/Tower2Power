using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();

        if (view.IsMine) // we own this
        {
            GameObject playerChild = GameObject.FindGameObjectWithTag("PlayerChild");
            playerChild.transform.SetParent(transform);
            transform.position = Vector3.zero;
            playerChild.transform.position = Vector3.zero;
        }
        else // we do not own this
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
