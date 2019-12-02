using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unidad
{
    public int index;
    List<CasillaGrid> auxPath;

    private static float rangoAtaqueRanger = 2f;
    private static float rangoVisionRanger = 3f;
    private static float rangoAccionRanger = 3f;
    private static int saludTotalRanger = 50;
    private static float velocidadRanger = .5f;

    public Ranger(Vector2 posicion) : base(posicion)
    {
        bando = true;
    }

    new void Start()
    {

        moveIndex = new Dictionary<TipoCasilla, float>
        {
            {TipoCasilla.pradera, .8f},
            {TipoCasilla.bosque, .5f },
            {TipoCasilla.carretera, .8f },
            {TipoCasilla.rio, Mathf.Infinity},
            {TipoCasilla.montaña, 10f},
            {TipoCasilla.safepoint, .8f},
            {TipoCasilla.ciudad, .8f}

        };

        velocidadUnidad = velocidadRanger;
        modoOfensivo = true;
        modoDefensivo = false;
        indiceNodo = 1;
        daño = 10;
        path = new List<CasillaGrid>();
        controlador = GameObject.FindGameObjectWithTag("Controlador").GetComponent<ControladorMapa>();
        //heuristica = new float[(int)base.GetControlador().getDim(), (int)base.GetControlador().getDim()];
        setRangoAtaque(rangoAtaqueRanger);
        setRangoVision(rangoVisionRanger);
        setRangoAccion(rangoAccionRanger);
        setSaludTotal(saludTotalRanger);
        setSaludActual(saludTotal);
        index = 0;

        sbr = new SBR("r");
        base.Start();


    }



}
