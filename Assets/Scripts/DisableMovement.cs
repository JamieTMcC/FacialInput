using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMovement : MonoBehaviour
{
    InterpretFacialActions InterpretFacialActions;
    void Start()
    {
        InterpretFacialActions = GameObject.Find("Part2Props").GetComponent<InterpretFacialActions>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            InterpretFacialActions.SetDisableForwardMovement(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            InterpretFacialActions.SetDisableForwardMovement(false);
        }
    }
}
