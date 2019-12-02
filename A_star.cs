using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_star: MonoBehaviour
{
    public float largo, ancho;
    List<Casilla> casillas;
    List<Casilla> closed;
    List<Casilla> open;

    private float[,] heuristica;
    private float[,] moveIndex;
    private Casilla goal;
    public GridEstrategia grid;


    void Start()
    {
        closed = new List<Casilla>();
        open = new List<Casilla>();

        for (int i = 0; i < ancho; i++)
        {
            for (int j = 0; j < largo; j++)
                moveIndex[j, i] = 1;
        }
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            goal = grid.getCasillaRaton();
           
            if (goal != null)
            {
                InitHeurística(goal);
                
            }
        }
    }

    public void InitHeurística(Casilla target)
    {
        for (int i = 0; i < grid.getAncho(); i++)
            for (int j = 0; j < grid.getLargo(); j++)
                heuristica[i, j] = Mathf.Sqrt(Mathf.Pow(i - target.getPosicionGrid().x, 2) + Mathf.Pow(j - target.getPosicionGrid().y, 2)) + moveIndex[i, j];


    }

    public List<Casilla> nextTarget(Casilla c, Casilla goal)
    {
        float menor = Mathf.Infinity;
        int i = 0;
        Nodo candidato = null;
        

        List<Nodo> abiertos = calculoAdyacente(c, c.getAdyacentes());
        List<Nodo> cerrados = new List<Nodo>();
        List<Nodo> aux = new List<Nodo>();

        //abiertos.Add(new Nodo(c.getPosicionGrid().x, c.getPosicionGrid().y, heuristica[(int)c.getPosicionGrid().x, (int)c.getPosicionGrid().y]))

        Vector2 cCoord = new Vector2(candidato.getPosicionGridX(), candidato.getPosicionGridY());

        while ( open.Count != 0 && !CompNC(cCoord, goal))
        {
            float coste = heuristica[(int)open[i].getPosicionGrid().x, (int)open[i].getPosicionGrid().y] + calcDist(c.getPosicionGrid(), c.getPosicionGrid());
            if (coste < menor)
            {
                candidato = abiertos[i];
                menor = coste;
            }

            abiertos.Remove(candidato);
            Casilla cAux = grid.getCasillaAt(candidato.getPosicionGridX(), candidato.getPosicionGridY());


            cerrados.Add(candidato);
            abiertos.AddRange(calculoAdyacente(c, c.getAdyacentes()));


        }
        


        return null;
    }


    public bool CompNC(Vector2 n, Casilla c)
    {
        Vector3 coordN = new Vector3(n.x, n.y, 0);
        return coordN.Equals(c.getPosicionGrid());
    }
    private List<Nodo> calculoAdyacente(Casilla central, List<Casilla> adyacentes)
    {
        List<Nodo> listaNodos = new List<Nodo>();
        float coste = 0;
        Vector2 posCentral, posAdyacente;
        int posX = 0;
        int posY = 0;

        foreach (Casilla c in adyacentes)
        {
            posCentral = central.getPosicionGrid();
            posAdyacente = c.getPosicionGrid();

            coste = Mathf.Sqrt(Mathf.Pow((posCentral.x - posAdyacente.x), 2) + Mathf.Pow((posCentral.y - posAdyacente.y), 2));

            posX = (int)posAdyacente.x;
            posY = (int)posAdyacente.y;
            coste = coste + heuristica[posX, posY];

            listaNodos.Add(new Nodo(posX, posY, coste, new Vector2(central.getPosicionGrid().x, central.getPosicionGrid().y)));
        }
        return listaNodos;
    }

    public float calcDist(Vector2 origen, Vector2 fin)
    {
        return Mathf.Sqrt(Mathf.Pow((origen.x - fin.x), 2) + Mathf.Pow((origen.y - fin.y), 2));
    }
}
