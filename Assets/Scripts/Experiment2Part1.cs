using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.IO;


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

public class Experiment2Part1: MonoBehaviour
{
    public TMP_Text InstructionText;

    public GameObject IntensityBar;

    public ShowcaseFace showcaseFace;

    private bool canProgress = false, nextExpression = false, activateIntensityBar = false;

    private int currentExpression = 0;

    public OVRFaceExpressions FaceDetector;

    private List<int> usableAUs = new List<int> { 0, 1, 24, 25, 26, 32, 33, 53, 54 };//examples change later

    private List<float> Neutral_Expressions = new List<float>();

    private List<List<float>> repetitions_logging, timespan_logging = new List<List<float>>();

    private PathGenerator pg = new PathGenerator();

    public bool SkipIntro, SkipCalibration;

    public int repetitions = 20, timespan = 20;

    void Start()
    {
        for(int i = 12; i < 22; i++)
        {
            usableAUs.Remove(i);
        }
        pg.GenerateNewIteration();
        InstructionText.text = "";
        IntensityBar.transform.localScale = new Vector3(0.25f, 0.0f, 0.25f);

        foreach(int i in usableAUs)
        {
            Debug.Log((OVRFaceExpressions.FaceExpression)i);
            StartCoroutine(showcaseFace.activateFAUondisplaymodel((OVRFaceExpressions.FaceExpression)i));
        }
        
        //StartCoroutine(InstructionCycle());
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
            yield return setInstructionText("In this experiment you will be tasked with performing these 'facial action units' repetitively and for long stretches of time");
            yield return waitForInput();
            yield return setInstructionText("Look at the name of the action unit and the face model and attempt to replicate this model");
            yield return waitForInput();
            yield return setInstructionText("First we will calibrate your natural facial state");
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
            yield return setInstructionText("Observe the changes made to the face on the right and attempt to replicate them");
            yield return waitForInput();
        }
       currentExpression = 0;
            activateIntensityBar = true;

            float currentExpression_threshold = 0;
            DateTime start = DateTime.UtcNow;
            DateTime finish = DateTime.UtcNow;
            TimeSpan repetitions_timespan = finish - start;
            while (currentExpression < usableAUs.Count)
            {
                //Starts the model making the expressions
                StartCoroutine(showcaseFace.activateFAUondisplaymodel((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]));

                setInstructionText("Set your threshold by making the expression and pressing A");
                waitForInput();
                currentExpression_threshold = getFloatValueFromIndex(currentExpression) *0.75f;//0.75 reduces the threshold to give some leeway
                repetitions_logging.Add(new List<float>());

                yield return setInstructionText("Repeated - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]));
                yield return waitForInput();

                start = DateTime.UtcNow;
                for (int i = 0; i < repetitions; i++)
                {
                    InstructionText.text = "Repeated - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]) + i.ToString() + "/20";

                    while (getFloatValueFromIndex(currentExpression) < currentExpression_threshold)
                    {
                        yield return new WaitForSeconds(0.05f);
                    }
                    repetitions_logging[currentExpression].Add(getFloatValueFromIndex(currentExpression));
                }
                currentExpression_threshold = repetitions_logging[currentExpression].Average() *0.85f; //should be a more consistent number so we can use a smaller threshold

                finish = DateTime.UtcNow;
                repetitions_timespan = finish - start;
                repetitions_logging[currentExpression].Add((float)repetitions_timespan.TotalSeconds);


            //PrintAllExpressions at point where current expression is activated
            PrintCompoundingFAUS("Repeated Expressions" + ((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]).ToString());

                timespan_logging.Add(new List<float>());
                yield return setInstructionText("Hold the expression until the timer reaches 0. The timer will begin as soon as you press A.");
                yield return waitForInput();
                yield return setInstructionText("Hold - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]));
                yield return waitForInput();

                
                for (int i = 0; i < timespan; i++)
                {
                    for(int j = 0; j < 10; j++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        timespan_logging[currentExpression].Add(getFloatValueFromIndex(currentExpression));
                        if(getFloatValueFromIndex(currentExpression) < currentExpression_threshold)
                        {
                            InstructionText.text = "Below Threshold - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]) + i.ToString() + "/20";
                        }else
                        {
                            InstructionText.text = "Above Threshold - " + Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[currentExpression]) + i.ToString() + "/20";
                        }   
                    }
                }
                
                showcaseFace.deactivateFAUondisplaymodel();
                StopCoroutine(showcaseFace.activateFAUondisplaymodel((OVRFaceExpressions.FaceExpression)usableAUs[currentExpression]));

                yield return new WaitForSeconds(0.5f);

                currentExpression += 1;

                for (int i = 0; i < 100; i++)
                {
                    showcaseFace.skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
                }


            }
            activateIntensityBar = false;

        PrintActivatedExpressions();


        yield return setInstructionText("Thank you for participating in this experiment");
        yield return waitForInput();
        yield return setInstructionText("Please remove the headset and listen to the debriefing");

    }

