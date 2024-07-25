using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOrientation : MonoBehaviour
{

    public GameObject resetPosition;
    // Start is called before the first frame update
    void Start()
    {


    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressed");

            // reset parent objects position
            transform.parent.position = resetPosition.transform.position;
            transform.parent.rotation = resetPosition.transform.rotation;
        }
    }
}
