using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Antecedente
{
    bool comprobacion(Unidad unidad);
}

public class enemigosCerca : Antecedente
{

    public enemigosCerca() { }

    public bool comprobacion(Unidad unidad)
    {
        int i = 0;
        List<Unidad> enemigos = unidad.getEnemigos();
        int numeroEnemigos = enemigos.Count;
        float vision = unidad.getRangoVision();

        while (i < numeroEnemigos)
        {
            if (vision > Vector3.Distance(enemigos[i].GetComponent<Transform>().transform.position, unidad.GetComponent<Transform>().transform.position))
                return true;
            i++;
        }

        return false;
    }
}

public class Curandose : Antecedente
{
    public Curandose() { }
    
    public bool comprobacion(Unidad unidad)
    {
        if (unidad.getModoDefensivo() && unidad.getSaludActual()*100/unidad.getSaludTotal() < 100)
        {
            return true;
        }
        if (unidad.getModoOfensivo() && unidad.getSaludActual()*100/unidad.getSaludTotal() < 60)
        {
            return true;
        }
        return false;
    }
}

public class SaludCritica : Antecedente
{
    public SaludCritica() { }
    private float porcentajeSaludCritica = 20;

    public bool comprobacion(Unidad unidad)
    {
        if (unidad.getSaludActual()*100/unidad.getSaludTotal()  <= porcentajeSaludCritica+10 && unidad.getModoDefensivo())
            return true;
        
        if (unidad.getSaludActual()*100/unidad.getSaludTotal() <= porcentajeSaludCritica && unidad.getModoOfensivo())
            return true;
            
        return false;
    }

}

/*
 * Antecedente para determinar si tu base está en peligro. Se considera que la seguridad de una base peligra si la
 * influencia enemiga en sus alrededores sobrepasa un limite.
 */ 
public class BaseEnPeligro : Antecedente
{
    private float limitePeligroInfluencia;

    public BaseEnPeligro(float limitePI = 6) { limitePeligroInfluencia = limitePI; }

    public bool comprobacion(Unidad unidad)
    {
        if (unidad.getBando() && unidad.getInfluenciaBase() < -limitePeligroInfluencia)
            return true;

        if (!unidad.getBando() && unidad.getInfluenciaBase() > limitePeligroInfluencia)
            return true;

        return false;

    }
}


public class CercaRP : Antecedente
{
        private TipoRallyPoint tipoRP;
        public CercaRP(TipoRallyPoint tipo) { tipoRP = tipo; }

       public bool comprobacion(Unidad unidad)
        {
            if (unidad.cercaRallyPoint(tipoRP) != null)
                return true;
            return false;
        }
}

public class RPCapturable : Antecedente
{
    private float limiteInfluencia;

    public RPCapturable(float limiteInf)
    {
        limiteInfluencia = limiteInf;
    }

    public bool comprobacion(Unidad unidad)
    {
        int coordenadasRPx,coordenadasRPy;
        float influenciaRP;
        List<RallyPoint> listaRP = unidad.getRallyPoints();

        foreach(RallyPoint rp in listaRP)
        {
            coordenadasRPx = (int)rp.getCoordenadas().x;
            coordenadasRPy = (int)rp.getCoordenadas().y;
            influenciaRP = unidad.GetControlador().getInfluenciaAt(coordenadasRPx, coordenadasRPy);

            if (unidad.getBando() && influenciaRP < 0 && influenciaRP >= -limiteInfluencia)
                return true;
            if (!unidad.getBando() && influenciaRP > 0 && influenciaRP <= limiteInfluencia)
                return true;
        }
        return false;
    }
}

public class BaseCapturable : Antecedente
{
    private float limiteInfluencia;

    public BaseCapturable(float limiteInf)
    {
        limiteInfluencia = limiteInf;
    }

    public bool comprobacion(Unidad unidad)
    {
        bool bando = unidad.getBando();
        CasillaGrid baseEnemiga;
        float influencia;

        if (bando)
            baseEnemiga = unidad.getCoordenadasTipoRallyPoint(TipoRallyPoint.baseB);
        else
            baseEnemiga = unidad.getCoordenadasTipoRallyPoint(TipoRallyPoint.baseA);

        influencia = unidad.GetControlador().getInfluenciaAt(baseEnemiga);

        if (bando && unidad.getModoOfensivo() && influencia > limiteInfluencia)
            return true;

        if (!bando && unidad.getModoOfensivo() && influencia < limiteInfluencia)
            return true;

        return false;
    }
}
public class influenciaCercana : Antecedente
{
        private float limiteInfluencia;

        public influenciaCercana(float limiteInf)
        {
            limiteInfluencia = limiteInf;
        }

        public bool comprobacion(Unidad unidad)
        {
            float influenciaUnidad = Mathf.Abs(unidad.influenciaCercana());
            float influenciaEnemigo = Mathf.Abs(unidad.getEnemigoMasCercano().influenciaCercana());
            float diferenciaInfluencia = influenciaEnemigo - influenciaUnidad;
            if ( diferenciaInfluencia > 0 && diferenciaInfluencia < limiteInfluencia)
                return true;

            return false;
        }
}


 