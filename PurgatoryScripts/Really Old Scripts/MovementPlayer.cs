using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour {

    public float speed = 6f;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigid;
    float camRayLength = 100f;
    public GameObject character;
    // Dash Distance
	//float distance = 5f;

    // Use this for initialization
    void Awake () {
        anim = GetComponent < Animator > ();
        playerRigid = GetComponent<Rigidbody>();
	}

    void Update()
    {
		/* Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            character.transform.position += character.transform.forward * distance;
        }*/
    }

    // Update is called once per frame
    void FixedUpdate () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turning();
        Animating(h, v);
	}

    void Move (float h, float v)
    {
        movement.Set(h, 0f, v);

        movement = movement.normalized * speed * Time.deltaTime;

        playerRigid.MovePosition(transform.position + movement);
    } 

    void Turning()
    {
        //raycast piste
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        //jos osuu jonnekkin floor layerissa
        if(Physics.Raycast(camRay, out floorHit, camRayLength))
        {
			Vector3 targetPos = floorHit.point;
			targetPos.y = (float)(transform.position.y + 1.3);
			//targetPos.z -= 1;

			Vector3 spawnPos = new Vector3 ((float)(transform.position.x), (float)(transform.position.y + 1.3), (float)(transform.position.z));

			Vector3 playerToMouse = targetPos - spawnPos;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigid.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
	{
		//jos on inputtia johonkin suuntaan pelaaja juoksee
		bool walking = h != 0f || v != 0f;
		anim.SetBool ("IsWalking", walking);
	}
}