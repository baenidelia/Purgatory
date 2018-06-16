using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class rotationV2 : MonoBehaviour
{
	//Boatloads of variables
	#region Variables
	[HideInInspector]
	public delegate void rotationDelegate();
	[HideInInspector]
	public event rotationDelegate rotationEvent;

    private Hand handu;
    private Hand otherHand;

    private UIGenerator uiGenComponent;
    private Generator genComponent;
    private LevelManager levelManager;
    private MapCube mapCubeScript;

    private List<GameObject> pivotGameObjects;
    private List<GameObject> pivotGameObjectsUI;
    private GameObject mapGameObject;
    //private Vector3 centerPointVector3; DISABLED UNTIL USED FOR MIDDLE CHECK
    private GameObject parentPivot;
    private GameObject parentPivotUI;
    private GameObject centerPivotCube;
    private GameObject centerPivotUI;
    private GameObject hand1;

    public int rotationCount;

    [HideInInspector]
    public bool rotating;

    private bool isCenterPointUsed;
	#endregion
	void Start()
    {
		//Set all necessary variables, need to find a better way than GameObject.Find
        genComponent = GameObject.Find("LevelGenerator").GetComponent<Generator>();
        uiGenComponent = GameObject.Find("Hand1").GetComponent<UIGenerator>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        //centerPointVector3 = genComponent.midPointVector3; DISABLED UNTIL USED FOR MIDDLE CHECK
        isCenterPointUsed = false;

        mapGameObject = uiGenComponent.parentHolderUI.gameObject;
        pivotGameObjects = GameObject.FindGameObjectsWithTag("Pivot").ToList();
        handu = gameObject.GetComponent<Hand>();

        centerPivotCube = GameObject.FindGameObjectWithTag("CenterPivot");
        centerPivotUI = GameObject.FindGameObjectWithTag("CenterPivotUI");
        hand1 = GameObject.Find("Hand1");
        otherHand = hand1.GetComponent<Hand>();
        mapCubeScript = hand1.GetComponent<MapCube>();

    }

    void Update()
    {
        //Check if rotation uses the center pivot CURRENTLY DISABLED
        /*if (mapGameObject.activeInHierarchy)
        {
            CheckForMiddle(cubeListSelection, centerPointVector3);
        }*/
    }

    public void Rotation(List<GameObject> rotatingObjects, Vector3 direction, bool centerPivotUsed, float rotationAmount, bool rotationStopped)
    {
        //Find the pivot point that the objects spin around in. Uses direction vectors like Vector3.up to determine axis when called.
        #region xRotation
        if (Mathf.Abs(direction.x) > 0.0001)
        {
            if (!centerPivotUsed)
            {
                foreach (GameObject pivotPoint in pivotGameObjects)
                {
                    //Compare the first hit result to every X value in pivotpoints to find the matching one. All pivots have different values so there is no overlap or duplicate results.
                    //Except in the middle for 3x3x3 or 5x5x5 which is why we use centerPivotUsed bool as it has been determined beforehand.
                    //Currently not working 100% and center pivot is used in every rotation.
                    if (Math.Abs(pivotPoint.transform.position.x - rotatingObjects[0].transform.position.x) < 0.0001)
                    {
                        parentPivot = pivotPoint;
                        break;
                    }
                }
            }
            //Defaults to centerPivot if it was predetermined
            else
            {
                parentPivot = centerPivotCube;
            }
        }
        #endregion

        //Same as above, but for Y axis
        #region yRotation
        else if (Mathf.Abs(direction.y) > 0.0001)
        {
            if (!centerPivotUsed)
            {
                foreach (GameObject pivotPoint in pivotGameObjects)
                {
                    if (Mathf.Abs(pivotPoint.transform.position.y - rotatingObjects[0].transform.position.y) < 0.0001)
                    {
                        parentPivot = pivotPoint;
                        break;
                    }
                }
            }
            else
            {
                parentPivot = centerPivotCube;
            }
        }
        #endregion
        //Z axis
        #region zRotation
        else if (Mathf.Abs(direction.z) > 0.0001)
        {
            if (!centerPivotUsed)
            {
                foreach (GameObject pivotPoint in pivotGameObjects)
                {
                    if (Mathf.Abs(pivotPoint.transform.position.z - rotatingObjects[0].transform.position.z) < 0.0001)
                    {
                        parentPivot = pivotPoint;
                        break;
                    }
                }
            }
            else
            {
                parentPivot = centerPivotCube;
            }
        }
        #endregion
        else
        {
            Debug.Log("Rotation logic failed when picking axis");
        }

        foreach (GameObject rotaObject in rotatingObjects)
        {
            rotaObject.transform.SetParent(parentPivot.transform);
        }
        //Start the rotation for the big cube
        if (rotationStopped)
            StartCoroutine(RotateMe(parentPivot.transform, direction, rotationAmount, false));
        else RotateSmallStep(parentPivot.transform, direction, rotationAmount, false, rotationStopped);
    }

    //UI Rotation - Same thing as above but for UI
    public void RotationUI(List<GameObject> rotatingObjectsUI, Vector3 directionUI, bool centerPivotUsedUI, float rotationAmount, bool rotationStopped)
    {
        //Get the center object and UIpivots
        centerPivotUI = GameObject.FindGameObjectWithTag("CenterPivotUI");
        pivotGameObjectsUI = GameObject.FindGameObjectsWithTag("PivotUI").ToList();
        #region xRotation

        if (Mathf.Abs(directionUI.x) > 0.0001)
        {
            if (!centerPivotUsedUI)
            {
                foreach (GameObject pivotPoint in pivotGameObjectsUI)
                {
                    if (Math.Abs(pivotPoint.transform.position.x - rotatingObjectsUI[0].transform.position.x) < 0.0001)
                    {
                        parentPivotUI = pivotPoint;
                        break;
                    }
                }
            }
            else
            {
                parentPivotUI = centerPivotUI;
            }
        }
        #endregion
        #region yRotation
        else if (Mathf.Abs(directionUI.y) > 0.0001)
        {
            if (!centerPivotUsedUI)
            {
                foreach (GameObject pivotPoint in pivotGameObjectsUI)
                {
                    if (Math.Abs(pivotPoint.transform.position.y - rotatingObjectsUI[0].transform.position.y) < 0.0001)
                    {
                        parentPivotUI = pivotPoint;
                        break;
                    }
                }
            }
            else
            {
                parentPivotUI = centerPivotUI;
            }
        }
        #endregion
        #region zRotation
        else if (Mathf.Abs(directionUI.z) > 0.0001)
        {
            if (!centerPivotUsedUI)
            {
                foreach (GameObject pivotPoint in pivotGameObjectsUI)
                {
                    if (Math.Abs(pivotPoint.transform.position.z - rotatingObjectsUI[0].transform.position.z) < 0.0001)
                    {
                        parentPivotUI = pivotPoint;
                        break;
                    }
                }
            }
            else
            {
                parentPivotUI = centerPivotUI;
            }
        }
        #endregion

        //Childing for UI rotation
        foreach (GameObject rotaObject in rotatingObjectsUI)
        {
            rotaObject.transform.SetParent(parentPivotUI.transform);
        }

        //Start the coroutine, bool set to true to let the rotation coroutine know its the UI

        if (rotationStopped)
            StartCoroutine(RotateMe(parentPivotUI.transform, directionUI, rotationAmount, true));
        else RotateSmallStepUI(parentPivotUI.transform, directionUI, rotationAmount, true, rotationStopped);

    }

    #region Middle pivot check
    private void CheckForMiddle(List<GameObject> objects, Vector3 midVector3)
    {
        //Go through the objects to find if the middle pivot should be used for rotation (CURRENTLY DISABLED)
        foreach (GameObject someName in objects)
        {
            if (someName.transform.position == midVector3)
            {
                isCenterPointUsed = true;
            }
        }
    }
    #endregion

    #region Rotation cleanup (WIP)
    private void CleanUpRotation()
    {
		//Set the rotated objects back to their original parent objects

		if (rotationEvent != null)
		{
#if UNITY_STANDALONE
			if (AnalyticsTestingClass.analyticsResults.rotationCount == 0 && SceneManager.GetActiveScene().buildIndex == 3)
			{
				Analytics.CustomEvent("First rotation", new Dictionary<string, object>
				{
					{"Time", Time.timeSinceLevelLoad }
				});
			}
#endif
#if UNITY_EDITOR
			if (AnalyticsTestingClass.analyticsResults.rotationCount == 0 && SceneManager.GetActiveScene().buildIndex == 3)
			{
				Debug.Log("Time of first rotation " + Time.timeSinceLevelLoad);
			}
#endif
			rotationEvent();
		}

		for (int i = 0; parentPivot.transform.childCount > 0;)
        {
            if (parentPivot.transform.GetChild(i).name != "VRStuff" && parentPivot.transform.GetChild(i).tag != "TeleportArea")
                parentPivot.transform.GetChild(i).parent = genComponent.parentHolder.transform;
            else
                parentPivot.transform.GetChild(i).parent = null;
        }
        for (int i = 0; parentPivotUI.transform.childCount > 0;)
        {
            parentPivotUI.transform.GetChild(i).parent = uiGenComponent.parentHolderUI.transform;
        }

        //Reset pivot rotations so nothing breaks
        centerPivotUI.transform.localRotation = Quaternion.identity;
        centerPivotCube.transform.localRotation = Quaternion.identity;
        levelManager.ResetCubeLists();
        levelManager.RestartMiniMapMaterials();
        levelManager.RestartDoorSymbols(true, true);
        levelManager.CheckDoors();
        mapCubeScript.forceMapShowing(false);
        pivotGameObjects.Clear();
        pivotGameObjectsUI.Clear();
    }
    #endregion

	//Coroutine was not made by me

    #region Rotation coroutine
    IEnumerator RotateMe(Transform pivot, Vector3 axis, float amountAngle, bool isUI)
    {
        rotating = true;
        CloseAllDoors();
        //Start and end values for the rotation
        Quaternion from = pivot.localRotation;
        Quaternion to = pivot.localRotation;
        to *= Quaternion.Euler(axis * amountAngle);

        //The rotation itself
        float elapsed = 0.0f;
        while (elapsed < .5f)
        {
            levelManager.RestartDoorSymbols(false, true);
            handu.DoHapticFeedBack(500);
            otherHand.DoHapticFeedBack(500);
            pivot.localRotation = Quaternion.Slerp(from, to, elapsed / .5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Round the values so that they dont complicate further calculations
        pivot.localRotation = to;
        Quaternion roundRot = new Quaternion(Mathf.Round(pivot.localRotation.x), Mathf.Round(pivot.localRotation.y), Mathf.Round(pivot.localRotation.z), Mathf.Round(pivot.localRotation.w));
        pivot.localRotation = roundRot;

        rotating = false;
        //Does cleanup when this function is called for the UI since it spins after the map itself
        if (isUI)
            CleanUpRotation();
        yield return new WaitForSeconds(1);
    }
    #endregion

    #region Rotation Courutine small steps

    public void RotateSmallStep(Transform pivot, Vector3 axis, float angleAmount, bool isUI, bool ended)
    {

        //rotating = true;
        CloseAllDoors();
        levelManager.RestartDoorSymbols(false, true);
        //Start and end values for the rotation
        Quaternion from = pivot.localRotation;
        Quaternion to = pivot.localRotation;
        to *= Quaternion.Euler(axis * angleAmount);

        //The rotation itself
        handu.DoHapticFeedBack(500);
        otherHand.DoHapticFeedBack(500);
        pivot.localRotation = Quaternion.Slerp(from, to, 1);
        parentPivot.transform.localRotation = pivot.localRotation;

    }
    public void RotateSmallStepUI(Transform pivot, Vector3 axis, float angleAmount, bool isUI, bool ended)
    {

        //rotating = true;
        CloseAllDoors();
        //Start and end values for the rotation
        Quaternion from = pivot.localRotation;
        Quaternion to = pivot.localRotation;
        to *= Quaternion.Euler(axis * angleAmount);

        //The rotation itself
        handu.DoHapticFeedBack(500);
        otherHand.DoHapticFeedBack(500);
        pivot.localRotation = Quaternion.Slerp(from, to, 1);
        parentPivotUI.transform.localRotation = pivot.localRotation;

    }

    #endregion
    private void CloseAllDoors()
    {
        GameObject[] openDoors = GameObject.FindGameObjectsWithTag("DoorOpen");
        for (int i = 0; i < openDoors.Length; i++)
        {
            openDoors[i].gameObject.GetComponent<Door>().CloseDoor();
        }
    }
}