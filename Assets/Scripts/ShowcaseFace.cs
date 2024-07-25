using System.Collections;
using UnityEngine;
public class ShowcaseFace : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    private OVRFaceExpressions.FaceExpression currentExpression;

    public bool activateMotion = false;
    public IEnumerator activateFAUondisplaymodel(OVRFaceExpressions.FaceExpression ex)
    {
        activateMotion = true;
        int currentIntensity = 0;
        int iterator = 5;
        if (ex == OVRFaceExpressions.FaceExpression.LipsToward)
        {
            while (activateMotion)
            {
                yield return new WaitForSeconds(0.1f);
                currentExpression = ex;
                currentIntensity += iterator;
                skinnedMeshRenderer.SetBlendShapeWeight(42, currentIntensity);
                skinnedMeshRenderer.SetBlendShapeWeight(43, currentIntensity);
                skinnedMeshRenderer.SetBlendShapeWeight(44, currentIntensity);
                skinnedMeshRenderer.SetBlendShapeWeight(45, currentIntensity);
                if (currentIntensity >= 60)
                {
                    iterator = -5;
                }
                else if (currentIntensity <= 0)
                {
                    iterator = 5;
                }
            }
        }
        else
        {
            currentExpression = ex;
            int a = CorrectExpression(ex);
            while (activateMotion)
            {
                yield return new WaitForSeconds(0.1f);
                currentIntensity += iterator;
                skinnedMeshRenderer.SetBlendShapeWeight(a, currentIntensity);
                if (currentIntensity >= 100)
                {
                    iterator = -5;
                }
                else if (currentIntensity <= 0)
                {
                    iterator = 5;
                }
            }

        }





    }

    public void deactivateFAUondisplaymodel()
    {
        Debug.Log(currentExpression);
        activateMotion = false;
        skinnedMeshRenderer.SetBlendShapeWeight((int)currentExpression, 0);
    }

    public int CorrectExpression(OVRFaceExpressions.FaceExpression ex)
    {
        int a = (int)ex;
        if ((int)ex > 41)
            a += 4; //account for 4 lips toward variables in model
        if((int)ex > 49)
            a -= 1; //account for missing lips toward
        if ((int)ex > 54)
            a += 2; //account for nasallabialfurrows
        if((int)ex > 56)
            a += 4; //account for nostrilcompressor/dilator
        return a;
    }

}
