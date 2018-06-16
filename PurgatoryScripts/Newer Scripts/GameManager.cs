
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	//Variables
    public static GameManager instance = null;
    private int levelCount;
    private LevelManager levelManager;

	private void Awake()
    {
		//Make this object in to a singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
		levelCount = SceneManager.sceneCountInBuildSettings;
        //Write a function to initialize the "Menu" scene
    }

	// Separate function for resetting the level and dying, calls the levelRestart function which doesnt
	// reset all statistics
    public void reloadLevel()
    {
		AnalyticsTestingClass.analyticsResults.levelRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	// Not sure if this one need resetEvents, but just in case
    public void exitGame()
    {
        Application.Quit();
        AnalyticsTestingClass.analyticsResults.resetEvents();
    }

	// used in the mainmenu to load a specific level and again reset events right after
    public void loadScene(int levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
        AnalyticsTestingClass.analyticsResults.resetEvents();
    }

	// If the buildIndex +1 equals level count, we have reached the end of the game so hubLevel gets loaded
	// If that is not the case load next level in the buildindex and reset events
    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 == levelCount)
            loadHubLevel();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AnalyticsTestingClass.analyticsResults.resetEvents();
    }

	//Gets the current scene name
    public string currentGameLevel()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void loadHubLevel()
    {
	    //Make the hublevel index0 in the build. ALWAYS
		//Loading the hublevel is quitting the level so events get reset.
		SceneManager.LoadScene(0); 
        AnalyticsTestingClass.analyticsResults.resetEvents();
    }
}
