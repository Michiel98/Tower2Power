using System.Collections;
using UnityEngine;

public class OutlinePulsator : MonoBehaviour
{
    public Material material;

    float currentValue = 1;

    Coroutine routine;

    void Start() => routine = StartCoroutine(Pulsate());

    IEnumerator Pulsate()
    {
        while (true)
        {
            while (this.currentValue != 1f)
            {
                currentValue = Mathf.MoveTowards(currentValue, 1f, 0.02f);
                material.SetFloat("_Transparency", currentValue);
                yield return new WaitForEndOfFrame();
            }

            while (this.currentValue != 0f)
            {
                currentValue = Mathf.MoveTowards(currentValue, 0f, 0.02f);
                material.SetFloat("_Transparency", currentValue);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}