using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

// Adding necessary dependancies
[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(AssigmentManager))]

public class LevelManager : MonoBehaviour
{
	//Variables
    private UIGenerator uiGenComponent;
    private Generator genComponent;

    [Header("Cube lists")]
    private List<GameObject> uiGameObjects;
    private List<GameObject> bigCubeGameObjects;
    [Space]

    [Header("Cube parents")]
    private GameObject uiCubeParent;
    private GameObject bigCubeParent;
    [Space]

    private Door[] doorGameObjects;
    private RoomTypeindicator[] roomTypeindicators;
    public AnalyticsTestingClass levelAnalytics;

	private LevelExitCollider exitCollider;

    void Start()
    {
		//Setting of above variables
	    exitCollider = FindObjectOfType<LevelExitCollider>();
        genComponent = GameObject.Find("LevelGenerator").GetComponent<Generator>();
        uiGenComponent = GameObject.Find("Hand1").GetComponent<UIGenerator>();
        uiGameObjects = uiGenComponent.comparisonGameObjectsGeneratorUI;
        bigCubeGameObjects = genComponent.comparisonGameObjectsGenerator;

        uiCubeParent = uiGenComponent.parentHolderUI;
        bigCubeParent = genComponent.parentHolder;
        doorGameObjects = FindObjectsOfType<Door>();
        roomTypeindicators = FindObjectsOfType<RoomTypeindicator>();

        levelAnalytics = FindObjectOfType<AnalyticsTestingClass>();
        levelAnalytics = AnalyticsTestingClass.analyticsResults;

		//Find the objects that contain the scripts for analytics to work, then call levelstart to send analytics for funnel
        levelAnalytics.FindGameObjects();
        RestartDoorSymbols(true, true);
        CheckDoors();
        LevelStart();
    }

    void Update()
    {
    }
    // Boatload of resets for necessary components in the game
    #region Resets and Returns

