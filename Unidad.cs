using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodoA
{
    CasillaGrid c;
    CasillaGrid padre;
    public float g;
    public float h;

    public NodoA(CasillaGrid casilla, CasillaGrid pred, float coste)
    {
        c = casilla;
        padre = pred;
        g = coste;
    }
    

    //Propiedad calculada
    public float f()
    {
        return g + h;
    }

    public CasillaGrid getPadre()
    {
        return this.padre;
    }

    public CasillaGrid getCasilla()
    {
        return this.c;
    }

    public void setPadre(CasillaGrid pred)
    {
        this.padre = pred;
    }

    public void setCasilla(CasillaGrid casilla)
    {
        this.c = casilla;
    }
}

public class Unidad : MonoBehaviour
{
    public int saludTotal, saludActual;
    public Transform healthbar;
    protected float rangoAtaque, rangoVision, rangoAccion;
    protected Vector2 coordenadas;
    protected float influencia = 1;
    protected CasillaGrid target;

    public List<CasillaGrid> path = new List<CasillaGrid>();
    public bool modoDefensivo, modoOfensivo;
    public bool bando;
    protected SBR sbr;
    protected ControladorMapa controlador;
    protected int indiceNodo = 0;

    protected List<CasillaGrid>[] patrullas;
    // TO DO. CAMBIAR NUMEROPATRULLA POR IDPATRULLA
    public int numeroPatrulla;
    protected int indicePatrulla = 0;
    protected bool patrullando = false;

    public List<Unidad> enemigos;
    public List<RallyPoint> rallypoints;

    [Range(0.5f, 10)]
    public float velocidadUnidad;

    [Range(10, 50)]
    public float daño;

    protected bool ataqueDisponible = true;
    protected bool enCombate = false;
    public bool seleccionada = false;
    public CasillaGrid baseUnidad;

    protected List<CasillaGrid> adyacentes;
    protected CasillaGrid cg;
    protected Dictionary<TipoCasilla, float> moveIndex;
    protected bool unidadViva;
    protected bool canMove;

    protected float limiteInfluencia = 10;

    public bool batallaTotal = false;

    private Sprite spriteBuscaEnemigo;
    private Sprite spritePatrulla;
    private Sprite spriteAtacaBaseEnemiga;
    private Sprite spriteDefiendeBase;
    private Sprite spriteCuracion;


    public Unidad(Vector2 posicion)
    {
        this.coordenadas = posicion;
    }

    public Vector2 buscaCobertura()
    {
        float distancia = Mathf.Infinity;
        Vector2 coordenadasCobertura = Vector2.zero;

        foreach(RallyPoint rp in rallypoints)
        {
            if (rp.getTipoRallyPoint() == TipoRallyPoint.cobertura && Vector2.Distance(rp.getCoordenadas(), this.getCoordenadas()) < distancia)
            {
                coordenadasCobertura = rp.getCoordenadas();
                distancia = Vector2.Distance(rp.getCoordenadas(), getCoordenadas());
            }
        }
        return coordenadasCobertura;
    }

    public Vector2 defiendeBase()
    {
        int i = 0;
        int size = rallypoints.Count;
        while( i< size)
        {
            if (bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseA)
                return rallypoints[i].getCoordenadas();

            if (!bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseB)
                return rallypoints[i].getCoordenadas();
            i++;
        }

        // Prevención de errores. Nunca deberia llegar a este punto
        return Vector2.zero;
    }

    public Vector2 atacaBase()
    {
        int i = 0;
        int size = rallypoints.Count;
        while (i < size)
        {
            if (bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseB)
                return rallypoints[i].getCoordenadas();

            if (!bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseA)
                return rallypoints[i].getCoordenadas();
            i++;
        }

        // Prevención de errores. Nunca deberia llegar a este punto
        return Vector2.zero;
    }

    public Vector2 decideAccion(Accion accion)
    {
        // TO DO. ASOCIAR ACCIONES A DESTINOS
        switch (accion)
        {
            case Accion.ataque:
                return Vector2.zero;
            case Accion.buscaEnemigo:
                return Vector2.zero;
            case Accion.defensaBase:
                return Vector2.zero;
            case Accion.defensaRP:
                return Vector2.zero;
            case Accion.huida:
                return Vector2.zero;
            case Accion.batallaTotal:
                return Vector2.zero;
            default:
                return Vector2.zero;
        }

    }

