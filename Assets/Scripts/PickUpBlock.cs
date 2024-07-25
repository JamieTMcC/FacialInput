using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpBlock : MonoBehaviour
{
    /// <summary>
    /// Handles the pickup and drop of the blocks
    /// </summary>


    public static int Drops = 0;
    public InterpretFacialActions InterpretFacialActions;

    public Material NoCollide, Collide, Pickup;

    public GameObject AnchorPoint;
    public Counter counter;

    private bool dropChecker = false;

    public bool beingHeld = false;

    public GameObject cube1object, cube2object;
    PickUpBlock cube1, cube2;

    private void Start()
    {
        if(this.gameObject.name == "Helper")
        {
            return;
        }
        cube1 = cube1object.GetComponent<PickUpBlock>();
        cube2 = cube2object.GetComponent<PickUpBlock>();
        Drops = 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && !dropChecker)
        {
            Drops++;
            Debug.Log("Drops: " + Drops);
            dropChecker = true;
            Invoke("Droppable", 0.5f);
        }
    }
    void Droppable()
    {
        dropChecker = false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (cube1.beingHeld || cube2.beingHeld)
        {
            return;
            
        }

        if (!beingHeld)
        {
            Debug.Log(this.transform.parent != collision.gameObject.transform);
            if (collision.gameObject.tag == "PlayerLookCollider" && this.transform.parent != collision.gameObject.transform)
            {
                this.transform.GetComponent<Renderer>().material = Collide;
            }
            if(collision.gameObject.tag == "Bin")
            { 
                counter.Increment();
                Destroy(this.gameObject);}
        }


    }

    private void OnTriggerExit(Collider collision)
    {
        if (!beingHeld)
        {
            Debug.Log(this.transform.parent != collision.gameObject.transform);
            if (collision.gameObject.tag == "PlayerLookCollider" && this.transform.parent != collision.gameObject.transform)
            {
                this.transform.GetComponent<Renderer>().material = NoCollide;
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {

        if(cube1.beingHeld || cube2.beingHeld)
        {
            return;
        }

        if (InterpretFacialActions.getHoldingObjectThresholdPassed() && collision.gameObject.tag == "PlayerLookCollider" && !beingHeld)
        {
            this.transform.GetComponent<Renderer>().material = Pickup;
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<Rigidbody>().isKinematic = true;
            //this.transform.parent = AnchorPoint.transform;
            //collision.gameObject.SetActive(false);
            beingHeld = true;
        }
    }

    private void Update()
    {
        if (this.gameObject.name != "Helper")
        {
            if (beingHeld)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, AnchorPoint.transform.position, 1f);
                this.transform.GetComponent<Renderer>().material = Pickup;

            }

            if (!InterpretFacialActions.getActivatingObjectThresholdPassed() && beingHeld)
            {
                this.transform.GetComponent<Renderer>().material = NoCollide;
                this.GetComponent<Rigidbody>().useGravity = true;
                this.GetComponent<Rigidbody>().isKinematic = false;
                // GameObject.FindGameObjectWithTag("PlayerLookCollider").SetActive(true);
                beingHeld = false;
            }
        }
    }

    //We keep track of drops as a way to measure performance
    public int getDrops()
    {
        return Drops;
    }   
}
