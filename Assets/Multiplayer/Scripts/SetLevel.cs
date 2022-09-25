using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

    [RequireComponent(typeof(PhotonView))]
    public class SetLevel : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        public TextMesh levelText;

        public int level = 5;

        public void Awake()
        {
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    break;
                }
            }
            if (!observed) Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            
        
        }


 public void IncreaseLevel()
    {
        //if (!photonView.IsMine) level+=1;
        level += 1;
        //if (PhotonNetwork.IsMasterClient) level += 1;
        //if (view.IsMine) Debug.Log(level);
    }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(level);
            }
            else
            {
                //Network player, receive data
                level = (int)stream.ReceiveNext();
            }
        }

        //private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
        //private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

        public void Update()
        {
            levelText.text = "Level: " + level.ToString();

            if (!photonView.IsMine)
            {
                //Update remote player (smooth this, this looks good, at the cost of some accuracy)
                //transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                //transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            }
        }

    }
