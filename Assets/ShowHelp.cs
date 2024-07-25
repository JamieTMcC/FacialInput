using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHelp : MonoBehaviour
{
    /// <summary>
    /// This script simply hides the irrelevant controller mapping for the scenario
    /// </summary>


    void Start()
    {
        Invoke("HideIrrelevantHelper", 0.2f);
    }

    void HideIrrelevantHelper()
    {
        if (Experiment2Main.ScenarioNo == 0)
        {
            this.gameObject.transform.Find("RightHelper").gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.transform.Find("LeftHelper").gameObject.SetActive(false);
        }

    }
}
