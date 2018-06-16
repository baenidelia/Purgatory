using UnityEngine;
using System.Collections;

public class RaycastShoot : MonoBehaviour
{

    /*[HideInInspector]*/ public int gunDamage = 1;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    [HideInInspector] public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    [HideInInspector] public float weaponRange = 50f;                                     // Distance in Unity units over which the player can fire
    [HideInInspector] public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;
    [HideInInspector] public LineRenderer laserLine;                                     // Holds a reference to the gun end object, marking the muzzle location of the gun
    MuzzleFlash muzzleFlash;
    private Ray asd;                                          // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    //private AudioSource gunAudio;                                       // Reference to the audio source which will play our shooting sound effect                                   // Reference to the LineRenderer component which will display our laserline

    public void Initialize()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        muzzleFlash = GetComponent<MuzzleFlash>();

        // Get and store a reference to our AudioSource component
        //gunAudio = GetComponent<AudioSource>();

    }

    public void Fire()
    {
		if (Time.timeScale > 0) {
			
			// Check if the player has pressed the fire button and if enough time has elapsed since they last fired
			// Start our ShotEffect coroutine to turn our laser line on and off

			StartCoroutine (ShotEffect ());

			// Set the start position for our visual effect for our laser to the position of gunEnd
			laserLine.SetPosition (0, gunEnd.position);

			//Debug.Log (laserLine.GetPosition (0));

			asd.direction = gunEnd.forward;
			asd.origin = gunEnd.position;

			RaycastHit[] hits;
			int layer = LayerMask.NameToLayer ("Shootable");
			hits = Physics.RaycastAll (asd, weaponRange, layer);
			laserLine.SetPosition (1, asd.origin + asd.direction * weaponRange);

			for (int i = 1; i < hits.Length; i++) {
				RaycastHit hit = hits [i];

				Vector3 direction = hit.rigidbody.transform.position - GameObject.FindGameObjectWithTag ("Player").transform.position;
				hit.rigidbody.AddForce (direction * hitForce);
				hit.rigidbody.gameObject.GetComponent<ZombieHealth> ().TakeDamage (gunDamage);
			}
		}
    }
    private IEnumerator ShotEffect()
    {

        //Turn on our line renderer
        laserLine.enabled = true;

        muzzleFlash.Activate();
        //Wait for .07 seconds
        yield return shotDuration;

        //Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }

    public void UpdateStatsLaser(int damage)
    {
        gunDamage = damage;
    }
}