    public ControladorMapa GetControlador()
    {
        return this.controlador;
    }

    public void setPath(List<NodoA> lista)
    {
        if (lista == null)
            return;
        foreach (NodoA nodo in lista)
        {
            path.Add(nodo.getCasilla());
        }
    }

    public List<CasillaGrid> getPath()
    {
        return this.path;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        coordenadas = Vector2.zero;
        ataqueDisponible = true;
        unidadViva = true;
        canMove = true;
        batallaTotal = false;

        enemigos = new List<Unidad>();
        Unidad[] unidades = GameObject.FindObjectsOfType<Unidad>();
        foreach(Unidad u in unidades)
        {
            if (bando != u.getBando())
                enemigos.Add(u);
        }


        // Definimos los rallypoints
        rallypoints = new List<RallyPoint>();

        rallypoints.Add(new RallyPoint(new Vector2(6, 2), TipoRallyPoint.baseA));
        rallypoints.Add(new RallyPoint(new Vector2(37, 36), TipoRallyPoint.baseB));

        rallypoints.Add(new RallyPoint(new Vector2(9,16), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(9, 20), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(32, 11), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(32, 15), TipoRallyPoint.paso));

        rallypoints.Add(new RallyPoint(new Vector2(13, 4), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(35, 6), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(4, 23), TipoRallyPoint.paso));
        rallypoints.Add(new RallyPoint(new Vector2(30, 18), TipoRallyPoint.paso));

        if (bando)
            baseUnidad = controlador.getCasillaAt(6,2);
        else
            baseUnidad = controlador.getCasillaAt(37, 36);


        // Inicializamos las casillas de las patrullas
        patrullas = new List<CasillaGrid>[6];

        CasillaGrid pasoA1 = controlador.getCasillaAt(9,16);
        CasillaGrid pasoA2 = controlador.getCasillaAt(32,11);
        CasillaGrid pasoA3 = controlador.getCasillaAt(13, 4);
        CasillaGrid pasoA4 = controlador.getCasillaAt(35, 6);
        CasillaGrid curacionA = controlador.getCasillaAt(13,4);

        CasillaGrid pasoB1 = controlador.getCasillaAt(9, 20);
        CasillaGrid pasoB2 = controlador.getCasillaAt(6,35);
        CasillaGrid pasoB3 = controlador.getCasillaAt(30,18);
        CasillaGrid baseB = controlador.getCasillaAt(37,36);

        List<CasillaGrid> patrullaRioNorte = new List<CasillaGrid>();
        patrullaRioNorte.Add(pasoA1);
        patrullaRioNorte.Add(pasoA2);

        List<CasillaGrid> patrullaNorte = new List<CasillaGrid>();
        patrullaNorte.Add(pasoA3);
        patrullaNorte.Add(pasoA4);

        List<CasillaGrid> patrullaBosque = new List<CasillaGrid>();
        patrullaBosque.Add(pasoA3);
        patrullaBosque.Add(pasoA1);
        patrullaBosque.Add(pasoA2);

        List<CasillaGrid> patrullaRioSur = new List<CasillaGrid>();
        patrullaRioSur.Add(pasoB1);
        patrullaRioSur.Add(pasoB3);


        List<CasillaGrid> patrullaSur = new List<CasillaGrid>();
        patrullaSur.Add(pasoB2);
        patrullaSur.Add(baseB);

        List<CasillaGrid> patrullaCuadrado = new List<CasillaGrid>();
        patrullaCuadrado.Add(pasoB1);
        patrullaCuadrado.Add(pasoB2);
        patrullaCuadrado.Add(baseB);
        patrullaCuadrado.Add(pasoB3);


        // Asignamos todas las patrullas a la estructura de datos patrullas.
        // Cada unidad tendrá asignada una patrulla por medio de un parámetro
        patrullas[0] = patrullaRioNorte;
        patrullas[1] = patrullaNorte;
        patrullas[2] = patrullaBosque;
        patrullas[3] = patrullaRioSur;
        patrullas[4] = patrullaSur;
        patrullas[5] = patrullaCuadrado;

        spriteBuscaEnemigo = Resources.Load<Sprite>("buscaEnemigo");
        spritePatrulla = Resources.Load<Sprite>("patrulla");
        spriteAtacaBaseEnemiga = Resources.Load<Sprite>("atacaBaseEnemiga");
        spriteDefiendeBase = Resources.Load<Sprite>("defiendeBase");
        spriteCuracion = Resources.Load<Sprite>("curacion");

    }

