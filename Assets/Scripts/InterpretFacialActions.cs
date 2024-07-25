using Oculus.Interaction.OVR.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InterpretFacialActions : MonoBehaviour
{
    /// <summary>
    /// This is the main class that interprets all inputs the user can use
    /// It also either sets flags or performs action correlated to the in game actions
    /// </summary>

    DisplayFacialAction MoveForward, TurnRight, TurnLeft, Interact, ShootBullet, MoveBackwards;

    public OVRFaceExpressions FaceExpressions;

    public TMP_Text DebugText;

    public GameObject Player, bullet;

    public GameObject ForwardDirectionObject;

    public Experiment2Main Experiment2Main;

    public float rotationSpeed = 5.0f;
    public float movespeed = 5.0f;

    public bool DebugMode = false, enableControls = false, give_warning = true;

    bool HoldingObjectThresholdPassed = false, ActivatingObjectThresholdPassed = false, bulletOnCooldown = false, disableForwardMovement = false;
    void Start()
    {
        Experiment2Main = GameObject.Find("Part2Props").GetComponent<Experiment2Main>();
        if (Experiment2Main.GetScenarioNo() == 0)
        {
            return;
        }

        if (!DebugMode)
        {
            DebugText.text = "";
        }
        GameObject FaceModels = GameObject.Find("FacialModels");



        MoveForward = FaceModels.transform.GetChild(0).transform.GetComponent<DisplayFacialAction>();
        TurnRight = FaceModels.transform.GetChild(1).transform.GetComponent<DisplayFacialAction>();
        TurnLeft = FaceModels.transform.GetChild(2).transform.GetComponent<DisplayFacialAction>();
        Interact = FaceModels.transform.GetChild(3).transform.GetComponent<DisplayFacialAction>();
        ShootBullet = FaceModels.transform.GetChild(4).transform.GetComponent<DisplayFacialAction>();
        MoveBackwards = FaceModels.transform.GetChild(5).transform.GetComponent<DisplayFacialAction>();



        ForwardDirectionObject = GameObject.Find("OVRCameraRig Variant/TrackingSpace/CenterEyeAnchor");

        if(Experiment2Main.ScenarioNo == 0)
        {
            enableControls = true;
        }


    }

    bool ThresholdPassed(DisplayFacialAction currentFacialAction)
    {

        if (FaceExpressions[currentFacialAction.ExpressionChosen] > currentFacialAction.Threshold)
        {
            if (DebugMode)
            {
                DebugText.text += currentFacialAction.ExpressionChosen + "\n";
            }
            currentFacialAction.gameObject.transform.Find("ActivationCube").gameObject.SetActive(true);
            return true;
        }
        currentFacialAction.gameObject.transform.Find("ActivationCube").gameObject.SetActive(false);
        return FaceExpressions[currentFacialAction.ExpressionChosen] > currentFacialAction.Threshold;

    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ToggleControl();
            Debug.Log("Controls Enabled" + enableControls.ToString());
        }

    }


    void FixedUpdate()
    {
        if (FaceExpressions.ValidExpressions == false)
        {
            if (give_warning)
            {
                Debug.LogWarning("Facial Expressions Not Valid");//This is put in place so that I can test without having to put on the headset, I get errors otherwise
                give_warning = false;
            }
            return;
        }
        else
        {
            give_warning = true;
                if (Experiment2Main.GetScenarioNo() == 0)
                {
                    UseButtons();
                }
                else
                {
                if (enableControls)
                {
                    UseFacialActions();
                }
                }
        }
    }

    void UseButtons()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || Input.GetKey(KeyCode.W)){
            fun_MoveForward(true);
        }
        else
        {
            fun_MoveForward(false);
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) || Input.GetKey(KeyCode.S))
        {
            fun_MoveBackwards(true);
        }else
        {
            fun_MoveBackwards(false);
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey(KeyCode.A))
        {
            fun_TurnLeft();
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || Input.GetKey(KeyCode.D))
        {
            fun_TurnRight();
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKey(KeyCode.Q))
        {
            fun_Shoot();
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || Input.GetKey(KeyCode.E))
        {
            fun_Interact(true);
        }else
        {
            fun_Interact(false);
        }
    }

    void UseFacialActions()
    {
        if (DebugMode)
        {
            DebugText.text = "";
        }
        if (FaceExpressions.ValidExpressions == false)
        {
            return;
        }

        if (ThresholdPassed(MoveForward))
        {
            fun_MoveForward(true);
        }
        else
        {
            fun_MoveForward(false);
        }
        if (ThresholdPassed(TurnRight))
        {
            fun_TurnRight();
        }
        if (ThresholdPassed(TurnLeft))
        {
            fun_TurnLeft();
        }
        if (ThresholdPassed(Interact))
        {
            fun_Interact(true);
        }
        else
        {
            fun_Interact(false);
        }
        if (ThresholdPassed(ShootBullet))
        {
            fun_Shoot();
        }
        if (ThresholdPassed(MoveBackwards))
        {
            fun_MoveBackwards(true);
        }else
        {
            fun_MoveBackwards(false);
        }
    }


    public bool getHoldingObjectThresholdPassed()
    {
        return HoldingObjectThresholdPassed;
    }

    public bool getActivatingObjectThresholdPassed()
    {
        return ActivatingObjectThresholdPassed;
    }

    public void fireBullet()
    {
        if (!bulletOnCooldown)
        {
                GameObject projectile = Instantiate(bullet);
                projectile.transform.position = Player.transform.Find("TrackingSpace/CenterEyeAnchor").position + (Player.transform.Find("TrackingSpace/CenterEyeAnchor").forward*3);
                projectile.transform.rotation = Player.transform.Find("TrackingSpace/CenterEyeAnchor").rotation;
                projectile.GetComponent<Rigidbody>().velocity = Player.transform.Find("TrackingSpace/CenterEyeAnchor").forward * 25f;
                bulletOnCooldown = true;
                Invoke("resetBulletCooldown", 1f);//Can only shoot every 1 second
        }
    }

    void resetBulletCooldown()
    {
        bulletOnCooldown = false;
    }

    void fun_MoveForward(bool Passed)
    {
        if(disableForwardMovement)
        {
            return;
        }
        if (Passed)
        {
            Player.GetComponent<Rigidbody>().AddForce(Player.transform.Find("TrackingSpace/CenterEyeAnchor").forward * movespeed*20f);
        }else
        {
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    void fun_MoveBackwards(bool Passed)
    {

        if (Passed)
        {
            Player.GetComponent<Rigidbody>().AddForce(-Player.transform.Find("TrackingSpace/CenterEyeAnchor").forward * movespeed*20f);
        }
        else
        {
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    void fun_TurnRight()
    {
        Player.transform.Rotate(transform.up, rotationSpeed);
    }
    void fun_TurnLeft()
    {
        Player.transform.Rotate(transform.up, -rotationSpeed);
    }
    void fun_Interact(bool Passed)
    {
        if (Passed)
        {
            HoldingObjectThresholdPassed = true;
            ActivatingObjectThresholdPassed = true;
        }
        else
        {
            HoldingObjectThresholdPassed = false;
            ActivatingObjectThresholdPassed = false;
        }
    }
    void fun_Shoot()
    {
        fireBullet();
    }

    public void SetDisableForwardMovement(bool value)
    {
        disableForwardMovement = value;
    }

    public void ToggleControl()
    {
        enableControls = !enableControls;
    }
}
