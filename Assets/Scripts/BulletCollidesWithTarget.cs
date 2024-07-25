using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollidesWithTarget : MonoBehaviour
{

    private Counter counter;

    void Start()
    {
        counter = GameObject.Find("TargetInstruction").GetComponent<Counter>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            counter.Increment();
            Destroy(collision.gameObject.transform.parent.gameObject); // Destroy full target
            Destroy(gameObject);
        }
    }
}