    protected void Update()
    {

        actualizaBarraSalud();

        if (seleccionada)
        {
            controlador.selected.GetComponent<MeshRenderer>().enabled = true;
            controlador.selected.transform.position = new Vector3(transform.position.x, transform.position.y, controlador.selected.transform.position.z);
        }

        int coordenadaGridX = (int)controlador.getCasillaActual(this).getPosicionGrid().x;
        int coordenadaGridY = (int)controlador.getCasillaActual(this).getPosicionGrid().y;
        Accion a = Accion.buscaEnemigo;

        this.setCoordenadas(coordenadaGridX, coordenadaGridY);

        bool click = Input.GetMouseButtonDown(1);

        if (seleccionada && click || path.Count > 0)
        {
            path.Clear();
            if (click)
                target = controlador.getCasillaRaton();

            if (target != null)
            {
                cg = controlador.getCasillaActual(this);
                adyacentes = cg.getAdyacentes();

                if (moveIndex[target.getTipoCasilla()] == Mathf.Infinity)
                    target = getClosest(cg, target);

                setPath(AStar(cg, target, moveIndex));
                MovePlayer();
            }
        }


        if (!seleccionada)
        {
            a = sbr.tomaDecision(this);
            target = new CasillaGrid(parseAction(a));


            if (target != null)
            {
                if (patrullando)
                {
                    List<CasillaGrid> patrullaActual = patrullas[numeroPatrulla];
                    int size = patrullaActual.Count;
                    if (Vector3.Distance(transform.position, target.getPosicion()) < 0.5f)
                    {
                        indicePatrulla = (indicePatrulla + 1) % size;
                        target = patrullaActual[indicePatrulla];
                    }
                }
                //TODO: hacer que se compruebe constantemente la posición del target, y cuando cambie, volver a lanzar el A*
                //COMPROBAR LA POSICIÓN FINAL DEL TARGET DE CADA UNIDAD

                cg = controlador.getCasillaActual(this);
                adyacentes = cg.getAdyacentes();


                setPath(AStar(cg, target, moveIndex));
                MovePlayer();
            }
        }
        actualizarIcono(a);
       
    }

    private void actualizarIcono(Accion a)
    {
        GameObject iconoAccion = transform.GetChild(1).gameObject;

        switch (a)
        {
            case Accion.buscaEnemigo:
                iconoAccion.GetComponent<SpriteRenderer>().sprite = spriteBuscaEnemigo;
                break;
            case Accion.patrulla:
                iconoAccion.GetComponent<SpriteRenderer>().sprite = spritePatrulla;
                break;
            case Accion.ataqueBase:
                iconoAccion.GetComponent<SpriteRenderer>().sprite = spriteAtacaBaseEnemiga;
                break;
            case Accion.defensaBase:
                iconoAccion.GetComponent<SpriteRenderer>().sprite = spriteDefiendeBase;
                break;
            case Accion.curacion:
                iconoAccion.GetComponent<SpriteRenderer>().sprite = spriteCuracion;
                break;
            default:
                break;
        }

}

    private void actualizaBarraSalud()
    {
        float salud = (float)saludActual / (float)saludTotal;
        healthbar.localScale = new Vector3(salud, 1);
    }

    public void MovePlayer()
    {
        if (!isUnidadViva() || !ataqueDisponible)
            return;

        int pathLength = path.Count;

        if (pathLength <= 1)
            return;

        Vector3 targetPoint = path[indiceNodo].getPosicion();
        Vector3 director = targetPoint - transform.position;
        Vector3 aux;
        float distancia;
        Vector3 movimiento = transform.position;

        aux = director.normalized;
        distancia = Vector3.Distance(getEnemigoMasCercano().transform.position, transform.position);
        if (!enCombate)
        {
            movimiento = Vector3.MoveTowards(transform.position, targetPoint, 1 / moveIndex[path[indiceNodo].getTipoCasilla()] * Time.deltaTime * velocidadUnidad * multiplicadorSeleccion());
            transform.position = movimiento;
        }

        if (enCombate && ataqueDisponible && distancia < rangoAtaque && getEnemigoMasCercano().isUnidadViva() && unidadViva)
        {
            getEnemigoMasCercano().modificarSalud(-daño);
            StartCoroutine(WaitSecAtaque(1f));
        }

        if (enCombate && distancia > rangoAtaque && getEnemigoMasCercano().isUnidadViva() && unidadViva)
        {
            movimiento = Vector3.MoveTowards(transform.position, targetPoint, 1 / moveIndex[path[indiceNodo].getTipoCasilla()] * Time.deltaTime * velocidadUnidad * multiplicadorSeleccion());
            transform.position = movimiento;
        }

    }

