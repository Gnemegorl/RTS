using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour {

    override
    public Steering getSteering(AgenteNPC agente)
    {
        Vector3 lineal = (target.getPosition() - agente.transform.position).normalized;
        lineal *= agente.maxAcceleration;
        lineal -= agente.velocity;
        return new Steering(0,lineal);
    }
}