    public void ResetCubeLists()
    {
        //Clear lists and find them again by searching each parent
        uiGameObjects.Clear();
        bigCubeGameObjects.Clear();
        for (int i = 0; i < uiCubeParent.transform.childCount; i++)
        {
            if (!uiCubeParent.transform.GetChild(i).transform.name.StartsWith("Pivot") && uiCubeParent.transform.GetChild(i).transform.tag != "MiniMapLights")
                uiGameObjects.Add(uiCubeParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < bigCubeParent.transform.childCount; i++)
        {
            if (!bigCubeParent.transform.GetChild(i).transform.name.StartsWith("Pivot"))
                bigCubeGameObjects.Add(bigCubeParent.transform.GetChild(i).gameObject);
        }

        uiGenComponent.comparisonGameObjectsGeneratorUI = uiGameObjects;
        genComponent.comparisonGameObjectsGenerator = bigCubeGameObjects;
    }

    public void RestartMiniMapMaterials()
    {
        foreach (Transform child in uiCubeParent.transform)
        {
            if (child.transform.name.StartsWith("UI"))
                child.gameObject.GetComponent<MiniMapMaterialChange>().highLightCube(false);
        }
    }
    public void CheckDoors()
    {
        for (int i = 0; i < roomTypeindicators.Length; i++)
        {
            roomTypeindicators[i].CheckMyDoors();
        }
    }
    public void RestartDoorSymbols(bool checkNextDoor, bool checkDoorSymbols)
    {
        for (int i = 0; i < doorGameObjects.Length; i++)
        {
            if (checkNextDoor)
                doorGameObjects[i].CheckIfNextDoorisLocked();
            if (checkDoorSymbols)
                doorGameObjects[i].CheckDoorSymbols();
        }

        //for (int i = 0; i < roomtypeIndicators.Length; i++)
        //{
        //    if (checkDoorSymbols)
        //        roomtypeIndicators[i].CheckDoorSymbols();
        //    roomtypeIndicators[i].ChangeIsDoorLocked(checkNextDoor);
        //}
    }

    public List<GameObject> ReturnUIGameObjects()
    {
        return uiGameObjects;
    }

    public List<GameObject> ReturnBigCubeGameObejcts()
    {
        return bigCubeGameObjects;
    }

    #endregion

	// Separating what gets sent/called depending on whether we are running the game from editor or no.
	// No cleaner solution came to mind, but it works like a charm
    public void LevelStart()
    {
#if UNITY_STANDALONE
		    levelAnalytics.levelStartTime = Time.realtimeSinceStartup;
		    AnalyticsEvent.LevelStart(SceneManager.GetActiveScene().name);
#endif
#if UNITY_EDITOR
		Debug.Log("Event sent (LevelStart)");
#endif
	}


	//At the end of the level, when the player touches the collider of the levelExit object, gather all data from AnalyticsTestingClass singleton
	//format it and send it as a dictionary to Unity analytics. Could use some formatting so that it's easier to read in unitys end
    public void EndLevel()
    {
		//Some sort of gamemanager? Which level to load in which instance*/
#if UNITY_STANDALONE
	    if (exitCollider != null && exitCollider.playerHasEntered || SceneManager.GetActiveScene().buildIndex == 0)
	    {
		    levelAnalytics.roomsVisited();
		    Dictionary<string, object> data = new Dictionary<string, object>
		    {
			    {"player_Deaths", levelAnalytics.deathCount},
			    {"rotation_Count", levelAnalytics.rotationCount},
			    {"doors_Opened", levelAnalytics.doorOpenCount},
			    {"teleport_Count", levelAnalytics.teleportCount},
			    {"rooms_Visited", levelAnalytics.roomVisitCount},
			    {"Level time", Mathf.Round((Time.realtimeSinceStartup - levelAnalytics.levelStartTime) * 10) / 10}
		    };
		    AnalyticsEvent.LevelComplete(GameManager.instance.currentGameLevel(), data);
	    }
#endif
#if UNITY_EDITOR
	    Debug.Log("Event sent | " + levelAnalytics.deathCount + " Deaths | " + levelAnalytics.doorOpenCount + " doorOpens | "
			+ levelAnalytics.roomVisitCount + " Rooms visited | " + levelAnalytics.rotationCount + " rotations | " + levelAnalytics.teleportCount + " teleportCount |" + Mathf.Round((Time.realtimeSinceStartup - levelAnalytics.levelStartTime) * 10) / 10 + " LEVELTIME");
#endif
		//GameManager.instance.nextLevel();
    }

    public void LevelQuit()
    {
#if UNITY_STANDALONE
	    if (exitCollider != null && exitCollider.playerHasEntered)
	    {
		    levelAnalytics.roomsVisited();
		    Dictionary<string, object> data = new Dictionary<string, object>
		    {
			    {"player_Deaths", levelAnalytics.deathCount},
			    {"rotation_Count", levelAnalytics.rotationCount},
			    {"doors_Opened", levelAnalytics.doorOpenCount},
			    {"teleport_Count", levelAnalytics.teleportCount},
			    {"rooms_Visited", levelAnalytics.roomVisitCount},
			    {"Level time", Mathf.Round((Time.realtimeSinceStartup - levelAnalytics.levelStartTime) * 10) / 10}
		    };
		    AnalyticsEvent.LevelQuit(GameManager.instance.currentGameLevel(), data);
	    }
#endif
#if UNITY_EDITOR
	    Debug.Log("Event sent | " + levelAnalytics.deathCount + " Deaths | " + levelAnalytics.doorOpenCount + " doorOpens | "
	              + levelAnalytics.roomVisitCount + " Rooms visited | " + levelAnalytics.rotationCount + " rotations | " + levelAnalytics.teleportCount + " teleportCount |");
#endif
		GameManager.instance.loadHubLevel();
    }

	// 3 functions for different calls to GameManager to manage the level start and end
    public void StartLevel(int index)
    {
        GameManager.instance.loadScene(index);
    }

    public void RestartLevel()
    {
        GameManager.instance.reloadLevel();
    }

	public void nextLevel()
	{
		GameManager.instance.nextLevel();
	}
}
