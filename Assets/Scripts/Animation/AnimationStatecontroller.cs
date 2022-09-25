using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStatecontroller : MonoBehaviour
{
    Animator animator;
    public GameObject mine;
    public GameObject rope;
    public GameObject lumber;
    public GameObject towerUpgradeUI;

    private bool wait;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        wait = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mine.active)
            animator.SetBool("isMine", true);
        else
            animator.SetBool("isMine", false);
        if (rope.active)
            animator.SetBool("isRope", true);
        else
            animator.SetBool("isRope", false);
        if (lumber.active)
            animator.SetBool("isWood", true);
        else
            animator.SetBool("isWood", false);
        if (towerUpgradeUI.active)
        {
            animator.SetBool("isHammering", true);
            Debug.Log("wait value" + wait);
            if (!wait)
                StartCoroutine(showSawing());
        }
        else
            animator.SetBool("isHammering", false);



    }

    IEnumerator showSawing()
    {
        wait = true;
        yield return new WaitForSeconds(5f);
        animator.SetBool("isSawing", !animator.GetBool("isSawing"));
        wait = false;
    }
}
