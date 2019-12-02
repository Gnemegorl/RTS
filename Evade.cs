using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Flee
{
    public AgenteNPC objetivoEvade;
    private float prediction = 2f;
    public float maxPrediction = 4f;

    override
    public Steering getSteering(AgenteNPC agente)
    {

        Vector3 direction = objetivoEvade.getPosition() - agente.getPosition();
        float distance = direction.magnitude;

        float speed = agente.velocity.magnitude;

        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else prediction = distance / speed;

        target = new AgenteNPC(objetivoEvade.getPosition());

        target.position += objetivoEvade.getVelocity() * prediction;

        return base.getSteering(agente);
    }

}
