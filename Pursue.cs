using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek
{
    public AgenteNPC objetivoPursue;
    private float prediction = 2f;
    public float maxPrediction = 4f;

    override
    public Steering getSteering(AgenteNPC agente)
    {

        Vector3 direction = objetivoPursue.getPosition() - agente.getPosition();
        float distance = direction.magnitude;

        float speed = agente.getVelocity().magnitude;

        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else prediction = distance / speed;

        target = new AgenteNPC(objetivoPursue.getPosition());

        target.position += objetivoPursue.getVelocity() * prediction;

        return base.getSteering(agente);
    }

}