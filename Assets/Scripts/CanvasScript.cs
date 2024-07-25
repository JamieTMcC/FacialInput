using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{

    /// <summary>
    /// Handles Canvas Interaction, depending on scenario can alter thresholds, which expressions are being used, and go between game actions
    /// </summary>
    LineRenderer lineRenderer;

    TMP_Text GameActionText, FacialActionText, ThresholdText, ThresholdModifierText;

    SkinnedMeshRenderer skinnedMeshRenderer;

    GameObject panel; //panel shows on the canvas when the threshold is met

    TMP_Dropdown ExpressionDropdown;

    DisplayFacialAction DSPFA;

    GameObject ActionObject;
    List<OVRFaceExpressions.FaceExpression> usableexpressionlist;

    InterpretFacialActions InterpretFacialActions;

    public Material invisible;

    OVRFaceExpressions FaceExpressions;

    bool expressionEditable;

    GameObject[] FacialModels = new GameObject[6];
    int ChildIndex = 0;

    public void Start()
    {
        this.enabled = false;
        Invoke("CheckScenarioNo", 0.5f);//Delay is needed to ensure that the ScenarioNo is set before the check
    }


    public void Initialise(GameObject ActionObject, bool ExpressionEditable)//ExpressionEditable used only for Scenario 3
    {
        expressionEditable = ExpressionEditable;
        this.ActionObject = ActionObject;
        FaceExpressions = GameObject.Find("OVRCameraRig Variant").GetComponent<OVRFaceExpressions>();

        skinnedMeshRenderer = this.gameObject.transform.Find("HighFidelityMirrored/jupiter_grp/head_ply").GetComponent<SkinnedMeshRenderer>();
        GameActionText = this.transform.Find("GameActionText").GetComponent<TMP_Text>();
        FacialActionText = this.transform.Find("FacialActionText").GetComponent<TMP_Text>();
        ThresholdText = this.transform.Find("ThresholdText").GetComponent<TMP_Text>();
        ThresholdModifierText = this.transform.Find("ThresholdModifierText").GetComponent<TMP_Text>();
        ExpressionDropdown = this.transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        panel = this.transform.Find("Panel").gameObject;
        usableexpressionlist = new List<OVRFaceExpressions.FaceExpression> { OVRFaceExpressions.FaceExpression.JawDrop, OVRFaceExpressions.FaceExpression.LidTightenerL, OVRFaceExpressions.FaceExpression.UpperLipRaiserL, OVRFaceExpressions.FaceExpression.CheekRaiserL, OVRFaceExpressions.FaceExpression.LidTightenerR, OVRFaceExpressions.FaceExpression.MouthLeft, OVRFaceExpressions.FaceExpression.CheekRaiserR, OVRFaceExpressions.FaceExpression.ChinRaiserT, OVRFaceExpressions.FaceExpression.LipCornerDepressorL, OVRFaceExpressions.FaceExpression.LipCornerPullerR, OVRFaceExpressions.FaceExpression.LipPuckerL, OVRFaceExpressions.FaceExpression.UpperLipRaiserR, OVRFaceExpressions.FaceExpression.CheekPuffL, OVRFaceExpressions.FaceExpression.CheekPuffR, OVRFaceExpressions.FaceExpression.DimplerL, OVRFaceExpressions.FaceExpression.LipPuckerR, OVRFaceExpressions.FaceExpression.LipStretcherR, OVRFaceExpressions.FaceExpression.OuterBrowRaiserR, OVRFaceExpressions.FaceExpression.CheekSuckR, OVRFaceExpressions.FaceExpression.LowerLipDepressorR };
        DSPFA = ActionObject.GetComponent<DisplayFacialAction>();
        
        lineRenderer = GameObject.Find("RightHandAnchor").GetComponent<LineRenderer>();
        InterpretFacialActions = GameObject.Find("Part2Props").GetComponent<InterpretFacialActions>();
        
        panel.SetActive(false);
        lineRenderer.enabled = true;
        //InterpretFacialActions.enabled = false;
        

        if (ExpressionEditable)
        {
            ExpressionDropdown.gameObject.SetActive(true);
            //Populate the dropdown with the available Action Units
            ExpressionDropdown.options.Clear();
            ExpressionDropdown.AddOptions(usableexpressionlist.Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList());
            ExpressionDropdown.value = usableexpressionlist.IndexOf(DSPFA.ExpressionChosen);
        }else
        {
            ExpressionDropdown.gameObject.SetActive(false);
        }

        DSPFA.ShapeFace(skinnedMeshRenderer, DSPFA.ExpressionChosen); //shapes the face on the canvas 
        UpdateText();
    }


    void UpdateText()
    {
        GameActionText.text = "Game Action: " + ActionObject.name;
        FacialActionText.text = "Facial Action: " + DSPFA.ExpressionChosen.ToString();
        ThresholdText.text = "Threshold: " + DSPFA.DefaultThreshold;
        ThresholdModifierText.text = "Threshold Modifier: " + DSPFA.ThresholdModifier;
        if(expressionEditable)
            DSPFA.ShapeFace(skinnedMeshRenderer, usableexpressionlist[ExpressionDropdown.value]);
    }

    public void IncreaseThreshold(float threshold)
    {
        DSPFA.ModifyThreshold(threshold*0.01f);
        UpdateText();
    }
    public void ChangeExpression()
    {
        DSPFA.SetExpression(usableexpressionlist[ExpressionDropdown.value]);
        UpdateText();
    }

    public void AcceptChanges()
    {
        lineRenderer.enabled = false;
        ActionObject.transform.Find("SelectionBox").GetComponent<Renderer>().material = invisible;
        InterpretFacialActions.enabled = true;
        GameObject.Find("RightHandAnchor").GetComponent<LineRenderer>().enabled = false;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        //shows a green box around the face on the canvas when the threshold is met
        //useful for user
        if (FaceExpressions[DSPFA.ExpressionChosen] > DSPFA.Threshold)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }

        //For quickness, saves people having to press accept every time
        if(OVRInput.GetDown(OVRInput.Button.Two))
        {
            AcceptChanges();
        }
    }

    public void nextGameAction()
    {
        ChildIndex++;
        if (ChildIndex > GameObject.Find("FacialModels").gameObject.transform.childCount -1)
        {
            ChildIndex = 0;
        }
        Initialise(FacialModels[ChildIndex], expressionEditable);
    }

    public void previousGameAction()
    {
        ChildIndex--;
        if (ChildIndex < 0)
        {
            ChildIndex = GameObject.Find("FacialModels").gameObject.transform.childCount-1;
        }
        Initialise(FacialModels[ChildIndex], expressionEditable);
    }

    private void CheckScenarioNo()
    {
        if (Experiment2Main.ScenarioNo == 0)
        {
            this.gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < GameObject.Find("FacialModels").gameObject.transform.childCount; i++)
        {
            FacialModels[i] = GameObject.Find("FacialModels").transform.GetChild(i).gameObject;
        }
        ChildIndex = 0;
        if (Experiment2Main.ScenarioNo == 3)
            Initialise(FacialModels[ChildIndex], true);
        else
            Initialise(FacialModels[ChildIndex], false);
        this.enabled = true;
    }

}
