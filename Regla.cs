using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Accion
    {ataque, defensaRP, ataqueRP, defensaBase,ataqueBase, huida,curacion, patrulla,buscaEnemigo,batallaTotal}



public abstract class Regla
{
    protected int prioridad;
    protected List<Antecedente> antecedente;
    protected Accion accionAsociada;


    public abstract bool comprobarAntecedentes(Unidad unidad);

    public int getPrioridad()
    {
        return prioridad;
    }

    public Accion getAccionAsociada()
    {
        return accionAsociada;
    }
}


public class descanso : Regla
{
    private Curandose curandose;

    public descanso()
    {
        prioridad = 10;
        accionAsociada = Accion.curacion;
        curandose = new Curandose();
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (curandose.comprobacion(unidad) && unidad.getCoordenadas().Equals(unidad.getCoordenadasBase()))
            return true;

        return false;
    }

}

public class curacion : Regla
{
    private SaludCritica saludCritica;

    public curacion()
    {
        prioridad = 10;
        saludCritica = new SaludCritica();
        accionAsociada = Accion.curacion;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (saludCritica.comprobacion(unidad))
            return true;
        return false;
    }
}

public class agresivo : Regla
{
    private int prioridadAgresivo = 5;
    private enemigosCerca enemigosCerca;

    public agresivo()
    {
        prioridad = prioridadAgresivo;
        accionAsociada = Accion.ataque;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (enemigosCerca.comprobacion(unidad))
            return true;
        return false;
    }
}


public class BuscaEnemigo : Regla
    {
        private enemigosCerca enemigosCerca;
        private int prioridadBuscaEnemigo;

        public BuscaEnemigo()
        {
            prioridad = 9;
            enemigosCerca = new enemigosCerca();

            accionAsociada = Accion.buscaEnemigo;
        }

        public override bool comprobarAntecedentes(Unidad unidad)
        {
            if (enemigosCerca.comprobacion(unidad))
                return true;
            else return false;
        }

    }

/** NUEVO
 * Regla que determina si una unidad debe huir por encontrarse en sutación de inferioridad.
 * Usara la influencia para determinar si debe huir.
 */ 
 /*
public class huidaInfluencia : Regla
{
    private float limiteInfluencia;
    private int prioridadHuida = 5;
    private influenciaCercana influenciaCercana;

    public huidaInfluencia(float limiteInf = 10)
    {
        limiteInfluencia = limiteInf;
        influenciaCercana = new influenciaCercana(limiteInfluencia);
        prioridad = prioridadHuida;
        accionAsociada = Accion.huida;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (influenciaCercana.comprobacion(unidad) && unidad.getModoDefensivo())
            return true;
        return false;
    }
}
*/

/**
 * Regla para asegurar un rally point. El tipo de rally point que interesa capturar debe pasarse como parametro al constructor.
 * 
 */
public class aseguraRP : Regla
{
    private TipoRallyPoint tipoRP;
    private CercaRP cercaRP;
    private int prioridadAseguraRP = 7;
    private float limiteInfluencia;

    public aseguraRP(TipoRallyPoint tipo)
    {
        prioridad = prioridadAseguraRP;
        tipoRP = tipo;
        cercaRP = new CercaRP(tipo);
        accionAsociada = Accion.defensaRP;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (unidad.getModoDefensivo() && cercaRP.comprobacion(unidad) )
            return true;
        return false;
    }
}

public class ataqueBaseEnemiga : Regla
{
    private int influenciaAtaqueFinal;
    private BaseCapturable baseCapturable;

    public ataqueBaseEnemiga(int influenciaAtaqueF = 5)
    {
        prioridad = 7;
        baseCapturable = new BaseCapturable(influenciaAtaqueF);
        accionAsociada = Accion.ataqueBase;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (baseCapturable.comprobacion(unidad) || unidad.isBatallaTotal())
            return true;

        return false;

    }
}

public class capturaRP : Regla
{
    private float limiteInfluencia;
    private RPCapturable RPCapturable;
    private SaludCritica saludCritica;

    public capturaRP(float limiteInf = 10)
    {
        limiteInfluencia = limiteInf;
        RPCapturable = new RPCapturable(limiteInf);
        saludCritica = new SaludCritica();
        prioridad = 6;
        accionAsociada = Accion.ataqueRP;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (unidad.getModoOfensivo() && RPCapturable.comprobacion(unidad) && saludCritica.comprobacion(unidad))
            return true;
        return false;
    }
}

public class Patrulla : Regla
{
        private int prioridadPatrulla = 5;
        private enemigosCerca enemigosCerca;

        public Patrulla()
        {
            prioridad = prioridadPatrulla;
            enemigosCerca = new enemigosCerca();
            accionAsociada = Accion.patrulla;
        }

        public override bool comprobarAntecedentes(Unidad unidad)
        {
            if (!enemigosCerca.comprobacion(unidad))
                return true;
            return false;
        }

    }


public class BatallaTotalOfensiva : Regla
{

    public BatallaTotalOfensiva()
    {
        prioridad = 8;
        accionAsociada = Accion.ataqueBase;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (unidad.isBatallaTotal() && unidad.getModoOfensivo())
            return true;
        return false;
    }
}

public class BatallaTotalDefensiva : Regla
{

    public BatallaTotalDefensiva()
    {
        prioridad = 8;
        accionAsociada = Accion.defensaBase;
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (unidad.isBatallaTotal() && unidad.getModoDefensivo())
            return true;
        return false;
    }
}

public class defiendeBase : Regla
{
    private BaseEnPeligro baseEnPeligro;

    public defiendeBase()
    {
        prioridad = 8;
        baseEnPeligro = new BaseEnPeligro();
    }

    public override bool comprobarAntecedentes(Unidad unidad)
    {
        if (unidad.getBando() && baseEnPeligro.comprobacion(unidad))
            return true;
        return false;
    }

}