    private float multiplicadorSeleccion()
    {
        if (seleccionada)
            return 2;
        return 1;
    }

    IEnumerator WaitSecAtaque(float sec)
    {
        ataqueDisponible = false;
        yield return new WaitForSeconds(sec);
        ataqueDisponible = true;
    }




    public CasillaGrid getClosest(CasillaGrid current, CasillaGrid goal)
    {

        float bestDist = Mathf.Infinity;
        float bestCost = Mathf.Infinity;
        CasillaGrid best = null;
        List<CasillaGrid> adyacentes = goal.getAdyacentes();
        foreach (CasillaGrid c in adyacentes)
        {
            /*
             * TODO
             * Antes de comprobar el adyacente más cercano al actual, comprobar el adyacente de menor coste.
             * Si todos tienen el mismo coste, comprobar distancias.
             *
             * */

            if (moveIndex[c.getTipoCasilla()] < bestCost)
            {
                bestDist = calcDist(current, c);
                best = c;
                bestCost = moveIndex[c.getTipoCasilla()];
            }

        }

        if (bestCost == Mathf.Infinity)
        {
            foreach (CasillaGrid c in adyacentes)
            {
                if (calcDist(current, c) <= bestDist)
                {
                    bestDist = calcDist(current, c);
                    best = c;
                    bestCost = moveIndex[c.getTipoCasilla()];
                }
            }
        }

        if (bestCost == Mathf.Infinity)
        {
            best = getClosest(current, best);
        }

        return best;
    }

    /**
     * Parseo de Acciones
     **/
    public CasillaGrid parseAction(Accion a)
    {

        switch (a)
        {
            case Accion.buscaEnemigo:
                Unidad e = getEnemigoMasCercano();
                patrullando = false;
                if (Vector3.Distance(transform.position, e.transform.position) < rangoAtaque)
                    enCombate = true;
                return controlador.getCasillaActual(e);
            case Accion.patrulla:
                enCombate = false;
                return eligePatrulla();
            case Accion.curacion:
                enCombate = false;
                return puntoCuracion();
            case Accion.defensaRP:
                enCombate = false;
                return controlador.getCasillaAt((int)cercaRallyPoint(TipoRallyPoint.paso).getCoordenadas().x, (int)cercaRallyPoint(TipoRallyPoint.paso).getCoordenadas().y);
            case Accion.ataqueRP:
                return RPDesprotegido();
            case Accion.ataqueBase:
                return controlador.getCasillaAt(getCoordenadasBaseEnemiga());
            case Accion.defensaBase:
                return controlador.getCasillaAt(baseUnidad.getPosicionGrid());
            default:
                break;
        }
        return null;
    }


    private CasillaGrid puntoCuracion()
    {
        if (bando)
            return getCoordenadasTipoRallyPoint(TipoRallyPoint.baseA);
        else
            return getCoordenadasTipoRallyPoint(TipoRallyPoint.baseB);
    }

    private CasillaGrid eligePatrulla()
    {
        List<CasillaGrid> listaPuntosPatrulla = patrullas[numeroPatrulla];
        patrullando = true;
        return listaPuntosPatrulla[indicePatrulla];
    }

    public Unidad getEnemigoMasCercano()
    {
        float distanciaMinima = Mathf.Infinity;
        float distanciaActual = 0;

        Unidad enemigoMasCercano = null;

        //TODO: comprobar si está vivo
        foreach (Unidad e in enemigos)
        {
            distanciaActual = Vector2.Distance(e.getCoordenadas(), this.getCoordenadas());
            if (distanciaActual < distanciaMinima)
            {
                distanciaMinima = distanciaActual;
                enemigoMasCercano = e;
            }
        }
        return enemigoMasCercano;
    }

