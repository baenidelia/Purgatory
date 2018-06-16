using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Valve.VR.InteractionSystem;

public class AnalyticsTestingClass : MonoBehaviour
{
	//Get a static copy of this class to make it into a Singleton
	public static AnalyticsTestingClass analyticsResults;

	//Set variables that we track
	public int deathCount;
	public int rotationCount;
	public int teleportCount;
	public int doorOpenCount;
	public int roomVisitCount;
	public float levelStartTime;

	public int laserKills;
	public int blenderKills;
	public int turretKills;
	public int laserBlenderKills;

	//Currently disabled until reworked
	//			vvvvv
	//[Header("NONE OF THESE CAN BE EMPTY!")]
	//public GameObject teleportReference;
	//public GameObject rotationReference;
	//public GameObject onDeathReference;
	////public GameObject doorReference;


	// Get script references
	private Teleport teleportScript;
	private rotationV2 rotationScript;
	private Player onDeathScript;
	//private Door doorScript;

	//Make a list for the uiCubes to use the count in calculating visited rooms.
	private List<GameObject> uiCubeList;

	//Here we create the singleton that holds all the information for us to use.
	void Awake()
	{
		if (analyticsResults == null)
			analyticsResults = this;
		else if (analyticsResults != this)
			Destroy(gameObject);
		DontDestroyOnLoad(analyticsResults);
	}

/*	Currently obsolete until reworked/restructured.
 *	void Start()
	{
		uiCubeList = GameObject.Find("Hand1").GetComponent<UIGenerator>().comparisonGameObjectsGeneratorUI;
		onDeathScript = GameObject.Find("Player").GetComponent<Player>();
		rotationScript = GameObject.Find("Hand2").GetComponent<rotationV2>();
		teleportScript = GameObject.Find("Teleporting").GetComponent<Teleport>();
		doorScript = FindObjectOfType<Door>();

		if (doorScript == null)
			Debug.LogWarning("Door script is missing reference! Go to " + gameObject.name + " and set the object the script is in");
		else if (onDeathScript == null)
			Debug.LogWarning("onDeath is missing reference! Go to " + gameObject.name + " and set the object the script is in");
		else if (rotationScript == null)
			Debug.LogWarning("Rotation script is missing reference! Go to " + gameObject.name + " and set the object the script is in");
		else if (teleportScript == null)
			Debug.LogWarning("Teleport script is missing reference! Go to " + gameObject.name + " and set the object the script is in");
		else if (teleportScript || rotationScript || onDeathScript || doorScript != null)
		{
            
			teleportScript.teleportEvent += addTeleportation;
			rotationScript.rotationEvent += addRotation;
			onDeathScript.deathEvent += addDeath;
			doorScript.doorOpenEvent += addDoorOpen;
            
		}
		else
		{
			print("This should practically never happen :)");
		}
	}*/
    public void FindGameObjects()
    {
		//Get a copy of each script that has our analyticsevents in them
        uiCubeList = GameObject.Find("Hand1").GetComponent<UIGenerator>().comparisonGameObjectsGeneratorUI;
        onDeathScript = GameObject.Find("Player").GetComponent<Player>();
        rotationScript = GameObject.Find("Hand2").GetComponent<rotationV2>();
        teleportScript = GameObject.Find("Teleporting").GetComponent<Teleport>();
        //doorScript = FindObjectOfType<Door>();

		//Null checks just in case, could be done better, will be looked at in the future
        if (onDeathScript == null)
            Debug.LogWarning("onDeath is missing reference! Go to " + gameObject.name + " and set the object the script is in");
        else if (rotationScript == null)
            Debug.LogWarning("Rotation script is missing reference! Go to " + gameObject.name + " and set the object the script is in");
        else if (teleportScript == null)
            Debug.LogWarning("Teleport script is missing reference! Go to " + gameObject.name + " and set the object the script is in");
        else if (teleportScript || rotationScript || onDeathScript /*|| doorScript != null*/)
        {
            teleportScript.teleportEvent += addTeleportation;
            rotationScript.rotationEvent += addRotation;
            onDeathScript.deathEvent += addDeath;
        //    doorScript.doorOpenEvent += addDoorOpen;
        }
        else
        {
			//Hasn't happened yet...
            print("This should practically never happen :)");
        }
    }
	
