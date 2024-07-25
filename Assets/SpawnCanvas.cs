using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCanvas : MonoBehaviour
{

    /// <summary>
    /// Hides the canvas when the user is not interacting with it, Disables controls when the user is interacting with the canvas
    /// </summary>
    public GameObject SelectionCanvas;

    Experiment2Main Experiment2Main;

    InterpretFacialActions InterpretFacialActions;

    // Start is called before the first frame update
    void Start()
    {
        Experiment2Main = GameObject.Find("Part2Props").GetComponent<Experiment2Main>();
        InterpretFacialActions = GameObject.Find("Part2Props").GetComponent<InterpretFacialActions>();
        this.enabled = false;
        Invoke("checkScenarioNo", 0.5f);//Delay is needed to ensure that the ScenarioNo is set before the check
        GameObject.Find("RightHandAnchor").GetComponent<LineRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            GameObject.Find("RightHandAnchor").GetComponent<LineRenderer>().enabled = true; //Enable the line renderer
            SelectionCanvas.SetActive(true);
            Debug.Log("Should activate canvas");
            InterpretFacialActions.enabled = false;
        }
    }

    void checkScenarioNo()
    {
        SelectionCanvas.SetActive(false);
        if (Experiment2Main.ScenarioNo == 0) // no facial input for scenario 0
            this.enabled = false;
        else
            this.enabled = true;
    }
}
