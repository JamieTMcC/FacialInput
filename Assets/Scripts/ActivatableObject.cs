using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour
    
{
    /// <summary>
    /// Handles interactions with buttons and the player look collider
    /// </summary>
    
    public static int timesentered = 0; //static int so that it isn't affected by multiple instances of the same object
    public int getTimesEntered()
    {
        return timesentered;
    }

    public InterpretFacialActions InterpretFacialActions;

    public Material NoCollide, Collide, Activated;

    Counter counter;

    private void Start()
    {
        if(gameObject.name != "Helper")
        counter = GameObject.Find("ButtonInstruction").GetComponent<Counter>();
        timesentered = 0;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (this.transform.GetComponent<Renderer>().sharedMaterial != Activated)
        {
            Debug.Log("Collision detected");

            if (collision.gameObject.tag == "PlayerLookCollider")
            {
                this.transform.GetComponent<Renderer>().material = Collide;
                timesentered++;
                Debug.Log("Times entered: " + timesentered);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (this.transform.GetComponent<Renderer>().sharedMaterial != Activated)
        {
            Debug.Log("Collision exit detected");
            if (collision.gameObject.tag == "PlayerLookCollider" && this.transform.GetComponent<Renderer>().material != Activated)
            {
                this.transform.GetComponent<Renderer>().material = NoCollide;
            }
        }
    }

    private void OnTriggerStay()
    {
            if (InterpretFacialActions.getActivatingObjectThresholdPassed() && this.transform.GetComponent<Renderer>().sharedMaterial == Collide)
            {
            Debug.Log("Activating object");
            counter.Increment();
            this.transform.GetComponent<Renderer>().material = Activated;
            this.transform.position = this.transform.position - new Vector3(0, 0.04f, 0);
            enabled = false;
            }
        }
    }
