using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unidad
{
    public int index;
    List<CasillaGrid> auxPath;

    private static float rangoAtaqueKnight = .5f;
    private static float rangoVisionKnight = 3f;
    private static float rangoAccionKnight = 3f;
    private static int saludTotalKnight = 70;
    private static float velocidadKnight = .9f;

    public Knight(Vector2 posicion) : base(posicion)
    {
        bando = true;
    }

    new void Start()
    {

        moveIndex = new Dictionary<TipoCasilla, float>
        {
            {TipoCasilla.pradera, .7f},
            {TipoCasilla.bosque, 5f },
            {TipoCasilla.carretera, .5f },
            {TipoCasilla.rio, Mathf.Infinity},
            {TipoCasilla.montaña, 15f},
            {TipoCasilla.safepoint, .8f},
            {TipoCasilla.ciudad, .8f}

        };

        velocidadUnidad = velocidadKnight;
        modoOfensivo = true;
        modoDefensivo = false;
        indiceNodo = 1;
        daño = 10;
        path = new List<CasillaGrid>();
        controlador = GameObject.FindGameObjectWithTag("Controlador").GetComponent<ControladorMapa>();
        setRangoAtaque(rangoAtaqueKnight);
        setRangoVision(rangoVisionKnight);
        setRangoAccion(rangoAccionKnight);
        setSaludTotal(saludTotalKnight);
        setSaludActual(saludTotal);
        index = 0;

        sbr = new SBR("k");
        base.Start();


    }
    


}

