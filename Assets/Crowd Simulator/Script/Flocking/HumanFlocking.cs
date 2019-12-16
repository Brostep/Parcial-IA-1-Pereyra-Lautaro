using UnityEngine;
using System.Collections;

public class HumanFlocking : Steering
{
	public float neighborhoodRadius = 10f;
	public float separationRadius = 2f;
	public float alignmentMult = 1f;
	public float cohesionMult = 1f;
	public float separationMult = 1f;

	public bool drawFlockingGizmos = false;
	public bool isAvoiding = false;

	Vector3 _alignment, _cohesion, _separation;

	HumanBehaviour h;

	override public void Start()
	{
		obstacle = Utility.LayerMaskToInt(obstacle);
		h = GetComponent<HumanBehaviour>();
	}

	void FixedUpdate()
	{
		var hits = Physics.OverlapSphere(transform.position, neighborhoodRadius);

		foreach (var hit in hits)
		{
			if (hit.gameObject == gameObject)
				continue;

			if (hit.gameObject.layer == obstacle)
			{
				var distance = hit.transform.position - transform.position;
				AddForce(Avoidance(distance));
					
			}
		}
		if (h.walking && !isAvoiding)
			AddForce(WanderWithStateTimed(wanderDistanceAhead, wanderRandomRadius, wanderRandomStrength));
		else
			AddForce(Flee(h.target));

		ApplyForces();

    }
}