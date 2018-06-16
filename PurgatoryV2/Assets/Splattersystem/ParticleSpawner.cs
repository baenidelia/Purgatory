using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{

	public ParticleSystem particleLauncher;
	public ParticleSystem splatterParticles;
	private List<ParticleCollisionEvent> collisionEvents;
	public ParticleDecalPool splatDecalPool;
	public Gradient particleColorGradient;

	public List<Material> bloodMaterials;

	void Start()
	{
		bloodMaterials = new List<Material>();
		collisionEvents = new List<ParticleCollisionEvent>();
	}

	void OnParticleCollision(GameObject other)
	{
		ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

		for (int i = 0; i < collisionEvents.Count; i++)
		{
			splatDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
			EmitAtLocation(collisionEvents[i]);
		}
	}

	void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
	{
		splatterParticles.transform.position = particleCollisionEvent.intersection;
		splatterParticles.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
		ParticleSystem.MainModule psMain = splatterParticles.main;
		psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));

		splatterParticles.Emit(1);  
	}

	void Update()
	{
		if (Input.GetButton("Fire1"))
		{
			ParticleSystem.MainModule psMain = particleLauncher.main;
			psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
			particleLauncher.Emit(1);
		}
	}

	public void spillBlood(Collision collision)
	{
		ParticleSystem.MainModule psMain = particleLauncher.main;
		//ParticleSystem.VelocityOverLifetimeModule psVelo = particleLauncher.velocityOverLifetime;
		//float velocityX = psVelo.x.constant;
		//float velocityY = psVelo.y.constant;
		//float velocityZ = psVelo.z.constant;
		//Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);

		//float sizeConstant = psMain.startSize.constant;
		//sizeConstant = collision.relativeVelocity.magnitude + sizeConstant;
		//Debug.Log("Magnitude of hit " + collision.relativeVelocity.magnitude);
		//psMain.startSize = sizeConstant;

		psMain.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
		particleLauncher.Emit(1);
	}
}