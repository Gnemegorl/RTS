using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    public Agente target;
    public Steering steering;

    [Range(1, 3)]
    public float weight = 1;

    public abstract Steering getSteering(AgenteNPC agente);

    public void setTarget(AgenteNPC agente) {
        this.target = agente;
    }

    public Agente getTarget()
    {
        return this.target;
    }
    public float getWeight()
    {
        return weight;
    }
}
