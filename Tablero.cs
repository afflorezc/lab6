using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_Ajedrez
{
    /*
     * Clase que abstrae el comportamiento del tablero de juego, se inicia definiendo un objeto
     * de tipo casilla cuyas propiedades basicas son su color y la ficha que la ocupa. En caso
     * de estar libre, la ficha no se ha asignado
     */
    internal class Casilla
    {
        public Color color { get; set; }
        public Ficha ficha { get; set; }

        public Casilla(Color color)
        {
            this.color = color;
        }
    }
    /*
     * Clase que representa el tablero de juego, este es basicamente una matriz de casillas
     * y las operaciones alteraran la propiedad ficha de cada una de las casillas, pues el color
     * no cambia
     */
    internal class Tablero
    {
        public Casilla[,] posiciones = new Casilla[8,8];
        /*
         * El objeto Tablero se inicializa con la posición de las fichas en el tablero 
         * correspondiente a una nueva partida
         */
        public Tablero()
        {
            for(int i = 0; i < posiciones.GetLength(0); i++)
            {
                for(int j=0; j < posiciones.GetLength(1); j++)
                {
                    // Se crea una casilla auxiliar y se asigna su color
                    Casilla casillaAux= new Casilla(Color.Negro);
                    PosicionMatriz posicion = new PosicionMatriz(i, j);
                    if(i % 2 == 0)
                    {
                        if(j % 2 == 0)
                        {
                            casillaAux.color = Color.Blanco;
                        }
                    }
                    else
                    {
                        if(j %2 == 1)
                        {
                            casillaAux.color= Color.Blanco;
                        }
                    }
                    // Se asigna una ficha a la casilla creada
                    casillaAux.ficha = asignarFicha(posicion);
                    // Se añade la casilla a la matriz de casillas que representan 
                    // el tablero
                    posiciones[i, j] = casillaAux;
                }
            }
        }
        /*
         * Método que asigna (retorna) una ficha a una posicion dada en el tablero. Este método
         * hace una asignación de acuerdo a las posiciones inciales de juego
         */
        public Ficha asignarFicha(PosicionMatriz posicion)
        {
            int fila = posicion.fila;
            Color color = Color.Negro;
            switch (fila)
            {
                // Ubica las fichas negras en el tablero
                case 0:
                    // Ubica las fichas principales de color negro en el tablero
                    return fichaPpal(color, posicion);
                case 1:
                    // Ubica los peones negros
                    return new Peon(color, posicion);

                // Ubica las fichas blancas en el tablero
                case 7:
                    color = Color.Blanco;
                    // se ubican los peones blancos
                    return new Peon(color, posicion);

                case 8:
                    color = Color.Blanco;
                    // Se ubican las fichas principales de color blanco del juego
                    return fichaPpal(color, posicion);
                default: 
                    // Las demás casillas son vacias en la posicion inicial de juego
                    return null;
            }          
        } 
        /*
         * Método que asigna un tipo apropiado de ficha (incluyendo su color) de acuerdo a
         * una posicion en el tablero que corresponde a la fila superior o la fila inferior
         */
        public Ficha fichaPpal(Color color, PosicionMatriz posicion)
        {
            int columna = posicion.columna;
            // Posiciones de las torres
            if (columna == 0 || columna == 7)
            {
                return new Torre(color, posicion);
            }
            // Posiciones de los caballos
            else if (columna == 1 || columna == 6)
            {
                return new Caballo(color, posicion);
            }
            // Posiciones de alfiles
            else if (columna == 2 || columna == 5)
            {
                return new Alfil(color, posicion);
            }
            // Posicion reina
            else if(columna == 3)
            {
                return new Reina(color, posicion);
            }
            // Posicion rey
            else
            {
                return new Rey(color, posicion);
            }
        }


    }
}
