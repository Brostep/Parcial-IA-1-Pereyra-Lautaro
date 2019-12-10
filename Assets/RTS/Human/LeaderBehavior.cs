using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBehavior : Steering
{
	public float neighborhoodRadius = 10f;
	public float separationRadius = 2f;
	public float alignmentMult = 1f;
	public float cohesionMult = 1f;
	public float separationMult = 1f;

	public bool drawFlockingGizmos = false;
	public bool isAvoiding = false;

	Vector3 _alignment, _cohesion, _separation;

	override public void Start()
	{
		obstacle = Utility.LayerMaskToInt(obstacle);
		target = FindObjectOfType<Point>().transform;
	}

	void FixedUpdate()
	{
		ResetForces();

		var hits = Physics.OverlapSphere(transform.position, neighborhoodRadius);

		var sumV = Vector3.zero;            //Suma de velocidades
		var sumP = Vector3.zero;            //Suma de posiciones
		var sumSepForce = Vector3.zero;     //Suma de fuerzas de separaci?n (deltas / distancia)

		int nHits = 0;
		foreach (var hit in hits)
		{
			if (hit.gameObject == gameObject)
				continue;

			if (hit.gameObject.layer == obstacle)
			{
				var distance = hit.transform.position - transform.position;
				var distanceMag = distance.magnitude;
				if (distanceMag < obstacleRadius) // chequeo por distancias de obstaculos
					AddForce(Avoidance(distance, obstacleRadius));
			}
			else
			{
				var other = hit.GetComponent<Steering>();
				if (other == null)
					continue;

				var deltaP = transform.position - other.position; // de otros hacia mi
				var distSqr = deltaP.sqrMagnitude;
				if (distSqr > 0f && distSqr < separationRadius * separationRadius)
				{
					sumSepForce += deltaP / distSqr;
				}

				nHits++;
				sumV += other.velocity;
				sumP += other.position;
			}
		}

		if (nHits > 0)
		{
			_alignment = sumV.normalized * maxVelocity - velocity;      //Promedio de "direcciones"
			_cohesion = Seek(sumP / nHits);                             //Seguir promedio de posiciones
			_separation = sumSepForce == Vector3.zero ? Vector3.zero : sumSepForce.normalized * maxVelocity - velocity; //Prmoedio de fuerzas

			AddForce(_alignment * alignmentMult);
			AddForce(_cohesion * cohesionMult);
			AddForce(_separation * separationMult);
		}

		//Seek Leader
		AddForce(Seek(target.position));

		ApplyForces();


	}
}