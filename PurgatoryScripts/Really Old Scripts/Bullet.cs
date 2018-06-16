using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	int damage = 0;

	void Start() {
		damage = GameObject.Find("archtronic").GetComponent<ProjectileShoot>().projDamage;
		Destroy (this.gameObject, 2f);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Zombie") {

			col.gameObject.GetComponent<ZombieHealth> ().TakeDamage (damage);

			// knockback enemy when hit
			Vector3 direction = col.gameObject.transform.position - GameObject.FindGameObjectWithTag ("Player").transform.position;
			col.GetComponent<Rigidbody>().AddForce(direction * 50f);

			Destroy (this.gameObject);
		}
	}
}
