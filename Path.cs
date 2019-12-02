using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Path: MonoBehaviour
{

    public List<Waypoint> points;

    public Path(List<Vector3> camino)
    {
        points = new List<Waypoint>();

        foreach (Vector3 v in camino)
            points.Add(new Waypoint(v));
    }


    public void setWP(List<Vector3> lista)
    {
        foreach (Vector3 v in lista)
            points.Add(new Waypoint(v));
    }
    public int getParam(Vector3 position,int lastParam)
    {
        return 0;
    }
    
    public Waypoint getPosition(int param)
    {
        return points[param];
    }

    public int getLength()
    {
        return points.Count;
    }

    public List<Waypoint> getWaypoints()
    {
        return this.points;
    }

}
