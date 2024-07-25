using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOrShowPointer : MonoBehaviour
{

    public GameObject pointer;

    //bool togglable = true;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<LineRenderer>().enabled = false;
        pointer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log(pointer.activeSelf);
            pointer.SetActive(!pointer.activeSelf);
        }
    }
}