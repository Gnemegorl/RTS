using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{
    override
    public Steering getSteering(AgenteNPC agente)
    {
        Steering steering = new Steering(0, new Vector3(0, 0, 0));
        AgenteNPC newTarget = new AgenteNPC(new Vector3(0, 0, 0));
        Vector3 direction = target.position - agente.position;

        if (direction.magnitude == 0)
            return steering;

        agente.orientation = Mathf.Atan2(direction.x, direction.z);

        return base.getSteering(agente);

    }
}
