using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowPath : Seek
{
    public Path path;
    public int currentNode = 0;
    public int pathDir = 1;

    override
    public Steering getSteering(AgenteNPC agente)
    {

        // AÑADIDO PARA EL GRID
        /*if (path == null)
            return new Steering(0,agente.getPosition());*/
        setPath(agente);

        if (path == null)
            return new Steering(0, Vector3.zero);

        

        // TO DO, REINICIAR CURRENT NODE A 0

        Waypoint targetPoint = path.getPosition(currentNode);
        int pathLength = path.getLength();

        if (Vector3.Distance(agente.transform.position, targetPoint.getPosition()) <= targetPoint.getRadius())
        {
            currentNode += pathDir;
        }
        // Opcion 1. Para llegado al último punto del camino
        if (currentNode >= pathLength)
            currentNode = pathLength - 1;
            
        target = targetPoint;
        return base.getSteering(agente);
    }

    public void setPath(AgenteNPC agente)
    {
        if (path != null)
            if (currentNode > path.getLength())
                currentNode = 0;
        path = agente.getPath();

    }

}
