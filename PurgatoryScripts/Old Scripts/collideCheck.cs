using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideCheck : MonoBehaviour {

	//Collision checker for a gamejam game
	//Tracks the entering of colliders to the "Goal" area and determines based on the count whether you win or not.
    private GameObject[] balls;
    List<Collider> compareBalls;
    public int ballCount;
    public int colliderCount;
    public bool DidWeWin;
    private GameObject[] colliderObjs;

	void Start () {

        Time.timeScale = 1.0f;
        DidWeWin = false;
        colliderObjs = GameObject.FindGameObjectsWithTag("Colliders");

        balls = GameObject.FindGameObjectsWithTag("Balls");

        List<SphereCollider> ballSphere = new List<SphereCollider>();

        foreach(GameObject ball in balls) 
        {
            SphereCollider ballColl = ball.GetComponent<SphereCollider>();
            if(ballColl == null)
            {
                var text = "null";
                Debug.Log(text);
            }
            ballSphere.Add(ballColl);
            Debug.Log(ballSphere.Count);
        }

        ballCount = ballSphere.Count;

        Debug.Log(ballCount);
	}
	

	void Update () {

        if (winCheck()) {
            Debug.Log("Wincheck returned true.");
            Time.timeScale = 0.0f;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        colliderCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        colliderCount--;
    }

   bool winCheck()
    {
        bool win = false;

        if(ballCount == colliderCount)
        {
            win = true;
            DidWeWin = true;
        }
        //Debug.Log(win);
        return win;
    }
    
}
