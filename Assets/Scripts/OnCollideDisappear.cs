using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollideDisappear : MonoBehaviour
{
    public static int shotsmissed = 0;
    private void Start()
    {
        if (gameObject.name != "Helper")
        Invoke("DestroySelf", 5);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "PlayerLookCollider")
        {
            Invoke("DestroySelf", 0.03f);
        }
    }

    private void DestroySelf()
    {
        try {
            shotsmissed++;
            Debug.Log("Shots missed: " + shotsmissed);
            Destroy(gameObject); }
        catch {; }
    }
}
