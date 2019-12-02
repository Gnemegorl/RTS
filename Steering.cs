using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    public float angular;
    public Vector3 linear;
    AgenteNPC agenteNPC;


    public Steering (float ang,Vector3 lin)
    {
        angular = ang;
        linear = lin;
    }

    public void addAngular(float ang)
    {
        angular += ang;
    }

    public void addLinear(Vector3 lin)
    {
        linear += lin;
    }

    public float getAngular()
    {
        return angular;
    }

    public Vector3 getLinear()
    {
        return linear;
    }


}
