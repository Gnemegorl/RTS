using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    [Range(0.1f,1f)]
    public float timeToTarget = 0.1f;

    public override Steering getSteering(AgenteNPC agente)
    {
        Vector3 linear = target.getVelocity() - agente.getVelocity();
        float maxAcc = agente.getMaxAcceleration();

        linear /= timeToTarget;

        if (linear.magnitude > maxAcc)
        {
            linear = linear.normalized;
            linear *= maxAcc;
        }

        return new Steering(0,linear);
    }

}
