using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBR
{
    private List<Regla> reglas;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SBR(string tipoUnidad)
    {
        reglas = new List<Regla>();
        // Reglas para el modo batalla total que toda unidad tiene.
        reglas.Add(new BatallaTotalDefensiva());
        reglas.Add(new BatallaTotalOfensiva());
        switch (tipoUnidad){
            case "r":
                reglas.Add(new BuscaEnemigo());
                reglas.Add(new Patrulla());
                reglas.Add(new curacion());
                reglas.Add(new descanso());
                reglas.Add(new aseguraRP(TipoRallyPoint.paso));
                break;
            case "k":
                reglas.Add(new BuscaEnemigo());
                reglas.Add(new curacion());
                reglas.Add(new descanso());
                reglas.Add(new capturaRP(10));
                reglas.Add(new ataqueBaseEnemiga(10));
                break;
            case "w":
                reglas.Add(new BuscaEnemigo());
                reglas.Add(new curacion());
                reglas.Add(new descanso());
                reglas.Add(new ataqueBaseEnemiga(10));
                break;
            default:
                break;
        }

    }

    public Accion tomaDecision(Unidad unidad)
    {

        // Recorre la lista de reglas y activa las pertinentes
        List<Regla> activadas = compruebaReglas(unidad);

        // Elige una de las reglas activadas en funcion de su prioridad
        return eligeRegla(activadas,unidad);

    }

    private List<Regla> compruebaReglas(Unidad unidad)
    {
        List<Regla> activadas = new List<Regla>();

        // Recorre las regla de tu lista de reglas

        foreach (Regla r in reglas)
        {
            // Comprueba los antecedentes para cada regla
            if (r.comprobarAntecedentes(unidad))
                // Si una regla cumple sus precedentes, añade esa regla a activadas
                activadas.Add(r);
        }

        // Devuelve una lista con las reglas candidatas
        return activadas;
    }

    /*
     * Metodo para elegir una regla de las candidatas.
     * Se escogerá en función de sus prioridades.
     * Devuelve la accion asociada a la regla escogida que debe llevar a cabo la unidad.
     */
    private Accion eligeRegla(List<Regla> reglasActivadas,Unidad unidad)
    {
        int prioridadActual = 0;
        int prioridadRegla;
        // Accion por defecto
        Accion accion = Accion.patrulla;

        // Buscamos de todas las reglas candidata a ser activadas, la de mayor prioridad
        foreach(Regla r in reglasActivadas)
        {
            prioridadRegla = r.getPrioridad();
            if (prioridadRegla > prioridadActual)
            {
                prioridadActual = prioridadRegla;
                accion = r.getAccionAsociada();
            }
        }

        return accion;
    }


}
