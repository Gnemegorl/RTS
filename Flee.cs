using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    override
    public Steering getSteering(AgenteNPC agente)
    {
        Vector3 lineal = (agente.getPosition() - target.getPosition()).normalized;
        lineal *= agente.maxAcceleration;
        lineal -= agente.velocity;
        return new Steering(0, lineal);
    }
}
