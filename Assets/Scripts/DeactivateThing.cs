using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateThing : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision Exit");
        this.gameObject.SetActive(false);
        Invoke("TurnBackOff", 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
    }


    private void TurnBackOff()
    {
        this.gameObject.SetActive(true);
    }
}
