using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    public OVRFaceExpressions FaceDetector;
    public TMP_Text InstructionText;
    // Start is called before the first frame update
    void Start()
    {
        InstructionText.text = "";
        List<int> usableAUs = Enumerable.Range(0, 63).ToList();
        foreach(int i in usableAUs)
        {
            Debug.Log((OVRFaceExpressions.FaceExpression)i);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
