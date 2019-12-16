using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Steering : MonoBehaviour, ISteerable
{
    public Transform target;
    public float velocityLimit = 5f;
    public float forceLimit = 10f;

    public float arrivalRadius = 5f;
    public float pursuitPeriod = 2f;

    public float wanderDistanceAhead = 5f;
    public float wanderRandomRadius = 5f;
    public float wanderPeriod;
    public float wanderRandomStrength = 5f;

    public float obstacleRadius = 5f;
    public bool wanderFly;
 
    Vector3 _velocity;
    private Vector3 _steerForce;

    Vector3 _wander;
    float _nextWander;

    public LayerMask obstacle;

    int nHits;

    public Vector3 position { get { return transform.position; } }
    public Vector3 velocity { get { return _velocity; } }
    public float mass { get { return 1f; } }
    public float maxVelocity { get { return velocityLimit; } }

    virtual public void Start() {   }
    protected Vector3 Seek(Vector3 targetPosition)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = deltaPos.normalized * maxVelocity;
        return desiredVel - _velocity;
    }

    protected Vector3 Flee(Vector3 targetPosition)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = -deltaPos.normalized * maxVelocity;        //La velocidad deseada es la OPUESTA a seek
        return desiredVel - _velocity;
    }

    protected Vector3 Arrival(Vector3 targetPosition, float arrivalRadius)
    {
        var deltaPos = targetPosition - transform.position;
        //var desiredVel = deltaPos.normalized * maxSpeed;		//¡No normalizamos!
        var distance = deltaPos.magnitude;
        Vector3 desiredVel;
        if (distance < arrivalRadius)
        {
            //1. desiredVel = deltaPos.normalized * maxVelocity * distance/arrivalRadius;
            //2. desiredVel = deltaPos/distance   * maxVelocity * distance/arrivalRadius;
            //3. desiredVel = deltaPos            * maxVelocity           /arrivalRadius;
            //4. vvvv
            desiredVel = deltaPos * maxVelocity / arrivalRadius;
        }
        else
        {
            //desiredVel = deltaPos.normalized * maxVelocity;
            desiredVel = deltaPos / distance * maxVelocity;
        }
        return desiredVel - _velocity;
    }

    protected Vector3 ArrivalOptimized(Vector3 targetPosition, float arrivalRadius)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = Utility.Truncate(deltaPos * maxVelocity / arrivalRadius, maxVelocity);
        return desiredVel - _velocity;
    }

    protected Vector3 WanderRandomPos()
    {
        wanderDistanceAhead = 0f;   //HACK: Seteamos a 0 para que los gizmos de wander no muestren cualquier cosa

        if (Time.time > _nextWander)
        {
            _nextWander = Time.time + wanderPeriod;
            _wander = Utility.RandomDirection() * wanderRandomStrength;
        }
        return Seek(_wander);
    }

    protected Vector3 WanderTwitchy()
    {
        var desiredVel = Utility.RandomDirection() * maxVelocity;
        return desiredVel - _velocity;
    }

    protected Vector3 WanderWithState(float distanceAhead, float randomRadius, float randomStrength)
    {
        _wander = Utility.Truncate(_wander + Utility.RandomDirection() * randomStrength, randomRadius);
        var aheadPosition = transform.position + _velocity.normalized * distanceAhead + _wander;
        return Seek(aheadPosition);
    }

    protected Vector3 WanderWithStateTimed(float distanceAhead, float randomRadius, float randomStrength)
    {
        if (Time.time > _nextWander)
        {
            _nextWander = Time.time + wanderPeriod;
            _wander = Utility.Truncate(_wander + Utility.RandomDirection() * randomStrength, randomRadius);
        }
        var aheadPosition = transform.position + _velocity.normalized * distanceAhead + _wander;
        return Seek(aheadPosition);
    }

    //Pursuit: Seek a proyección futura
    protected Vector3 Pursuit(ISteerable who, float periodAhead)
    {
        var deltaPos = who.position - transform.position;
        var targetPosition = who.position + who.velocity * deltaPos.magnitude / who.maxVelocity;
        return Seek(targetPosition);
    }

    //Evade: Flee a proyección futura
    protected Vector3 Evade(ISteerable who, float periodAhead)
    {
        var deltaPos = who.position - transform.position;
        var targetPosition = who.position + who.velocity * deltaPos.magnitude / who.maxVelocity;
        return Flee(targetPosition);
    }

    protected Vector3 Avoidance(Vector3 distance)
    {
//         var adjustedDistance = distance.normalized;
//         var difference = obstacleRadius - distance.magnitude + 1;
//         adjustedDistance = adjustedDistance * difference;
// 		return _velocity - adjustedDistance;
		return Flee(distance);
	}

    //Reinicia las fuerzas
    protected void ResetForces()
    {
        _steerForce = Vector3.zero;
    }

    // Agrega fuerzas
    protected void AddForce(Vector3 force)
    {
        _steerForce += force;
    }

    // Aplica la integración: fuerza (aceleración) a velocidad -y- velocidad a posición.
    protected void ApplyForces()
    {
        //Euler integration
        var dt = Time.fixedDeltaTime;
        if (!wanderFly)
            _steerForce.y = 0f;
        _steerForce = Utility.Truncate(_steerForce, forceLimit);
        _velocity = Utility.Truncate(_velocity + _steerForce * dt, maxVelocity);
        transform.position += _velocity * dt;
        transform.forward = Vector3.Slerp(transform.forward, _velocity, 0.1f);
    }
}
