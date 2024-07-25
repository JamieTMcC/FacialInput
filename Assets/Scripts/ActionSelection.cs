using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelection : MonoBehaviour
{
    public InterpretFacialActions InterpretFacialActions;

    public Material NoCollide, Collide, Activated;

    public GameObject SelectionCanvas,SpawnPoint;

    Experiment2Main Experiment2Main;

    private void Start()
    {
        Experiment2Main = GameObject.Find("Part2Props").GetComponent<Experiment2Main>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.name == "OVRCameraRig Variant")
        {
            return;
        }

        if (collision.gameObject.GetComponent<Renderer>().sharedMaterial != Activated)
        {
            Debug.Log("Collision detected");

            if (collision.gameObject.tag == "SelectionBox")
            {
                collision.gameObject.GetComponent<Renderer>().material = Collide;
            }

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "OVRCameraRig Variant")
        {
            return;
        }

        if (collision.gameObject.GetComponent<Renderer>().sharedMaterial != Activated)
        {
            Debug.Log("Collision exit detected");
            if (collision.gameObject.tag == "SelectionBox" && collision.gameObject.GetComponent<Renderer>().material != Activated)
            {
                collision.gameObject.GetComponent<Renderer>().material = NoCollide;
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.name == "OVRCameraRig Variant")
        {
            return;
        }

        if (OVRInput.Get(OVRInput.Button.One) && collision.gameObject.GetComponent<Renderer>().sharedMaterial == Collide)
        {
            collision.gameObject.GetComponent<Renderer>().material = Activated;
            GameObject instance = Instantiate(SelectionCanvas);
            instance.transform.position = SpawnPoint.transform.position;
            instance.transform.Rotate(0,90f,0);
            //instance.transform.rotation = new Quaternion(instance.transform.rotation.x*-1, instance.transform.rotation.y, instance.transform.rotation.z, 0);
            this.transform.parent.gameObject.GetComponent<LineRenderer>().enabled = true;
            if (Experiment2Main.ScenarioNo < 3)
            {
                instance.GetComponent<CanvasScript>().Initialise(collision.gameObject.transform.parent.gameObject, false);
                instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y, instance.transform.position.z);//accounting for lack of dropdown
            }
            else
            {
                instance.GetComponent<CanvasScript>().Initialise(collision.gameObject.transform.parent.gameObject, true);
            }
        }

    }
}
