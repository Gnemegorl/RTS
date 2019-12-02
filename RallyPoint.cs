using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoRallyPoint
{baseA,baseB,curacion,cobertura,paso}


public class RallyPoint
{
    private Vector2 coordenadas;
    private TipoRallyPoint tipo;

    public RallyPoint (Vector2 coord,TipoRallyPoint tipoR)
    {
        coordenadas = coord;
        tipo = tipoR;
    }

    public Vector2 getCoordenadas()
    {
        return coordenadas;
    }

    public TipoRallyPoint getTipoRallyPoint()
    {
        return tipo;
    }


}