    public RallyPoint cercaRallyPoint(TipoRallyPoint tipo)
    {
        RallyPoint rpObjetivo = null;
        float distanciaMasCercana = Mathf.Infinity;
        float distancia;

        foreach (RallyPoint rp in rallypoints)
        {
            distancia = Vector2.Distance(coordenadas, rp.getCoordenadas());
            if (rp.getTipoRallyPoint() == tipo && distancia < rangoVision*10 && distancia < distanciaMasCercana)
            {
                rpObjetivo = rp;
                distanciaMasCercana = distancia;
            }
        }

        return rpObjetivo;
    }


    /**
    * Metodo para determinar la influencia en un area del mapa alrededor de una unidad. Se tomara como referencia
    * el rango de vision de la unidad para realizar el cálculo.
    **/
    public float influenciaCercana()
    {

        Vector2 coordenadasUnidad = getCoordenadas();
        int ancho = (int)controlador.getDim();
        int alto = (int)controlador.getDim();
        float distancia = 0;
        float influencia = 0;

        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                distancia = Vector2.Distance(coordenadasUnidad, new Vector2(j, i));
                if (distancia < rangoVision)
                    influencia += controlador.getInfluenciaAt(j, i);
            }
        }

        return influencia;
    }


    public void modificarSalud(float delta)
    {
        saludActual += (int)Mathf.Round(delta);
        if (saludActual <= 0)
        {
            saludActual = 0;
        }
        if (saludActual > saludTotal)
            saludActual = saludTotal;

        if (saludActual == saludTotal)
        {
            unidadViva = true;
            GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            modoOfensivo = false;
            modoDefensivo = true;
        }

        if (saludActual == 0)
        {
            unidadViva = false;
            GetComponent<MeshRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.position = controlador.getCasillaAt((int)getCoordenadasBase().x, (int)getCoordenadasBase().y).getPosicion();
        }
    }


    /**
        * A* y métodos asociados
        * */
    
        
    protected List<NodoA> AStar(CasillaGrid actual, CasillaGrid goal, Dictionary<TipoCasilla, float> moveIndex)
    {

        //List<NodoA> cerrados = new List<NodoA>();
        //List<NodoA> abiertos = new List<NodoA>();
        Dictionary<Vector2,NodoA> cerrados = new Dictionary<Vector2, NodoA>();
        Dictionary<Vector2,NodoA> abiertos = new Dictionary<Vector2, NodoA>();
        CasillaGrid target = goal;
        NodoA starter = new NodoA(actual,null, 0);

        abiertos.Add(starter.getCasilla().getPosicionGrid(),starter);
        
        while (abiertos.Count != 0)
        {
            NodoA bestNode = GetLowestCostNode(abiertos);

            abiertos.Remove(bestNode.getCasilla().getPosicionGrid());
            cerrados.Add(bestNode.getCasilla().getPosicionGrid(),bestNode);


            if (bestNode.getCasilla().getPosicionGrid() == goal.getPosicionGrid())
            {
                ICollection<NodoA> camino = cerrados.Values;
                return ReconstructPath(camino);
            }
                


            List<NodoA> vecinos = Neighbours(bestNode.getCasilla(), moveIndex);
            foreach (NodoA n in vecinos)
            {

                if (cerrados.ContainsKey(n.getCasilla().getPosicionGrid()))
                    continue;

                float costeNodo =  bestNode.g + EuclideanDistance(bestNode.getCasilla().getPosicionGrid(), n.getCasilla().getPosicionGrid()) + moveIndex[n.getCasilla().getTipoCasilla()];
                n.g = +costeNodo;
                n.h = EuclideanDistance(n.getCasilla().getPosicionGrid(), goal.getPosicionGrid());
                n.setPadre(bestNode.getCasilla());

                
                if (abiertos.ContainsKey(n.getCasilla().getPosicionGrid()))
                        continue;

                abiertos.Add(n.getCasilla().getPosicionGrid(),n);
  
            }
        }
        return null;
    }
    


    public NodoA checkPosition(NodoA nodo, List<NodoA> nodos)
    {
        foreach (NodoA n in nodos)
        {

            if (n.getCasilla().getPosicionGrid()
                == nodo.getCasilla().getPosicionGrid())
                return n;  
        }

        return null;
    }

    public bool compareNode(List<NodoA> listaNodo, NodoA actual)
    {
        bool cEq = false;
        bool pEq = false;
        float dC = 0;
        float dP = 0;
        if (listaNodo.Count == 0)
            return false;
        foreach (NodoA n in listaNodo)
        {
            dC = calcDist(n.getCasilla(), actual.getCasilla());
            if (dC == 0)
                cEq = true;
            dP = calcDist(n.getPadre(), actual.getPadre());
            if ( dP == 0)
                pEq = true;

            if (cEq && pEq)
                return true;    
        }
            
        
            return false;
    }

    public List<NodoA> Neighbours(CasillaGrid casilla, Dictionary<TipoCasilla, float> moveIndex)
    {
        NodoA aux = null;
        List<NodoA> listNodes = new List<NodoA>();
        List<CasillaGrid> adyacentes = casilla.getAdyacentes();

        foreach (CasillaGrid c in adyacentes)
        {
            if (moveIndex[c.getTipoCasilla()] == Mathf.Infinity)
                    continue;
            aux = new NodoA(c, casilla, moveIndex[c.getTipoCasilla()]);
            listNodes.Add(aux);
        }

        return listNodes;
    }

    public NodoA GetLowestCostNode(Dictionary<Vector2,NodoA> diccionario)
    {
        NodoA nodoCosteMinimo = null;
        float costeMinimo = Mathf.Infinity;
        ICollection<NodoA> nodos = diccionario.Values;
        

        foreach(NodoA nodoActual in nodos)
        {
            if (nodoActual.f() < costeMinimo)
            {
                costeMinimo = nodoActual.f();
                nodoCosteMinimo = nodoActual;
            }
        }
        return nodoCosteMinimo;

    }


    public NodoA GetLowest(List<NodoA> lista)
    {
        NodoA best = null;
        float lowestCost = Mathf.Infinity;

        foreach (NodoA n in lista)
        {
            if (n.f() < lowestCost)
            {
                best = n;
                lowestCost = n.f();
            }

        }
        return best;
    }

    public float EuclideanDistance(Vector2 origen, Vector2 destino)
    {
        return (Mathf.Sqrt(Mathf.Pow((origen.x - destino.x), 2) + Mathf.Pow((origen.y - destino.y), 2)));
    }

    public List<NodoA> ReconstructPath(ICollection<NodoA> lista)
    {
        List<NodoA> path = new List<NodoA>();
        List<NodoA> aux = new List<NodoA>(lista);
        aux.Reverse();
        CasillaGrid padre = null;
        if (lista==null || lista.Count==0)
            return null;
       

        foreach (NodoA current in aux)
        {

            if (current.getPadre() != null)
            {
                if (path.Count == 0)
                {
                    path.Add(current);
                    padre = current.getPadre();
                }
                else if (current.getCasilla() == padre)
                {
                    path.Add(current);
                    padre = current.getPadre();
                }

            }
            else
                path.Add(current);


        }
        path.Reverse();
        return path;
    }
    
    public float calcDist(CasillaGrid origen, CasillaGrid fin)
    {
        return Mathf.Sqrt(Mathf.Pow((origen.getPosicionGrid().x - fin.getPosicionGrid().x), 2) + Mathf.Pow((origen.getPosicionGrid().y - fin.getPosicionGrid().y), 2));
    }

    public float calcDist(Vector3 origen, CasillaGrid fin)
    {
        return Mathf.Sqrt(Mathf.Pow((origen.x - fin.getPosicionGrid().x), 2) + Mathf.Pow((origen.y - fin.getPosicionGrid().y), 2));
    }
    
    public float getInfluenciaBase()
    {
        return controlador.getInfluenciaZona(getCoordenadasBase());
    }

    public Vector2 getCoordenadasBase()
    {
        int i = 0;
        int size = rallypoints.Count;
        while (i < size)
        {
            if (bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseA)
                return rallypoints[i].getCoordenadas();

            if (!bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseB)
                return rallypoints[i].getCoordenadas();

            i++;
        }

        return Vector2.zero;
    }

    public Vector2 getCoordenadasBaseEnemiga()
    {
        int i = 0;
        int size = rallypoints.Count;
        while (i < size)
        {
            if (bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseB)
                return rallypoints[i].getCoordenadas();

            if (!bando && rallypoints[i].getTipoRallyPoint() == TipoRallyPoint.baseA)
                return rallypoints[i].getCoordenadas();

            i++;
        }

        return Vector2.zero;
    }

    public CasillaGrid RPDesprotegido()
    {
        int coordenadasRPx, coordenadasRPy;
        float influenciaRP;


        foreach (RallyPoint rp in rallypoints)
        {
            coordenadasRPx = (int)rp.getCoordenadas().x;
            coordenadasRPy = (int)rp.getCoordenadas().y;
            influenciaRP = controlador.getInfluenciaAt(coordenadasRPx, coordenadasRPy);

            if (bando && influenciaRP < 0 && influenciaRP >= -limiteInfluencia)
                return controlador.getCasillaAt(coordenadasRPx, coordenadasRPy);
            if (!bando && influenciaRP > 0 && influenciaRP <= limiteInfluencia)
                return controlador.getCasillaAt(coordenadasRPx, coordenadasRPy);
        }

        // Proteccion en caso de no encontrar el punto deseado. Ataque la base enemiga
        if (bando)
        {
            coordenadasRPx = (int)getCoordenadasTipoRallyPoint(TipoRallyPoint.baseB).getPosicionGrid().x;
            coordenadasRPy = (int)getCoordenadasTipoRallyPoint(TipoRallyPoint.baseB).getPosicionGrid().x;
            return controlador.getCasillaAt(coordenadasRPx, coordenadasRPy);
        }
        else
        {
            coordenadasRPx = (int)getCoordenadasTipoRallyPoint(TipoRallyPoint.baseA).getPosicionGrid().x;
            coordenadasRPy = (int)getCoordenadasTipoRallyPoint(TipoRallyPoint.baseA).getPosicionGrid().x;
            return controlador.getCasillaAt(coordenadasRPx, coordenadasRPy);
        }
    }



    public CasillaGrid getCoordenadasTipoRallyPoint(TipoRallyPoint tipo)
    {
        int size = rallypoints.Count;
        Vector2 coord;
        for (int i = 0; i < size; i++)
        {
            coord = rallypoints[i].getCoordenadas();
            if (tipo == rallypoints[i].getTipoRallyPoint())
                return controlador.getCasillaAt((int)coord.x, (int)coord.y);
        }
        return null;
    }

    public void setSeleccionada(bool sel)
    {
        seleccionada = sel;
    }

    public float getInfluencia()
    {
        return influencia;
    }
    public bool getBando()
    {
        return bando;
    }

    public float getRangoAtaque()
    {
        return rangoAtaque;
    }
    public void setRangoAtaque(float aRange)
    {
        this.rangoAtaque = aRange;
    }

    public float getRangoAccion()
    {
        return rangoAccion;
    }
    public void setRangoAccion(float rango)
    {
        rangoAccion = rango;
    }

    public float getRangoVision()
    {
        return rangoVision;
    }
    public void setRangoVision(float vRange)
    {
        this.rangoVision = vRange;
    }

    public int getSaludTotal()
    {
        return saludTotal;
    }

    public void setSaludTotal(int health)
    {
        this.saludTotal = health;
    }

    public int getSaludActual()
    {
        return saludActual;
    }
    public void setSaludActual(int currentHealth)
    {
        this.saludActual = currentHealth;
    }

    public List<Unidad> getEnemigos()
    {
        return enemigos;
    }

    public bool getModoOfensivo()
    {
        return modoOfensivo;
    }
    public bool getModoDefensivo()
    {
        return modoDefensivo;
    }

    public Vector2 getCoordenadas()
    {
        return coordenadas;
    }
    public void setCoordenadas(int x, int y)
    {
        coordenadas = new Vector2(x, y);
    }

    public CasillaGrid getTarget()
    {
        return this.target;
    }
    public void setTarget(CasillaGrid objetivo)
    {
        this.target = objetivo;
    }

    public List<RallyPoint> getRallyPoints()
    {
        return rallypoints;
    }

    public bool isUnidadViva()
    {
        return unidadViva;
    }

    public bool isBatallaTotal()
    {
        return batallaTotal;
    }

    public void setBatallaTotal(bool value)
    {
        batallaTotal = value;
    }

}
