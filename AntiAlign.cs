using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAlign : SteeringBehaviour
{
    float timeToTarget = 0.1f;
    float targetRotation = 0;

    float test = 0;

    override
    public Steering getSteering(AgenteNPC agente)
    {
        Steering steering = new Steering(0, new Vector3(0, 0, 0));

        float rotacion = 0;
        if (agente.getOrientation() > 3.14f)
            rotacion = 6.28f - agente.getOrientation() + target.getOrientation() -Mathf.PI;
        else rotacion = target.orientation - agente.orientation;

        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize > target.exteriorAngle)
        {
            targetRotation = agente.maxRotation;
        }

        else if (rotationSize > target.interiorAngle)
        {
            targetRotation = agente.maxRotation * rotacion / rotationSize;
        }

        targetRotation *= rotacion / rotationSize;
        steering.angular = targetRotation - agente.rotation;
        steering.angular /= timeToTarget;

        return steering;

    }


}
