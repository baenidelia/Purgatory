using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerContactTrigger : MonoBehaviour {
	
	//Basic area checker for our game, light stuff isnt mine
	//Used to determine when we can rotate the map and send analytics of all the rooms the player visited
    [SerializeField]
    private GameObject playerGameObject;
    public bool isPlayerInThisCube;
    private UIGenerator uIGenerator;

    private Light[] myLights;
    private List<MeshRenderer> myEmmitLights = new List<MeshRenderer>();

    private Color myLightColor;

    void Awake()
    {
        isPlayerInThisCube = false;
        myLights = transform.parent.GetComponentsInChildren<Light>();
        for (int i = 0; i < myLights.Length; i++)
        {
            myLights[i].enabled = false;
        }
        Transform[] allChildren = transform.parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.CompareTag("Light"))
            {
                myEmmitLights.Add(child.gameObject.GetComponent<MeshRenderer>());
            }
        }
    }
    private void Start()
    {

        uIGenerator = GameObject.Find("Hand1").GetComponent<UIGenerator>();
    }

    public void ChangeLightColor(Color color)
    {
        myLightColor = color;
        for (int i = 0; i < myLights.Length; i++)
        {
            myLights[i].color = color;
        }
        ChangeEmmitLightColor(color);
    }
    private void ChangeEmmitLightColor(Color color)
    {
        for (int i = 0; i < myEmmitLights.Count; i++)
        {
            myEmmitLights[i].material.SetColor("_EmissionColor", color);
        }
    }
    public Color ReturnRoomTypeColor()
    {
        return myLightColor;
    }
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.tag == "PlayerCollider" && !isPlayerInThisCube)
        {
            isPlayerInThisCube = true;
            for (int i = 0; i < myLights.Length; i++)
            {
                myLights[i].enabled = true;
            }
            uIGenerator.PlayerCubeChanged();
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.gameObject.tag == "PlayerCollider" && isPlayerInThisCube)
        {
            for (int i = 0; i < myLights.Length; i++)
            {
                myLights[i].enabled = false;
            }
            isPlayerInThisCube = false;
            uIGenerator.PlayerCubeChanged();
        }
    }
    
}
