using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPID : MonoBehaviour
{
    public int PID = 0;
    void Awake()
    {
        GameManager.Instance.PID = PID;

    }
}