    IEnumerator CalibrateNeutralFaceExpression()
    {
        while (!nextExpression)
        {
            yield return new WaitForSeconds(0.05f);
        }
        var values = Enum.GetValues(typeof(OVRFaceExpressions.FaceExpression));
        string path = pg.GetPath() + "Neutral_Expressions.csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {
                foreach (OVRFaceExpressions.FaceExpression ex in values)
            {
                if(ex == OVRFaceExpressions.FaceExpression.Max)
                {
                    break;
                }
                Debug.Log(ex.ToString() + "," + FaceDetector[ex]);
                Neutral_Expressions[(int)ex] = FaceDetector[ex];

                    writetext.WriteLine(ex.ToString() + "," + FaceDetector[ex]);
                }
        }
    }

    IEnumerator waitForInput(bool DisplayExpression = false) { 
        while (!nextExpression)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if(DisplayExpression)
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
        canProgress = true;
    }

    void SetIntensityBarHeight(float intensityValue)
    {
        IntensityBar.transform.localScale = new Vector3(0.25f, intensityValue, 0.25f);
    }

    void PrintCompoundingFAUS(String Filename)
    {
        string path = pg.GetPath() + Filename + ".csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {


            var values = Enum.GetValues(typeof(OVRFaceExpressions.FaceExpression));
            foreach (OVRFaceExpressions.FaceExpression ex in values)
            {
                if (ex == OVRFaceExpressions.FaceExpression.Max)
                {
                    break;
                }
                writetext.WriteLine(ex.ToString() + "," + FaceDetector[ex]);
            }
        }



    }

    void PrintActivatedExpressions()
    {
        int i = 0;
        string path = pg.GetPath() + "Repeated_Expressions.csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {
            i = 0;
            while (i < usableAUs.Count)
            {
                writetext.Write(Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[i]) + ",");
                foreach (float f in repetitions_logging[i])
                {
                    writetext.WriteLine(f.ToString() +",");
                }
                writetext.Write("\n");
                i++;
            }

        }

        path = pg.GetPath() + "Timed_Expressions.csv";
        using (StreamWriter writetext = new StreamWriter(path))
        {
            i = 0;
            while (i < usableAUs.Count)
            {
                writetext.Write(Enum.GetName(typeof(OVRFaceExpressions.FaceExpression), usableAUs[i]) + ",");
                foreach (float f in timespan_logging[i])
                {
                    writetext.WriteLine(f.ToString() + ",");
                }
                writetext.Write("\n");
                i++;
            }

        }
    }

    float getFloatValueFromIndex(int index)
    {
        return FaceDetector[(OVRFaceExpressions.FaceExpression)usableAUs[index]];
    }



    void Update()
    {
        if (canProgress && OVRInput.Get(OVRInput.Button.One))
        {
            nextExpression = true;
            canProgress = false;
        }
        if ((activateIntensityBar))
        {
            SetIntensityBarHeight(getFloatValueFromIndex(currentExpression));
        }
    }

}