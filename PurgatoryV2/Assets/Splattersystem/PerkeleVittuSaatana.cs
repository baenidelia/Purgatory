using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkeleVittuSaatana : MonoBehaviour
{
	private Transform moverTransform;
	public float timeBeforeTurn;
	public float currentTime;

	// Use this for initialization
	void Start ()
	{
		moverTransform = gameObject.transform;
		currentTime = Time.time;
		timeBeforeTurn = 0.5f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.time - currentTime < timeBeforeTurn)
		{
			moverTransform.Translate(Vector3.forward * Time.deltaTime * 10);
		}
		else 
		{
			moverTransform.Rotate(0,90,0);
			currentTime = Time.time;
		}
	}
}
