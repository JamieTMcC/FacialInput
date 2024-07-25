using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This class mostly handles the logging of the data
    /// also handles the completion checking for each trial/scenario
    /// </summary>

    public static int[] Scenarios = new int[] { 0, 1, 2 };
    public static int ScenariosComplete = 0;
    private int FrameNo = 0;
    public static GameManager Instance;
    public static Experiment2Main Experiment2Main;
    private static PathGenerator pg;
    private static string path = "";
    private DisplayFacialAction[] FacialModels;
    private InterpretFacialActions InterpretFacialActions;
    private OVRInput.Button[] Controller_Inputs;
    private Counter[] Counters;
    private Vector3 PlayerLocation,PlayerRotation;
    private Finish Finish;
    private static bool newtrial = true;
    public static OVRFaceExpressions.FaceExpression[] FaceExpressions = new OVRFaceExpressions.FaceExpression[6];
    public static float[] FaceExpressionDefaultThresholds = new float[6];
    public static float[] FaceExpressionModifiers = new float[6];
    [SerializeField]
    public int PID = 0;
    public int NoOfTrials = 2;

    static GameManager()
    {
        pg = new PathGenerator();
        Instance = null;
    }


    private void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes

            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
        if (path == "")
        {
            Scenarios = Scenarios.OrderBy(x => Guid.NewGuid()).ToArray();
            Debug.Log("Scenario Order" + Scenarios[0] + Scenarios[1] + Scenarios[2]);
        }
        Debug.Log("GameManager Awake");

        //needed to find names for scenario 0 
 /*       FacialModels = new DisplayFacialAction[6];
        for (int i = 0; i < 6; i++)
        {
            FacialModels[i] = GameObject.Find("FacialModels").transform.GetChild(i).gameObject.GetComponent<DisplayFacialAction>();
        }*/
    }


    // Start is called before the first frame update
    void Start()
    {
        OnCollideDisappear.shotsmissed = 0;
        Experiment2Main = GameObject.Find("Part2Props").GetComponent<Experiment2Main>();
        Experiment2Main.ScenarioNo = Scenarios[ScenariosComplete];
        Debug.Log("GameManager Start");
        InvokeRepeating("Log", 1.0f, 1/30f);// Log at 30Hz
    }

    void CollectObjects()
    {
        FrameNo = 0;
        InterpretFacialActions = GameObject.Find("Part2Props").GetComponent<InterpretFacialActions>();
        Experiment2Main = GameObject.Find("Part2Props").GetComponent<Experiment2Main>();
        Counters = new Counter[3];
        Counters[0] = GameObject.Find("CubeInstruction").GetComponent<Counter>();
        Counters[1] = GameObject.Find("ButtonInstruction").GetComponent<Counter>();
        Counters[2] = GameObject.Find("TargetInstruction").GetComponent<Counter>();
        PlayerLocation = GameObject.Find("OVRCameraRig Variant").transform.position;
        PlayerRotation = GameObject.Find("OVRCameraRig Variant").transform.eulerAngles;
        Finish = GameObject.Find("FinishingGoal").GetComponent<Finish>();

    }


    void AdvanceScene()
    {
        if (Experiment2Main.ScenarioNo == 3)
        {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        if (Experiment2Main.randomise)
        {
            ScenariosComplete++;
            if (ScenariosComplete == 3)
            {
                //ScenariosComplete = 0;
                Experiment2Main.ScenarioNo = 3;
                Experiment2Main.randomise = false;
            }
            else
            {
                Experiment2Main.ScenarioNo = Scenarios[ScenariosComplete];
            }

        }
        else
        {
            Experiment2Main.ScenarioNo++;
        }
        Debug.Log("Scenario Number:" + Experiment2Main.ScenarioNo);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void AdvanceTrial()
    {

        OnCollideDisappear.shotsmissed = 0;

        newtrial = true;
        if (Experiment2Main.ScenarioNo != 0)
        {
            for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
            {
                FaceExpressions[i] = FacialModels[i].ExpressionChosen;
                FaceExpressionDefaultThresholds[i] = FacialModels[i].DefaultThreshold;
                FaceExpressionModifiers[i] = FacialModels[i].ThresholdModifier;
            }
        }

        Experiment2Main.TrialNo += 1;
        if (Experiment2Main.TrialNo == NoOfTrials)
        {
            Experiment2Main.TrialNo = 0;
            AdvanceScene();
        }
        Debug.Log("Trial Number:" + Experiment2Main.ScenarioNo);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPosition();
        }

    }

    void Log()
    {

        if(newtrial)
        {
            CollectObjects();
            FacialModels = new DisplayFacialAction[6];
            for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
            {
                FacialModels[i] = GameObject.Find("FacialModels").transform.GetChild(i).gameObject.GetComponent<DisplayFacialAction>();
            }
            Experiment2Main.StartTimer();

        }
        if (path == "" || path.Split("/")[^1] != "Participant_" + PID + "_Scenario_" + Experiment2Main.GetScenarioNo() + ".csv")
        {   
            
            string header = "FrameNo,ParticipantID,ScenarioNo,TrialNo,TimeInTrial,PlayerLocation,CubesPlaced,ButtonsPressed,TargetsShot,CubesDropped,ShotsMissed,ActivationsMissed,ControlsEnabled,";
            if (path != "")
            {
                path = path.Remove(path.LastIndexOf('/') + 1);
            }
            else
            {
                pg.GenerateNewIteration();
                path = pg.GetPath();
            }

            path += "Participant_" + PID + "_Scenario_" + Experiment2Main.GetScenarioNo() + ".csv";

            if (Experiment2Main.GetScenarioNo() == 0)
            {
                Controller_Inputs = new OVRInput.Button[6] {
                    OVRInput.Button.PrimaryThumbstickUp,
                    OVRInput.Button.PrimaryThumbstickDown,
                    OVRInput.Button.PrimaryThumbstickLeft,
                    OVRInput.Button.PrimaryThumbstickRight,
                    OVRInput.Button.PrimaryIndexTrigger,
                    OVRInput.Button.PrimaryHandTrigger};
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
                    {
                        string ActionName = FacialModels[i].gameObject.name;
                        header += ActionName + "_InputButton," + ActionName + "_InputButtonPressed,";
                    }
                    writetext.WriteLine(header);
                }


            }
            else
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {

                    for (int i = 0; i < 6; i++)
                    {
                        string ActionName = FacialModels[i].gameObject.name;
                        header += ActionName + "_AU,"
                               + ActionName + "_WorkingThreshold,"
                               + ActionName + "_AUValue,"
                               + ActionName + "_IsActivated,";
                    }
                    header = header.Remove(header.Length - 1);
                    writetext.WriteLine(header);
                }
            }
        }

        if (newtrial)
        {
            Debug.Log(newtrial);


            for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
            {
                if (Experiment2Main.ScenarioNo != 0 && Experiment2Main.TrialNo != 0)
                {
                    FacialModels[i].SetExpression(FaceExpressions[i]);
                    FacialModels[i].ThresholdModifier = FaceExpressionModifiers[i];
                    FacialModels[i].SetDefaultThreshold(FaceExpressionDefaultThresholds[i]);
                }
                else if (Experiment2Main.ScenarioNo ==0)
                {
                    Destroy(FacialModels[i].gameObject);
                }
            }



            newtrial = false;
            Debug.Log("Reassigned Facial Model Variables From Last Trial or Removed Face Models if Control Scenario");
        }


        string line =FrameNo + "," + PID + "," + Experiment2Main.GetScenarioNo() + "," + Experiment2Main.GetTrialNo() + "," + Experiment2Main.getTime() + "," + Experiment2Main.GetPlayerLocation() + ",";
        for(int i = 0; i < 3; i++)
        {
            line += Counters[i].score.ToString() + ",";
        }
        line += GameObject.Find("Helper").GetComponent<PickUpBlock>().getDrops() + ",";
        line += OnCollideDisappear.shotsmissed + ",";
        line += GameObject.Find("Helper").GetComponent<ActivatableObject>().getTimesEntered() + ",";
        line += (InterpretFacialActions.enabled && InterpretFacialActions.enableControls) + ",";
        if (Experiment2Main.GetScenarioNo() == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                line += Controller_Inputs[i].ToString() + "," + OVRInput.Get(Controller_Inputs[i]) + ",";
            }
        }
        else
        {
            if (InterpretFacialActions.FaceExpressions.ValidExpressions)
            {
                for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
                {
                    line += FacialModels[i].ExpressionChosen +
                        "," + FacialModels[i].Threshold +
                        "," + InterpretFacialActions.FaceExpressions[FacialModels[i].ExpressionChosen] +
                        "," + (InterpretFacialActions.FaceExpressions[FacialModels[i].ExpressionChosen] > FacialModels[i].Threshold) + ",";
                }
            }
        }
        line = line.Remove(line.Length - 1);
        FrameNo++;
        File.AppendAllText(path, line + "\n");

        if (Finish.finished)
        {
            Finish.finished = false;
            ResetPosition();
            AdvanceTrial();
        }
    }


    private void ResetPosition()
    {
        GameObject.Find("OVRCameraRig Variant").transform.position = PlayerLocation;
        GameObject.Find("OVRCameraRig Variant").transform.eulerAngles = PlayerRotation;
    }

}
