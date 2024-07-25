using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class Experiment2Main : MonoBehaviour
{
    public bool randomise = true;
    public static int ScenarioNo = 0;
    public static int TrialNo = 0;
    public TMP_Text ScenarioTrialText;
    GameObject FaceModels; //Face models should always be in order of 0:MoveForward, 1:TurnRight, 2:TurnLeft, 3:Interact, 4:ShootBullet, 5:MoveBackwards
    OVRFaceExpressions.FaceExpression[,] ScenarioFaceExpressions = new OVRFaceExpressions.FaceExpression[3,6];
    float[,] ScenarioFaceExpressionDefaultIntensities = new float[3,6];
    Counter[] Counters = new Counter[3];
    int TargetScore = 0;
    Stopwatch TimeLogger = new Stopwatch();
    private Dictionary<string, string> FaceExpressionDefaultValues = new Dictionary<string, string>();


    void setDefault(bool resetting= false)
    {
 
        if ((TrialNo == 0 || resetting) && ScenarioNo != 0)
        {
            for (int i = 0; i < FaceModels.transform.childCount; i++)
            {
                FaceModels.transform.GetChild(i).gameObject.GetComponent<DisplayFacialAction>().ThresholdModifier = 1.0f;
                FaceModels.transform.GetChild(i).gameObject.GetComponent<DisplayFacialAction>().SetExpression(ScenarioFaceExpressions[ScenarioNo - 1, i]);
                FaceModels.transform.GetChild(i).gameObject.GetComponent<DisplayFacialAction>().SetDefaultThreshold(ScenarioFaceExpressionDefaultIntensities[ScenarioNo - 1, i]);
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        ScenarioTrialText.text = "Scenario: " + ScenarioNo + "\n Trial: " + TrialNo;
        Default_values();

        //MoveForward, TurnRight, TurnLeft, Interact, ShootBullet, MoveBackwards
        //as determined by the experiment
        ScenarioFaceExpressions[0, 0] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[0, 1] = OVRFaceExpressions.FaceExpression.LidTightenerR;
        ScenarioFaceExpressions[0, 2] = OVRFaceExpressions.FaceExpression.LidTightenerL;
        ScenarioFaceExpressions[0, 3] = OVRFaceExpressions.FaceExpression.JawSidewaysLeft;
        ScenarioFaceExpressions[0, 4] = OVRFaceExpressions.FaceExpression.LipSuckLB;
        ScenarioFaceExpressions[0, 5] = OVRFaceExpressions.FaceExpression.ChinRaiserB;

        ScenarioFaceExpressions[1, 0] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[1, 1] = OVRFaceExpressions.FaceExpression.MouthRight;
        ScenarioFaceExpressions[1, 2] = OVRFaceExpressions.FaceExpression.MouthLeft;
        ScenarioFaceExpressions[1, 3] = OVRFaceExpressions.FaceExpression.LidTightenerR;
        ScenarioFaceExpressions[1, 4] = OVRFaceExpressions.FaceExpression.CheekPuffL;
        ScenarioFaceExpressions[1, 5] = OVRFaceExpressions.FaceExpression.LipSuckLB;

        ScenarioFaceExpressions[2, 0] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[2, 1] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[2, 2] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[2, 3] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[2, 4] = OVRFaceExpressions.FaceExpression.JawDrop;
        ScenarioFaceExpressions[2, 5] = OVRFaceExpressions.FaceExpression.JawDrop;
        for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                ScenarioFaceExpressionDefaultIntensities[j, i] = GetDefaultThreshold(ScenarioFaceExpressions[j, i]);
            }
            ScenarioFaceExpressionDefaultIntensities[2, i] = 1.0f;
        }


        if (ScenarioNo != -1)
        {


            Counters[0] = GameObject.Find("CubeInstruction").GetComponent<Counter>();
            Counters[1] = GameObject.Find("ButtonInstruction").GetComponent<Counter>();
            Counters[2] = GameObject.Find("TargetInstruction").GetComponent<Counter>();

            for (int i = 0; i < Counters.Length; i++)
            {
                TargetScore += Counters[i].count;
            }
            InvokeRepeating("CheckScore", 1.0f, 1.0f);

            FaceModels = GameObject.Find("FacialModels");
            setDefault();

        }
    }

    public int GetScenarioNo()
    {
        return ScenarioNo;
    }

    void CheckScore()
    {
        int score = 0;
        for (int i = 0; i < Counters.Length; i++)
        {
            score += Counters[i].score;
        }
        if (score == TargetScore)
        {

            //write code of announcement here
            GameObject.Find("MovingWall").GetComponent<MoveWall>().BeginMovement();
            CancelInvoke();
        }

    }

    public int GetTrialNo()
    {
        return TrialNo;
    }

    public string GetPlayerLocation()
    {
        return GameObject.Find("OVRCameraRig Variant/TrackingSpace/CenterEyeAnchor").transform.position.x + "|" + GameObject.Find("OVRCameraRig Variant/TrackingSpace/CenterEyeAnchor").transform.position.y + "|" + GameObject.Find("OVRCameraRig Variant/TrackingSpace/CenterEyeAnchor").transform.position.z;
    }

    public string getTime()
    {
        return TimeLogger.ElapsedMilliseconds.ToString();
    }

    public void StartTimer()
    {
        TimeLogger.Start();
    }

    //this includes averages for all expressions not just the ones we use
    private void Default_values() {
        string s = "BrowLowererL 0.24600730000270093\r\nBrowLowererR 0.2023697416323497\r\nCheekPuffL 0.20509834196057222\r\nCheekPuffR 0.11874947891569806\r\nCheekRaiserL 0.16630021262073988\r\nCheekRaiserR 0.16865884007291185\r\nCheekSuckL 0.06400105007636328\r\nCheekSuckR 0.05572559588478119\r\nChinRaiserB 0.2206305005802649\r\nChinRaiserT 0.10060931003458662\r\nDimplerL 0.05985067603088745\r\nDimplerR 0.10502667448502526\r\nInnerBrowRaiserL 0.10504621184111071\r\nInnerBrowRaiserR 0.08847723555053652\r\nJawDrop 0.5555015854625357\r\nJawSidewaysLeft 0.18417654702145048\r\nJawSidewaysRight 0.07863540249142038\r\nJawThrust 0.21578978118671915\r\nLidTightenerL 0.5265218166484248\r\nLidTightenerR 0.4873438090105443\r\nLipCornerDepressorL 0.17027878049378836\r\nLipCornerDepressorR 0.1078927110227238\r\nLipCornerPullerL 0.08905216485205009\r\nLipCornerPullerR 0.11561326967964333\r\nLipFunnelerLB 0.004487643963424901\r\nLipFunnelerLT 0.005558782809960293\r\nLipFunnelerRB 0.0009186792688387595\r\nLipFunnelerRT 0.018033859616608562\r\nLipPressorL 0.0006137376980753574\r\nLipPressorR 0.0008907949375628586\r\nLipPuckerL 0.1436482676717575\r\nLipPuckerR 0.1239708140245048\r\nLipStretcherL 0.027570741541369542\r\nLipStretcherR 0.05358023007460455\r\nLipSuckLB 0.19483203624326723\r\nLipSuckLT 0.0977347795844668\r\nLipSuckRB 0.20332164957414064\r\nLipSuckRT 0.11063478550248877\r\nLipTightenerL 0.013160408092616444\r\nLipTightenerR 0.01679492890448859\r\nLipsToward 0.027044637308469573\r\nLowerLipDepressorL 0.14367603381235025\r\nLowerLipDepressorR 0.22168606557399645\r\nMouthLeft 0.4554982957315331\r\nMouthRight 0.34272897346560743\r\nNoseWrinklerL 0.06749454525097982\r\nNoseWrinklerR 0.15408636160309666\r\nOuterBrowRaiserL 0.0876257253569052\r\nOuterBrowRaiserR 0.08692437198904836\r\nUpperLidRaiserL 0.16174506005931327\r\nUpperLidRaiserR 0.175002462742051\r\nUpperLipRaiserL 0.250953057888783\r\nUpperLipRaiserR 0.3014346973636303";
        string[] lines = s.Split("\r\n");
        foreach (string line in lines)
        {
            string[] parts = line.Split(" ");
            FaceExpressionDefaultValues.Add(parts[0], parts[1]);
        }

    }

    public float GetDefaultThreshold(OVRFaceExpressions.FaceExpression fe)
    {
        return float.Parse(FaceExpressionDefaultValues[fe.ToString()]);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            setDefault(true);
        }
    }
}