	//And a boatload of functions for all the events we are calling in other parts of the code.
	//These functions get called in other codes and register that they have happened in here.
	//Quite basic stuff, but effective.
	private void addDeath()
	{
		deathCount = deathCount + 1;
	}

	private void addRotation()
	{
		rotationCount = rotationCount + 1;
		uiCubeList = GameObject.Find("Hand1").GetComponent<UIGenerator>().comparisonGameObjectsGeneratorUI;
	}

	private void addTeleportation()
	{
		teleportCount = teleportCount + 1;
	}

	public void addDoorOpen()
	{
		doorOpenCount = doorOpenCount + 1;
	}

	public void roomsVisited()
	{
		if (uiCubeList != null)
		{
			//Go through each room to find out if they have been discovered
			foreach (GameObject uiCube in uiCubeList)
			{
				if (uiCube.GetComponent<MiniMapMaterialChange>().discovered)
				{
					roomVisitCount++;
				}
			}
		}
		else
		{
			Debug.Log("UICubelist is null");
		}
	}

	//Not how I would like to do this, string checks are just unreliable at best, should also add nullchecks to them
	//Sends a customEvent "DeathCause" to Unity Analytics with the object that killed the player and the level they were in
	public void deathAnalytic(string deathObject, string levelName)
	{
			Analytics.CustomEvent("DeathCause", new Dictionary<string, object>
			{
				{deathObject, levelName}
			});
			Debug.Log("Death caused by " + deathObject + " in level " + levelName);

		switch (deathObject)
		{
			case "laser":
				laserKills++;
				break;
			case "turret":
				turretKills++;
				break;
			case "blender":
				blenderKills++;
				break;
			case "laserblender":
				laserBlenderKills++;
				break;
				default:
				Debug.Log("Deathanalytics could not recognize the object that killed the player");
					break;
		}
	}

	//Logic based checker for the deadliest trap that gets printed to the panel at the end of the level
	public int getDeadliestTrap()
	{
		if (laserKills > blenderKills)
			return 1;
		if (turretKills > blenderKills)
			return 2;
		return laserBlenderKills > blenderKills ? 3 : 0;
	}

	//Resets everything to do with analytics and unsubscribes events
	public void resetEvents()
	{
		teleportScript.teleportEvent -= addTeleportation;
		rotationScript.rotationEvent -= addRotation;
		onDeathScript.deathEvent -= addDeath;
	//	doorScript.doorOpenEvent -= addDoorOpen;
		deathCount = 0;
		rotationCount = 0;
		teleportCount = 0;
		doorOpenCount = 0;
		roomVisitCount = 0;
		laserKills = 0;
		turretKills = 0;
		blenderKills = 0;
        laserBlenderKills = 0;
	}

	//Just in case resetEvents doesnt get called events get unsubbed and everything reset
	public void onDestroy()
	{
		teleportScript.teleportEvent -= addTeleportation;
		rotationScript.rotationEvent -= addRotation;
		onDeathScript.deathEvent -= addDeath;
	//	doorScript.doorOpenEvent -= addDoorOpen;
		deathCount = 0;
		rotationCount = 0;
		teleportCount = 0;
		doorOpenCount = 0;
		roomVisitCount = 0;
		laserKills = 0;
		turretKills = 0;
		blenderKills = 0;
        laserBlenderKills = 0;
    }
	//When the player dies, everything doesnt get reset, but events get unsubscribed since they are set on each load.
    public void levelRestart()
    {
        teleportScript.teleportEvent -= addTeleportation;
        rotationScript.rotationEvent -= addRotation;
        onDeathScript.deathEvent -= addDeath;
        //	doorScript.doorOpenEvent -= addDoorOpen;
        //rotationCount = 0;
        teleportCount = 0;
        doorOpenCount = 0;
        roomVisitCount = 0;
    }

	//Gives a very rough idea of how long the player has played our game for
	//NOTE TO SELF ---------------------------------------------------------
	//could use formatting, realtimesincestartup is in seconds 
	void OnApplicationQuit()
	{
		if(!Application.isEditor)
		Analytics.CustomEvent("Total playtime", new Dictionary<string, object>
		{
			{"Time", Time.realtimeSinceStartup }
		});
	}
}