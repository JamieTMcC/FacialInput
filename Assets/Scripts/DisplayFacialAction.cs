using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFacialAction : MonoBehaviour
{
    /// <summary>
    /// This class serves as both a container for the game action attributes and as a means to
    /// give the user a visual representation of the facial action they are currently using
    /// </summary>

    public OVRFaceExpressions.FaceExpression ExpressionChosen;
    public float ThresholdModifier = 1.0f;
    //[SerializeField]
    public float Threshold;

    public float DefaultThreshold = 1.0f; //We need a default threshold to maintain an appropriate value since otherwise the threshold modifier will scale with the threshold

    public SkinnedMeshRenderer skinnedMeshRenderer; //This is just the face model

    public TMPro.TMP_Text ActionUnitText,ThresholdText;
    void Awake()
    {
        ActionUnitText = this.transform.Find("ActionUnitText").GetComponent<TMPro.TMP_Text>();
        ActionUnitText.text = ExpressionChosen.ToString();
        skinnedMeshRenderer = this.transform.Find("HighFidelityMirrored/jupiter_grp/head_ply").GetComponent<SkinnedMeshRenderer>();
        ThresholdText = this.transform.Find("ThresholdText").GetComponent<TMPro.TMP_Text>();
        ThresholdText.text = "Threshold: " + Threshold.ToString();

    }

    public void SetExpression(OVRFaceExpressions.FaceExpression faceExpression)
    {
        ExpressionChosen = faceExpression;
        ActionUnitText.text = faceExpression.ToString();

        ShapeFace(this.skinnedMeshRenderer, faceExpression);

    }


    // There isn't a one-to-one mapping between the OVRFaceExpressions.FaceExpression enum and the blendshapes in the model
    // This function attempts to correct for that but some expressions just don't work well on the model
    public void ShapeFace(SkinnedMeshRenderer skinnedMeshRenderer, OVRFaceExpressions.FaceExpression faceExpression)
    {
        for (int i = 0; i < 100; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
        }

        if (faceExpression == OVRFaceExpressions.FaceExpression.LipsToward)//^ Like this one
        {
            skinnedMeshRenderer.SetBlendShapeWeight(42, 100);
            skinnedMeshRenderer.SetBlendShapeWeight(43, 100);
            skinnedMeshRenderer.SetBlendShapeWeight(44, 100);
            skinnedMeshRenderer.SetBlendShapeWeight(45, 100);
        }
        else
        {
            int a = CorrectExpression(faceExpression);
            skinnedMeshRenderer.SetBlendShapeWeight(a, 100);
        }
    }

    public void SetDefaultThreshold(float newThreshold)
    {
        DefaultThreshold = newThreshold;
        Threshold = DefaultThreshold * ThresholdModifier;
        ThresholdText.text = "Threshold: " + Threshold.ToString();

    }

    public void ModifyThreshold(float modifier)
    {
        ThresholdModifier = ThresholdModifier + modifier;
        Threshold = DefaultThreshold * ThresholdModifier;
        ThresholdText.text = "Threshold: " + Threshold.ToString();
    }

    public int CorrectExpression(OVRFaceExpressions.FaceExpression ex)
    {
        int a = (int)ex;
        if ((int)ex > 41)
            a += 4; //account for 4 lips toward variables in model
        if ((int)ex > 49)
            a -= 1; //account for missing lips toward
        if ((int)ex > 54)
            a += 2; //account for nasallabialfurrows
        if ((int)ex > 56)
            a += 4; //account for nostrilcompressor/dilator
        return a;
    }
}
