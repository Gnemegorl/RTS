using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unidad
{
    public int index;
    List<CasillaGrid> auxPath;

    private static float rangoAtaqueWarrior = .5f;
    private static float rangoVisionWarrior = 3f;
    private static float rangoAccionWarrior = 3f;
    private static int saludTotalWarrior = 80;
    private static float velocidadWarrior = .7f;

    public Warrior(Vector2 posicion) : base(posicion)
    {
        bando = true;
    }

    new void Start()
    {

        moveIndex = new Dictionary<TipoCasilla, float>
        {
            {TipoCasilla.pradera, .8f},
            {TipoCasilla.bosque, 2f },
            {TipoCasilla.carretera, .7f },
            {TipoCasilla.rio, Mathf.Infinity},
            {TipoCasilla.montaña, 6f},
            {TipoCasilla.safepoint, .8f},
            {TipoCasilla.ciudad, .5f}

        };

        velocidadUnidad = velocidadWarrior;
        modoOfensivo = true;
        modoDefensivo = false;
        indiceNodo = 1;
        daño = 15;
        path = new List<CasillaGrid>();
        controlador = GameObject.FindGameObjectWithTag("Controlador").GetComponent<ControladorMapa>();
        setRangoAtaque(rangoAtaqueWarrior);
        setRangoVision(rangoVisionWarrior);
        setRangoAccion(rangoAccionWarrior);
        setSaludTotal(saludTotalWarrior);
        setSaludActual(saludTotal);
        index = 0;

        sbr = new SBR("w");
        base.Start();


    }



}

