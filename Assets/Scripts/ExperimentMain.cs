using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;


/// <summary>
/// explaination of the experiment
/// calibration of neutral face
/// iterate through expressions normal (and intense)
/// pause between each expression for quiz
/// 
/// features:
/// ability to go backwards
/// display face expression on model
/// ?toggle face expression on model/ model constantly changing
/// 
/// 
/// </summary>

public class ExperimentMain : MonoBehaviour
{


    public TMP_Text InstructionText;

    public GameObject IntensityBar, MirroringModel, Slider;

    public ShowcaseFace showcaseFace;

    private bool canProgress = false, nextExpression = false, inExpressionSection = false, activateIntensityBar = false, refreshedJoyStickInput = true;

    private int currentExpression = 0;

    public OVRFaceExpressions FaceDetector;

    private static List<int> usableAUs = Enumerable.Range(0, 70).ToList(), usableAUsforLogging = Enumerable.Range(0,70).ToList();

    public static float[] Neutral_Expressions = new float[Enum.GetNames(typeof(OVRFaceExpressions.FaceExpression)).Length];

    public static float[] Normal_Expressions = new float[Enum.GetNames(typeof(OVRFaceExpressions.FaceExpression)).Length];

    private float[] Intense_Expressions = new float[Enum.GetNames(typeof(OVRFaceExpressions.FaceExpression)).Length];

    public PathGenerator pg;

    public bool SkipIntro, SkipCalibration, SkipRound1, SkipIntro2, SkipRound2, SkipOutro, SkipIntense, use1stSet, use2ndSet, useOnlyRight, useOnlyLeft, Randomise;
    public Stopwatch stopwatch = new Stopwatch();

    Coroutine Recording,SodItCoroutine;
    List<OVRFaceExpressions.FaceExpression> usableexpressionlist;
    //logging tools
    public int participantID = 0;
    private int trialNumber = 0;
    public bool recordOutofTrial = false, sodItRecordBoth = false;
    private int frameNumber = 0;
    private string trialPhase;

    private static System.Random rng = new System.Random();  

