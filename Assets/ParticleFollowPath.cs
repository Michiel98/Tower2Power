using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowPath : MonoBehaviour
{
    public string pathName;
    public float time;
    // Start is called before the first frame update

    public void StartAnimation(Vector3 position)
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(position.x, position.y+1, position.z), "easetype", iTween.EaseType.easeInOutSine, "time", time, "oncomplete", "DestroyOnComplete","oncompletetarget", gameObject));
    }

    public void DestroyOnComplete()
    {
        Destroy(gameObject);
        Debug.Log("Destroyed " + gameObject);
    }
}
