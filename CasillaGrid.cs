using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoCasilla
{ pradera, bosque, carretera, rio, montaña, safepoint, ciudad }

public class CasillaGrid
{
        private TipoCasilla tipo;
        private Vector3 posicion;
        private Vector2 coordenadas;
        private List<CasillaGrid> adyacentes;

        public CasillaGrid (Vector3 posicionGlobal, Vector2 coord)
        {
            adyacentes = new List<CasillaGrid>();
            posicion = posicionGlobal;
            coordenadas = coord;
        }
        
        public CasillaGrid (CasillaGrid casilla)
        {
            posicion = casilla.getPosicion();
            coordenadas = casilla.getPosicionGrid();
            adyacentes = casilla.getAdyacentes();
            tipo = casilla.getTipoCasilla();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public Vector3 getPosicion()
        {
            return posicion;
        }

        public Vector2 getPosicionGrid()
        {
            return coordenadas;
        }


        public TipoCasilla getTipoCasilla()
        {
            return tipo;
        }

        public void setTipoCasilla(TipoCasilla tipoCasilla)
        {
            tipo = tipoCasilla;
        }

        public void AddAdyacente(CasillaGrid casilla)
        {
            adyacentes.Add(casilla);
        }

        public List<CasillaGrid> getAdyacentes()
        {
            return adyacentes;
        }

        


}
