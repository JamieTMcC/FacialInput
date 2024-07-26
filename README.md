
  

# Facial Input Experiment Part 2 Breakdown

  

## Intro

  

This experiment is build to test both the ability of the quest pro to actively monitor intensities of various Facial Action Units(AUs) for input and test peoples' capacity to use facial expressions as a form of input.

  

## Wee Note

You will probably have to generate the lighting for the scene when you first load it. It is bland in the editor, but garish in the headset if you don't.

  

### Program Lifecycle

  

The basic steps for the program are as follows:

  

* Program boots and upon awakening randomises scenario order for 0-2

* Program populates necessary variables with objects from the scene

* The Logging procedure in the GameManager Starts, the occurance of log is also how often the program checks for a finished state

* For any scenario 0-2, the user must first calibrate there intensities for each of the in-game actions, e.g. walk forward, interact

* For scenario 3, they must first chose an AU to be paired with an in-game action, then calibrate their intensities

* Users then move blocks, shoot targets and press buttons until a wall moves then must navigate a short course to reach the green cube

* After this users are moved to the next trial, keeping the intensities (And AUs) from the previous trial, we have multiple trials for each scenario

* Static variables are reset and the scene is reloaded, not necessarily in that order

* After all trials are complete we move to the next scenario

* After all scenarios are complete, the program will close itself

  

## Important Scripts

  

### GameManager

  

GameManager handles the logging and transition between scenes. It also checks to see if the player is colliding with the end goal to prompt the transition between scenes. **This is what you want to look at if you want to edit the number of scenes or the logging algorithm.**

  

### Experiment2Main

  

This keeps track of the tasks the user has achieved up until the labyrinth. It also houses alot of data samples which we use to populate the DisplayFacialAction objects, which house chosen AUs and thresholds. A string is present in this program which means that any AU featured will be able to retrieve a corresponding intensity value, an average of many participants from experiment 1. **This is what you want to look at if you want to edit which AUs are used for each scenario.**

  

```c#

\\Scenario  1

ScenarioFaceExpressions[0, 0] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[0, 1] = OVRFaceExpressions.FaceExpression.LidTightenerR;

ScenarioFaceExpressions[0, 2] = OVRFaceExpressions.FaceExpression.LidTightenerL;

ScenarioFaceExpressions[0, 3] = OVRFaceExpressions.FaceExpression.JawSidewaysLeft;

ScenarioFaceExpressions[0, 4] = OVRFaceExpressions.FaceExpression.LipSuckLB;

ScenarioFaceExpressions[0, 5] = OVRFaceExpressions.FaceExpression.ChinRaiserB;

\\ Scenario 2

ScenarioFaceExpressions[1, 0] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[1, 1] = OVRFaceExpressions.FaceExpression.MouthRight;

ScenarioFaceExpressions[1, 2] = OVRFaceExpressions.FaceExpression.MouthLeft;

ScenarioFaceExpressions[1, 3] = OVRFaceExpressions.FaceExpression.LidTightenerR;

ScenarioFaceExpressions[1, 4] = OVRFaceExpressions.FaceExpression.CheekPuffL;

ScenarioFaceExpressions[1, 5] = OVRFaceExpressions.FaceExpression.LipSuckLB;

\\ Scenario 3, but people choose their own AUs here

ScenarioFaceExpressions[2, 0] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[2, 1] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[2, 2] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[2, 3] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[2, 4] = OVRFaceExpressions.FaceExpression.JawDrop;

ScenarioFaceExpressions[2, 5] = OVRFaceExpressions.FaceExpression.JawDrop;

```

  

### InterpretFacialActions

  

This is what handles user input throughout the program. The user starts off without any interaction in all but the control scenario. They must first either activate the AU movement or bring up the UI to alter the thresholds or AUs which is done via the controller as displayed above the Facial Models.

  

*This could be doing with a rework to make it more robust against different numbers of in-game actions wanted.*

  

**This is where you want to look to change how functions work, or at least where to start to look and follow the trial to which functions use the flags it triggers.**

  

*Small note: there exists a bug which doesn't really impact the program, but when the user is walking through the blocks which disable forward movement they gain a speed boost while traveling backwards? :|*

  

### PathGenerator

  

Very small file, this one simply generates a unique path everytime the program is ran. PersistentDataPath is used so it will go to a stable location no matter what platform it is ran on, it appends a time stamp to the directory so that no file is every written over.

  

**Google Application.PersistentDataPath to find out where abouts the log files will end up depending on used OS**

  

### CanvasScript

  

Quite a wordy script here. Mostly populates and manipulates the DisplayFacialAction script for a given in-game action. Shouldn't need to edit this really. Small note, the CanvasScript updates the DisplayFacialAction in real time, the user doesn't have to press accept for the changes to hold when the menu is closed. Buttons on the canvas control alot of the methods within this class.

  

**Used this Script to edit the available AUs in the dropdown menu for scenario 3 - usableexpressionlist**

  

```c#

usableexpressionlist = new List<OVRFaceExpressions.FaceExpression> { OVRFaceExpressions.FaceExpression.JawDrop, OVRFaceExpressions.FaceExpression.LidTightenerL...

```

  

### DisplayFacialAction

  

A very Important script attached to the facial model objects which are in the Scenarios 1-3. These display the action unit to the user but also house the data corresponding to the in-game action from the AU point of view. Fairly simple, but crucial that it doesn't mess up.

#### ShapeFace

This method is crazy. skinnedMeshRenderer is the name of the head model used to showcase the AU, however this doesn't inherently align with the OVRFaceExpressions.FaceExpression enum. This means we need to account for this when feeding it a AU. This method solves it (I think) but is very dodgy. It is also used by the CanvasScript to showcase the model face there.

  

### EveryOtherScript

  

Every other script is either boilerplate or unused. I'm not going to count my chicks yet, but these scripts should be sound, although some have static variables so be aware of that when transitioning between trials and scenarios.

### Study 1 Procedure
Study 1 involved asking participants to emulate the face of the model with a variety of transformations applied which map to the AUs trackable by the quest pro. The process for capturing face data was as follows:
* Participants are given an intro to the experiment explaining what is required of them
* During this process participants are tasked with calibrating natural face intensities the exact wording for this is *"Please make a neutral face and when ready press A to lock in a natural state"*, upon pressing A every AU intensity is logged when participants are making no expression.
* AUs were done in a random order, however, left and right components of the same AUs where done together to help participants identify what they need to do quicker, e.g. CornerPullerL and CornerPullerR, or MouthLeft and MouthRight
* For each AU
	* Participants are given a practice period where they can observe and attempt to make each expression, they press A to continue to trial
	* While in trial
		*  the participants are given a short countdown
		* They are then prompted to hold the chosen AU for a period of 3 seconds
		* They are then told to release
	* After the trial they are given an in-headset questionaire for each AU ranking, Borg RPE, Discomfort, and their performance
* After the experiment in the headset is concluded they are given an open-ended questionaire
##### Need Support

E-mail: JamieThomasMcCready@gmail.com
Github: https://github.com/JamieTMcC/FacialInput
