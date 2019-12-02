using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : Agente
{
    public float radius = 1;

    public Waypoint(Vector3 posicion,float radio=.5f): base(posicion)
    {
        radius = radio;
    }

    public float getRadius()
    {
        return radius;
    }
}
