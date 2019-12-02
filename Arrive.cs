using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{
    override
    public Steering getSteering(AgenteNPC agente)
    {
        float targetSpeed = 1;
        Vector3 targetVelocity;
        Vector3 lineal = Vector3.zero;
        float timeToTarget = 0.1f;
        float maxAcc = agente.maxAcceleration;

        Vector3 direction = target.getPosition() - agente.transform.position;
        float distance = direction.magnitude;
        if (distance < target.interiorRadius) return new Steering(0, Vector3.zero);


        if (distance > target.exteriorRadius) targetSpeed = maxAcc;
        else targetSpeed = maxAcc * distance / target.exteriorRadius;

        targetVelocity = direction;
        targetVelocity = targetVelocity.normalized;
        targetVelocity *= targetSpeed;
        lineal = targetVelocity - agente.velocity;
        lineal /= timeToTarget;
        
        if (lineal.magnitude > maxAcc)
        {
            lineal = lineal.normalized;
            lineal *= agente.maxAcceleration;
        }

        return new Steering(0,lineal);
    }

}
