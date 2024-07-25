using Meta.WitAi.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public bool finished = false;

    private void Start()
    {
        finished = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            finished = true;
        }
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Return))
        {
            finished = true;
        }
    }
}
