using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ControladorMapa : MonoBehaviour
{
    public CasillaGrid[,] casillas;

    public GameObject selected;
    public Tilemap tilemap;
    public Tilemap detalles;
    public Tilemap influence;
    public Unidad unidadSeleccionada;
    CasillaGrid baseA, baseB;
    public int ratioCuracion = 5;
    private bool enfriamientoCura = false;

    // Array bidimensional que representa la influencia de los bandos en el mapa. Un valor positio en una posicion indica una mayor influencia del bando del jugador.
    // Un valor negativo indica una mayor influencia de su rival.
    private float[,] mapaInfluencia;
    private int dim = 41;

    public List<Unidad> unidades;
    private bool refreshMap = true;

    public float tiempoCaptura;
    float tiempoBaseA, tiempoBaseB;
    public GameObject panelVictoria, victoriaAzul, victoriaRoja;

    // Start is called before the first frame update
    void Awake()
    {
        //tilemap = GameObject.FindGameObjectWithTag("TileGrid").GetComponent<Tilemap>();
        Vector3 posicionGlobal = Vector3.zero;
        casillas = new CasillaGrid[dim, dim];
        mapaInfluencia = new float[dim, dim];
        
        for (int i = 0; i < dim; i++)
        {
            for(int j=0; j< dim; j++)
            {
                posicionGlobal = new Vector3((float)0.5*j + (float)0.5*i,(float)-0.25*j + (float)0.25*i,0);
                casillas[i, j] = new CasillaGrid(posicionGlobal,new Vector2(i, j));
                
            }
        }
        getName();
        inicializarAdyacentes();

        baseA = getCasillaAt(6,2);
        baseB = getCasillaAt(37,36);

        tiempoBaseA = 0;
        tiempoBaseB = 0;
    }

    private int convertI(int i)
    {
        return Mathf.Abs(i + 1);
    }

    private int convertJ(int j)
    {
        return Mathf.Abs(j+2);
    }

    void Update()
    {
        if (refreshMap)
        {
            //actualizaCasillaUnidades();
            actualizaMapa();
            StartCoroutine(DibujarMapaInfluencia());

            // Limites para encontrar todo el grid de influencia:
            // i = [-3,40]
            // j = [0,-43]
            for (int i = -3; i < 40; i++)
            {
                for (int j = 0; j > -43; j--)
                {
                    if (mapaInfluencia[convertI(i), convertJ(j)] > 0)
                    {
                        SetTileColour(new Color(0, 0, 255, 0.2f), new Vector3Int(i, j, 4), influence);
                    }
                    else if (mapaInfluencia[convertI(i), convertJ(j)] < 0)
                    {
                        SetTileColour(new Color(255, 0, 0, 0.2f), new Vector3Int(i, j, 4), influence);
                    }
                    else SetTileColour(new Color(0, 0, 0, 0), new Vector3Int(i, j, 4), influence);
                }
            }
        }

       activarVictoria(comprobarVictoria());

        seleccionarUnidad();


        if(!enfriamientoCura)
            StartCoroutine(curacion());

    }

    public void activarModoDefensivo()
    {
        unidadSeleccionada.modoOfensivo = false;
        unidadSeleccionada.modoDefensivo = true;
    }

    public void activarModoOfensivo()
    {
        unidadSeleccionada.modoDefensivo = false;
        unidadSeleccionada.modoOfensivo = true;
    }
    private void activarVictoria(int victoria)
    {
        switch (victoria)
        {
            case 1:
                panelVictoria.SetActive(true);
                victoriaRoja.SetActive(false);
                return;
            case 2:
                panelVictoria.SetActive(true);
                victoriaAzul.SetActive(false);
                return;
            default:
                return;
        }
    }

    private int comprobarVictoria()
    {
        int victoria = 0;

        if (getInfluenciaAt(baseB) > 0)
        {
            tiempoBaseB += Time.deltaTime;
            if (tiempoBaseB >= tiempoCaptura)
                victoria = 1;
        }
        else tiempoBaseB = 0;

        if (getInfluenciaAt(baseA) < 0)
        {
            tiempoBaseA += Time.deltaTime;
            if (tiempoBaseA >= tiempoCaptura)
                victoria = 2;
        }
        else tiempoBaseA = 0;

        return victoria;
    }

    private void seleccionarUnidad()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (Unidad u in unidades)
                u.setSeleccionada(false);
            selected.GetComponent<MeshRenderer>().enabled = false;
            bool unidadEncontrada = false;
            int i = 0;
            int size = unidades.Count;
            CasillaGrid casillaObjetivo = getCasillaRaton();

            if (casillaObjetivo == null)
                return;

            while(i<size && !unidadEncontrada)
            {
                if (unidades[i].getCoordenadas() == casillaObjetivo.getPosicionGrid())
                {
                    unidadEncontrada = true;
                    unidades[i].setSeleccionada(true);
                    unidadSeleccionada = unidades[i];
                    return;

                }
                i++;
            }

        }
        
    }

    public void activarBatallaTotal()
    {
        foreach (Unidad u in unidades)
            u.setBatallaTotal(true);
    }

    IEnumerator curacion()
    {
        enfriamientoCura = true;
        yield return new WaitForSeconds(1);
        curaUnidades();
        enfriamientoCura = false;
    }

    IEnumerator DibujarMapaInfluencia()
    {
        refreshMap = false;
        yield return new WaitForSeconds(0.5f);
        refreshMap = true;
    }

    private void actualizaCasillaUnidades()
    {
        CasillaGrid cgu;

        foreach (Unidad u in unidades)
        {
            cgu = getCasillaInfluencia(u);
        }
    }

    private void actualizaMapa()
    {
        float[,] nuevoMapa = new float[dim, dim];
        Vector2 coordenadasCasilla = Vector2.zero;
        float rangoVision;
        float distancia;

        // Para cada unidad, calculamos la influencia que ejerce sobre sus casillas cercanas
        foreach (Unidad u in unidades)
        {
            // Influirá en casillas que caigan dentro de su rango de vision
            rangoVision = u.getRangoVision();
            // Para cada casilla, si cae dentro de ese radio
            foreach (CasillaGrid cg in casillas)
            {
                coordenadasCasilla = cg.getPosicionGrid();
                distancia = Vector2.Distance(coordenadasCasilla, u.getCoordenadas());
                if (distancia < rangoVision)
                {
                    // TO DO. SOLO LAS UNIDADES VIVAS INFLUYEN EN EL MAPA
                    // Si es del bando del jugador, la influencia incrementa. Si es del rival, disminuye
                    if (u.getBando())
                    {
                        nuevoMapa[(int)coordenadasCasilla.x, (int)coordenadasCasilla.y] += Mathf.Clamp(u.getInfluencia() / distancia, 1, 5);
                        //Debug.Log("casilla bando: " + coordenadasCasilla + " - " + mapaInfluencia[(int)coordenadasCasilla.x, (int)coordenadasCasilla.y]);
                    }
                    else
                    {
                        nuevoMapa[(int)coordenadasCasilla.x, (int)coordenadasCasilla.y] -= Mathf.Clamp(u.getInfluencia() / distancia, 1, 5);
                        //Debug.Log("casilla bandoE: " + coordenadasCasilla + " - " + mapaInfluencia[(int)coordenadasCasilla.x, (int)coordenadasCasilla.y]);
                    }
                }
            }
        }
        mapaInfluencia = nuevoMapa;
    }

    public void getName()
    {
        Tilemap tiles = GameObject.FindGameObjectWithTag("TileGrid").GetComponentInChildren<Tilemap>();
        tiles.CompressBounds();

        BoundsInt bounds = tilemap.cellBounds;

        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x =0; x < bounds.size.x; x++){
            for (int y = bounds.size.y -1; y > 0; y--)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                { 
                    setType(tile.name, x, bounds.size.y - y);
                    
                }
                else
                    print("Casilla " + x + "," + y + " no encontrada");
            }
        }

    }

    public void setType(String nombre, int x, int y)
    {
        if (nombre == "g4591" || nombre == "g4599")
            casillas[x, y].setTipoCasilla(TipoCasilla.pradera);

        else if (nombre == "g4776")
            casillas[x, y].setTipoCasilla(TipoCasilla.carretera);

        else if (nombre == "g4712")
            casillas[x, y].setTipoCasilla(TipoCasilla.bosque);

        else if (nombre == "g4629")
            casillas[x, y].setTipoCasilla(TipoCasilla.montaña);

        else if (nombre == "g7104")
            casillas[x, y].setTipoCasilla(TipoCasilla.rio);

        else if (nombre == "g4617" || nombre == "g7128")
            casillas[x, y].setTipoCasilla(TipoCasilla.safepoint);

        else if (nombre == "g4529")
            casillas[x, y].setTipoCasilla(TipoCasilla.ciudad);

        //print("PosX = " + x + ", PosY = " + y +  " Tipo = " + casillas[x,y].getTipoCasilla() + " Nombre tile = " + nombre + " Pos casilla = " + casillas[x,y].getPosicionGrid());
    }

    public CasillaGrid getCasillaRaton()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)),
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) + Vector3.forward * 288);
        if (hit.collider != null)
            return getCasillaObjetivo(hit.point);

        else return null;
    }

    public CasillaGrid getCasillaActual(Unidad u)
    {
        RaycastHit2D hit = Physics2D.Raycast(u.transform.position, new Vector3(u.transform.position.x, u.transform.position.y, 10) + Vector3.forward * 288);
        if (hit.collider != null)
            return getCasillaObjetivo(hit.point);

        else return null;
    }



    public CasillaGrid getCasillaInfluencia(Unidad u)
    {
        RaycastHit2D hit = Physics2D.Raycast(u.transform.position + new Vector3(0,-1,0),  Vector3.forward * 288);
        
        if (hit.collider != null)
            return getCasillaObjetivo(hit.point);

        else return null;
    }

    private CasillaGrid getCasillaObjetivo(Vector2 posicionImpacto)
    {
        float distanciaMinima = Mathf.Infinity;
        float distancia;
        Vector3 posicionImpactoMundo = new Vector3(posicionImpacto.x, posicionImpacto.y, 0);
        CasillaGrid objetivo = null;

        foreach (CasillaGrid c in casillas)
        {
            distancia = (c.getPosicion() - posicionImpactoMundo).magnitude;
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetivo = c;
            }
        }
        return objetivo;
    }

    private List<CasillaGrid> getAdyacentes(Vector2 coordenadas)
    {
        return getCasillaAt((int)coordenadas.x, (int)coordenadas.y).getAdyacentes();
    }


    public CasillaGrid getCasillaAt(Vector2 v)
    {
        return getCasillaAt((int)v.x, (int)v.y);
    }
    public CasillaGrid getCasillaAt(int x,int y)
    {
        return casillas[x,y];
    }

    public void inicializarAdyacentes()
    {
        Vector2 posicionGrid;

        foreach (CasillaGrid c in casillas)
        {
            posicionGrid = c.getPosicionGrid();

            bool x0 = (posicionGrid.x == 0);
            bool y0 = (posicionGrid.y == 0);
            bool xLargo = (posicionGrid.x == dim - 1);
            bool yAncho = (posicionGrid.y == dim - 1);


            if ((!x0 && !y0) && (!xLargo && !yAncho) && (!x0 && !yAncho) && (!xLargo && !y0) && (!x0) && (!y0) && (!xLargo) && (!yAncho))
            {
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y + 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y - 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y + 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y - 1));
            }

            else if (x0)
            {
                if (y0)
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));

                }
                else if (yAncho)
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y - 1));

                }
                else
                {

                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y - 1));

                }
            }
            else if (y0)
            {
                if (xLargo)
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));
                }
                else
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y + 1));
                }
            }
            else if (xLargo)
            {
                if (yAncho)
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y - 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                }
                else
                {
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y - 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y + 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                    c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y + 1));

                }
            }
            else if (yAncho)
            {
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x - 1, (int)posicionGrid.y - 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x, (int)posicionGrid.y - 1));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y));
                c.AddAdyacente(getCasillaAt((int)posicionGrid.x + 1, (int)posicionGrid.y - 1));
            }

        }

    }


    public void curaUnidades()
    {
        foreach(Unidad u in unidades)
        {
            if (u.getBando() && u.getCoordenadas().Equals(baseA.getPosicionGrid()))
                u.modificarSalud(ratioCuracion);

            else if (u.getCoordenadas().Equals(baseB.getPosicionGrid()))
                u.modificarSalud(ratioCuracion);

        }
    }

    /// <summary>
    /// Set the colour of a tile.
    /// </summary>
    /// <param name="colour">The desired colour.</param>
    /// <param name="position">The position of the tile.</param>
    /// <param name="tilemap">The tilemap the tile belongs to.</param>
    private void SetTileColour(Color colour, Vector3Int position, Tilemap map)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        map.SetTileFlags(position, TileFlags.None);
        // Set the colour.
        map.SetColor(position, colour);
    }


    public float getInfluenciaZona(Vector2 coordenadaZona)
    {
        float influenciaZona = 0;
        float rangoInfluencia = 8;

        for (int i = 0; i< dim; i++)
            for (int j = 0; j<dim; j++)
            {
                if (Vector2.Distance(coordenadaZona, new Vector2(i, j)) < rangoInfluencia)
                    influenciaZona += mapaInfluencia[i, j];
            }

        return influenciaZona;
    }

    public float getInfluenciaAt(Vector2 posicionGrid)
    {
        return getInfluenciaAt((int)posicionGrid.x, (int)posicionGrid.y);
    }

    public float getInfluenciaAt(CasillaGrid cg)
    {
        return getInfluenciaAt(cg.getPosicionGrid());
    }

    public float getInfluenciaAt(int i, int j)
    {
        return mapaInfluencia[i, j];
    }

    public float[,] getMapaInfluencia()
    {
        return this.mapaInfluencia;
    }

    public float getDim()
    {
        return this.dim;
    }

}