    public void Shuffle(List<List<int>> mylistoflists)  
    {  
        int n = mylistoflists.Count;
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            List<int> value = mylistoflists[k];  
            mylistoflists[k] = mylistoflists[n];  
            mylistoflists[n] = value;  
        }  
    }


    void Start()
    {
        if(sodItRecordBoth)
            recordOutofTrial = false;
        Slider.SetActive(false);
        this.gameObject.transform.Find("AUreminder").GetComponent<TMP_Text>().text = "";
        pg.GenerateNewIteration();
        InstructionText.text = "";
        IntensityBar.transform.localScale = new Vector3(0.25f, 0.0f, 0.25f);
        usableAUs = new List<int>() { 0, 3, 4, 7, 8, 9, 10, 23, 24, 25, 27, 29, 30, 33, 34, 35, 39, 40, 43, 44, 45, 49, 50, 51, 54, 55, 58, 59, 62 };//LR
        if (use2ndSet)
            usableAUs = new List<int>() { 1, 2, 5, 6, 8, 9, 11, 22, 24, 26, 27, 28, 31, 32, 36, 37, 38, 41, 42, 46, 47, 48, 50, 52, 53, 56, 57, 60, 61 };//RL
        if (useOnlyRight)
            usableAUs = new List<int>() { 1, 3, 5, 7, 8, 9, 11, 23, 24, 26, 27, 29, 31, 33, 36, 37, 39, 41, 43, 46, 47, 49, 50, 52, 54, 56, 58, 60, 62 };//RR
        if (useOnlyLeft)
            usableAUs = new List<int>() { 0, 2, 4, 6, 8, 9, 10, 22, 24, 25, 27, 28, 30, 32, 34, 35, 38, 40, 42, 44, 45, 48, 50, 51, 53, 55, 57, 59, 61 };//LL
        if (!use1stSet && !use2ndSet && !useOnlyRight && !useOnlyLeft)
        {
            usableAUs.AddRange(new List<int>() { 1, 2, 5, 6, 8, 9, 11, 22, 24, 26, 27, 28, 31, 32, 36, 37, 38, 41, 42, 46, 47, 48, 50, 52, 53, 56, 57, 60, 61 });
            usableAUs = usableAUs.Distinct().ToList();
        }
        usableAUs.Sort();
        usableexpressionlist = usableAUs.Cast<OVRFaceExpressions.FaceExpression>().ToList();
        usableAUsforLogging = usableAUs;
        if(Randomise){
            List<int> list2 = new List<int>() { 0, 2, 4, 6, 8, 9, 10, 22, 24, 25, 27, 28, 30, 32, 34, 35, 38, 40, 42, 44, 45, 48, 50, 51, 53, 55, 57, 59, 61 };//LL
            List<int> list1 = new List<int>() { 1, 3, 5, 7, 8, 9, 11, 23, 24, 26, 27, 29, 31, 33, 36, 37, 39, 41, 43, 46, 47, 49, 50, 52, 54, 56, 58, 60, 62 };//RR
            List<List<int>> listoflist = new List<List<int>>() {};
            for(int i=0; i <list2.Count; i++){
                var newlist = new List<int>() {list2[i], list1[i]};
                listoflist.Add(newlist);
            }
            Shuffle(listoflist);
            for(int i=0; i <listoflist.Count; i++){
                Debug.Log(listoflist[i][0]);
            }
            usableAUs = listoflist.SelectMany(x=>x).ToList();
            string allinastring = "";
            foreach(int i in usableAUs){
                allinastring += i.ToString() + ","; 
            }
            Debug.Log(allinastring);
            usableAUs = usableAUs.Distinct().ToList();
            allinastring = "";
            foreach(int i in usableAUs){
                allinastring += i.ToString() + ","; 
            }
            Debug.Log(allinastring + "     " + usableAUs.Count());

        }

        foreach (int i in usableAUs)
        {
            Debug.Log((OVRFaceExpressions.FaceExpression)i);
            // StartCoroutine(showcaseFace.activateFAUondisplaymodel((OVRFaceExpressions.FaceExpression)i));
        }

        StartCoroutine(InstructionCycle());

    }


    IEnumerator InstructionCycle()
    {
        yield return new WaitForSeconds(1.0f);
        if (!SkipIntro)
        {
            yield return setInstructionText("Welcome to the experiment");
            yield return waitForInput();
            yield return setInstructionText("The aim of this experiment is to determine which parts of the face could be used as input for a game or application");
            yield return waitForInput();
            MirroringModel.SetActive(true);
            yield return setInstructionText("The quest pro is capable of facial tracking, see how the face and torso will mimic your actions");
            yield return waitForInput();
            yield return setInstructionText("You can take a moment to observe the capabilities of this tracking, when you continue the model will disappear");
            yield return waitForInput();
            MirroringModel.SetActive(false);
            yield return setInstructionText("You will be tasked with replicating the actions done by the large head");
            yield return waitForInput();
            yield return setInstructionText("You will be given a short quiz after each expression");
            yield return waitForInput();
            yield return setInstructionText("We begin by calibrating your natural face intensities");
            yield return waitForInput();
        }
        if (!SkipCalibration) {
            yield return setInstructionText("Please make a neutral face and when ready press A to lock in a natural state");
            yield return CalibrateNeutralFaceExpression();
            yield return waitForInput();
        }
        if (!SkipIntro)
        {
            yield return setInstructionText("Now we will begin the main part of the experiment");
            yield return waitForInput();
            yield return setInstructionText("Observe the changes made to the face and attempt to accurately replicate them to the greatest extent before losing comfort or control");
            yield return waitForInput();
            yield return setInstructionText("Imagine the face to be like a mirror. The text above will " +
                "end in a letter which corresponds to the side of your face you should alter");
            yield return waitForInput();
            yield return setInstructionText("You will be given a countdown which after you should hold the expression until the text says 'release', you will do this 3 times");
            yield return waitForInput();
            yield return setInstructionText("You will be given time before this where you can observe and practice the expression");
            yield return waitForInput();
        }
        currentExpression = 0;
        if (!SkipRound1)
        {
            activateIntensityBar = true;
            while (currentExpression < usableAUs.Count)
            {
                trialPhase = "waiting";
                this.gameObject.transform.Find("AUreminder").GetComponent<TMP_Text>().text = (currentExpression + 1).ToString() + "/53" + ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString();
                //startDisplayingFaceExpressiononModel()
                Coroutine Displaying = StartCoroutine(showcaseFace.activateFAUondisplaymodel((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]));


                yield return setInstructionText("Practice" + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]));
                yield return waitForInput();


                //Normal_Expressions[currentExpression] = FaceDetector[(OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]];


                if (recordOutofTrial)
                {
                    for (int i =0; i<3; i++)
                    {
                    Recording = StartCoroutine(StartRecording("Recording_Participant_" + participantID.ToString()));
                    frameNumber = 0;
                    stopwatch.Start();
                    InstructionText.text = "Get Ready";
                    yield return new WaitForSeconds(2.0f);
                    for (int j = 0; j < 2; j++)
                    {
                        InstructionText.text = "Starting in " + (2 - j).ToString();
                        yield return new WaitForSeconds(1.0f);
                    }
                    trialNumber = i + 1;
                    InstructionText.text = "Hold the expression";
                    yield return new WaitForSeconds(3.0f);
                    InstructionText.text = "Release";
                    PrintCompoundingFAUS("Recording_Participant_" + participantID.ToString(), true);
                    trialNumber = 0;
                    yield return new WaitForSeconds(2.0f);
                    stopwatch.Stop();
                    stopwatch.Reset();
                    }
                    StopCoroutine(Recording);
                }
                else{
                    if(sodItRecordBoth)
                    {
                        Coroutine SodItCoroutine = StartCoroutine(StartRecording("OOT_Recording_Participant_"+ participantID.ToString()));
                    }

                for (int i = 0; i < 3; i++)
                {
                    
                    InstructionText.text = "Get Ready";
                    yield return new WaitForSeconds(2.0f);
                    for (int j = 0; j < 2; j++)
                    {
                        InstructionText.text = "Starting in " + (2 - j).ToString();
                        yield return new WaitForSeconds(1.0f);
                    }
                    frameNumber = 0;
                    stopwatch.Start();
                    Recording = StartCoroutine(StartRecording("Recording_Participant_" + participantID.ToString()));
                    trialNumber = i + 1;
                    InstructionText.text = "Hold the expression";
                    trialPhase = "holding";
                    yield return new WaitForSeconds(3.0f);
                    InstructionText.text = "Release";
                    trialPhase = "released";
                    PrintCompoundingFAUS("Recording_Participant_" + participantID.ToString(), true);
                    yield return new WaitForSeconds(1.0f);
                    StopCoroutine(Recording);
                    stopwatch.Stop();
                    stopwatch.Reset();
                    trialPhase = "waiting";
                    yield return new WaitForSeconds(2.0f);

                }
                }

                if(sodItRecordBoth){
                    StopCoroutine(SodItCoroutine);
                }

                //StopDisplayingFaceExpressiononModel()
                showcaseFace.deactivateFAUondisplaymodel();
                StopCoroutine(Displaying);

                yield return new WaitForSeconds(0.5f);



                inExpressionSection = true;


                yield return setInstructionText("Please Answer The Questions");
                yield return waitForInput();
                InstructionText.text = "";

                inExpressionSection = false;

                yield return slider_interaction();

                currentExpression += 1;
                for (int i = 0; i < 100; i++)
                {
                    showcaseFace.skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
                }


            }
            activateIntensityBar = false;
        }
        else if (!SkipRound2)
        {
            currentExpression = 63;
        }
        if (!SkipIntro2)
        {
            yield return setInstructionText("We have almost finished the experiment. The facial tracking software recently updated and now supports tongue tracking.");
            yield return waitForInput();
            yield return setInstructionText("Unfortunately the models provided do not support this and so we will see demonstrations");
            yield return waitForInput();

        }
        if (!SkipRound2)
        {
            activateIntensityBar = true;
            while (currentExpression < usableAUs.Count)
            {

                yield return setInstructionText("Normal - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]));
                yield return waitForInput(true);
                Normal_Expressions[currentExpression] = FaceDetector[(OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]];

                PrintCompoundingFAUS("Normal_Expressions_" + ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString());




                yield return setInstructionText("Intense - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]));
                yield return waitForInput(true);
                Intense_Expressions[currentExpression] = FaceDetector[(OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]];

                PrintCompoundingFAUS("Intense_Expressions_" + ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString());

                currentExpression += 1;

                inExpressionSection = true;
                yield return setInstructionText("If you want to retry press B, otherwise press A to continue");
                yield return waitForInput();
                inExpressionSection = false;
            }
            activateIntensityBar = false;

        }

        //PrintActivatedExpressions();


        yield return setInstructionText("Thank you for taking part in this experiment. You may now take off the headset.");
        yield return waitForInput();
        //SceneManager.LoadScene(sceneName: "ExperimentPart2");

    }

    IEnumerator CalibrateNeutralFaceExpression()
    {
        while (!nextExpression)
        {
            yield return new WaitForSeconds(0.05f);
        }
        string path = pg.GetPath() + "Calibration_Participant_" + participantID.ToString() + ".csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {
            foreach (OVRFaceExpressions.FaceExpression ex in usableexpressionlist)
            {
                if (ex == OVRFaceExpressions.FaceExpression.Max)
                {
                    break;
                }
                Debug.Log(ex.ToString() + "," + FaceDetector[ex]);
                Neutral_Expressions[(int)ex] = FaceDetector[ex];

                writetext.WriteLine(ex.ToString() + "," + FaceDetector[ex]);
            }
        }
    }

    IEnumerator StartRecording(String Filename)
    {
        while (true)
        {
            PrintCompoundingFAUS(Filename);
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator waitForInput(bool DisplayExpression = false) {
        while (!nextExpression)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (DisplayExpression)
        {
            SetIntensityBarHeight(0.0f);
        }
        nextExpression = false;
    }


    IEnumerator setInstructionText(String InstructionMessage)
    {
        InstructionText.text = InstructionMessage;
        yield return new WaitForSeconds(2.0f);
        InstructionText.text += "\n Press A to Continue";
        if (inExpressionSection)
        {
            InstructionText.text += "\n Press B to go back";
        }
        canProgress = true;
    }

    void SetIntensityBarHeight(float intensityValue)
    {
        IntensityBar.transform.localScale = new Vector3(0.25f, intensityValue, 0.25f);
    }

    void PrintCompoundingFAUS(String Filename, bool endoftrialframe = false)
    {
        string path = pg.GetPath() + Filename + ".csv";
        if (!File.Exists(path))
        {
            Debug.Log("Create File");
            using (StreamWriter writetext = new StreamWriter(path))
            {
                string header = "PID,AUofFocus,TrialNo,TrialPhase,TimeInTrial,EndOfTrialFrame,TrialFrameNo";

                foreach (OVRFaceExpressions.FaceExpression ex in usableexpressionlist)
                {
                    if (ex == OVRFaceExpressions.FaceExpression.Max)
                    {
                        break;
                    }
                    header += "," + ex.ToString();
                }
                Debug.Log(header);
                writetext.WriteLine(header);
            }
        }
        Debug.Log("Append to File");
        //shaping output PID,AUofFocus,TrialNo,TimeInTrial,EndOfTrialFrame,TrialFrameNo...
        string output = "";
        output += participantID.ToString() + ",";
        output += ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString() + ",";
        output += trialNumber.ToString() + ",";
        output += trialPhase + ",";
        output += stopwatch.ElapsedMilliseconds.ToString() + ",";
        output += endoftrialframe + ",";
        output += frameNumber.ToString();

        frameNumber++;

        foreach (OVRFaceExpressions.FaceExpression ex in usableexpressionlist)
        {
            if (ex == OVRFaceExpressions.FaceExpression.Max)
            {
                break;
            }
            output += "," + FaceDetector[ex];
        }

        File.AppendAllText(path, output + "\n");
    }

    void PrintActivatedExpressions()
    {
        string path = pg.GetPath() + "Normal_Expressions.csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {
            int i = 0;
            while (i < usableAUs.Count)
            {
                writetext.Write(Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[i]) + ",");
                writetext.Write(Normal_Expressions[i]);
                writetext.Write("\n");
                i++;
            }
        }
        if (!SkipIntense)
        {

            path = pg.GetPath() + "Intense_Expressions.csv";
            using (StreamWriter writetext = new StreamWriter(path))
            {
                var values = Enum.GetValues(typeof(OVRFaceExpressions.FaceExpression));
                foreach (OVRFaceExpressions.FaceExpression ex in values)
                {
                    if (ex == OVRFaceExpressions.FaceExpression.Max)
                    {
                        break;
                    }
                    writetext.WriteLine(ex.ToString() + "," + Intense_Expressions[(int)ex]);
                }

            }
        }
    }


    void Update()
    {
        if (canProgress && (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Return)))
        {
            nextExpression = true;
            canProgress = false;
        }
        if ((activateIntensityBar))
        {
            SetIntensityBarHeight(FaceDetector[(OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]]);
        }
        if (Slider.activeSelf && ((OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft)))) && refreshedJoyStickInput)
        {
            Slider.GetComponent<Slider>().value -= 1;
            refreshedJoyStickInput = false;
            Slider.gameObject.transform.Find("SliderValue").GetComponent<TMP_Text>().text = Slider.GetComponent<Slider>().value.ToString();

        }
        if (Slider.activeSelf && ((OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)))) && refreshedJoyStickInput)
        {
            Slider.GetComponent<Slider>().value += 1;
            refreshedJoyStickInput = false;
            Slider.gameObject.transform.Find("SliderValue").GetComponent<TMP_Text>().text = Slider.GetComponent<Slider>().value.ToString();

        }
        if (Slider.activeSelf && !((OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))) || ((OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))))))
        {
            refreshedJoyStickInput = true;
        }

    }


    IEnumerator slider_interaction()
    {
        Slider.SetActive(true);
        String[] Questions = new String[] { "How comfortable was the expression?", "How much effort did it take to make the expression?", "How well would you rate your performace in replicating the expression?" };
        String[] LeftText = new String[] { "Very Comfortable", "No Effort", "Replicated Perfectly" };
        String[] RightText = new String[] { "Very Uncomfortable", "Maximum Effort", "Failed to Replicate" };
        int[] qvalues = new int[3];
        for (int i = 0; i < 3; i++)
        {
            Slider.GetComponent<Slider>().value = 5;
            Slider.gameObject.transform.Find("SliderValue").GetComponent<TMP_Text>().text = Slider.GetComponent<Slider>().value.ToString();
            Slider.gameObject.transform.Find("SliderQuestion").GetComponent<TMP_Text>().text = Questions[i];
            Slider.gameObject.transform.Find("LeftTextSlider").GetComponent<TMP_Text>().text = LeftText[i];
            Slider.gameObject.transform.Find("RightTextSlider").GetComponent<TMP_Text>().text = RightText[i];
            yield return new WaitForSeconds(1.0f);
            canProgress = true;
            while (!nextExpression)
            {
                yield return new WaitForSeconds(0.1f);
            }
            nextExpression = false;
            qvalues[i] = (int)Slider.GetComponent<Slider>().value;
        }
        printQuestionnaire(qvalues, Questions);
        Slider.SetActive(false);
    }

    void printQuestionnaire(int[] qvalues, String[] Topics)
    {
        string path = pg.GetPath() + "Questionnaire_Participant_" + participantID.ToString() + ".csv";
        if (!File.Exists(path))
        {
            using (StreamWriter writetext = new StreamWriter(path))
            {

                writetext.WriteLine("ParticipantID,AUofFocus,Discomfort,Borg RPE,Performance");
            }
        }
        string output = "";
        output += participantID.ToString() + ",";
        output += ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString() + ",";
        output += qvalues[0].ToString() + ",";
        output += qvalues[1].ToString() + ",";
        output += qvalues[2].ToString();
        File.AppendAllText(path, output + "\n");
    }
}
