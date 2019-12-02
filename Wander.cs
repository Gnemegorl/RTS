using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    float wanderOrientation;
    float wanderOffset = 40;
    float wanderRadius = 3;
    float wanderRate = 0.4f;
    //float orientation;

    public override Steering getSteering(AgenteNPC agente)
    {
        Steering steering = new Steering(0, new Vector3(0, 0, 0));
        wanderOrientation += Random.Range(-1,1) * wanderRate;

        float targetOrientation;
        targetOrientation = wanderOrientation  + agente.orientation;

        Vector3 centro;
        centro = agente.position + wanderOffset * Cuerpo.orientationToVector(agente.orientation);
        

        centro += wanderRadius * orientationAsVector(targetOrientation);

        target = new Agente(centro);

        steering = base.getSteering(agente);

        steering.linear = agente.maxAcceleration * Cuerpo.orientationToVector(agente.orientation);

        return steering;
    }


    public Vector3 orientationAsVector(float orientation)
    {
        return new Vector3(Mathf.Sin(orientation), 0, Mathf.Cos(orientation));
    }
}
