using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public Image imagen { get; set; }

        public bool marcada { get; set; }

        /*
         * Costructor básico del objeto casilla que asigna su propiedad de color
         */
        public Casilla(Color color)
        {
            this.color = color;
            this.marcada = false;
        }
        /*
         * Método que hace una copia de una casilla de modo que no se referencie y el nuevo
         * objeto pueda ser cambiado sin afectar el original
         */
        public void copiarCasilla(Casilla casilla)
        {
            this.color = casilla.color;
            this.marcada = casilla.marcada;
            this.imagen = casilla.imagen;
            
            if (casilla.ficha != null)
            {
                Ficha ficha = new Ficha(casilla.ficha.color, casilla.ficha.posicion);
                casilla.ficha.copiarFicha(ref ficha);
                this.ficha = ficha;
            }
            else
            {
                this.ficha = null;
            }

        }
        /*
         * Método que asigna la imagen apropiada para un peon en la casilla
         */
        public void imagenPeon(Peon peon)
        {
            switch (peon.color)
            {
                case Color.Blanco:
                    if(this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._3a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._3b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._4a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._4b;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._5a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._5b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._6a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._6b;
                        }
                    }
                    break;
            }
        }

        /*
         * Método que asigna la imagen apropiada para una torre
         */
        public void imagenTorre(Torre torre)
        {
            switch (torre.color)
            {
                case Color.Blanco:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._7a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._7b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._8a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._8b;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._9a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._9b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._10a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._10b;
                        }
                    }
                    break;
            }
        }

        /*
         * Método que asigna la imagen apropiada para un caballo en la casilla
         */
        public void imagenCaballo(Caballo caballo)
        {
            switch (caballo.color)
            {
                case Color.Blanco:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._11a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._11b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._12a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._12b;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._13a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._13b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._14a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._14b;
                        }
                    }
                    break;
            }
        }
        /*
         * Método que asigna la imagen apropiada para un Alfil en la casilla
         */
        public void imagenAlfil(Alfil alfil)
        {
            switch (alfil.color)
            {
                case Color.Blanco:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._15a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._15b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._16a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._16b;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._17a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._17b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._18a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._18b;
                        }
                    }
                    break;
            }
        }
        /*
         * Método que asigna la imagen apropiada para una reina en la casilla
         */
        public void imagenReina(Reina reina)
        {
            switch (reina.color)
            {
                case Color.Blanco:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._19a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._19b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._20a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._20b;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._21a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._21b;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._22a;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._22b;
                        }
                    }
                    break;
            }
        }

        /*
         * Método que asigna la imagen apropiada para un rey en la casilla
         */
        public void imagenRey(Rey rey)
        {
            switch (rey.color)
            {
                case Color.Blanco:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._23a;
                        }
                        else if(!rey.enJaque)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._23b;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._23c;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._24a;
                        }
                        else if(!rey.enJaque)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._24b;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._24c;
                        }
                    }
                    break;
                case Color.Negro:
                    if (this.color == Color.Blanco)
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._25a;
                        }
                        else if(!rey.enJaque)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._25b;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._25c;
                        }
                    }
                    else
                    {
                        if (!this.marcada)
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._26a;
                        }
                        else if (!rey.enJaque) 
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._26b;
                        }
                        else
                        {
                            this.imagen = global::Lab6_Ajedrez.Properties.Resources._26c;
                        }
                    }
                    break;
            }
        }
        /*
         * Método que asigna la imagen apropiada desde los recursos de acuerdo al tipo
         * de ficha contenida en esta, su color, y el color de la casilla
         */
        public void asignarImagen()
        {
            if(this.ficha == null)
            {
                if (this.color == Color.Blanco)
                {
                    if (!marcada)
                    {
                        this.imagen = global::Lab6_Ajedrez.Properties.Resources._1a;
                    }
                    else
                    {
                        this.imagen = global::Lab6_Ajedrez.Properties.Resources._1b;
                    }
                }
                else
                {
                    if (!marcada)
                    {
                        this.imagen = global::Lab6_Ajedrez.Properties.Resources._2a;
                    }
                    else
                    {
                        this.imagen = global::Lab6_Ajedrez.Properties.Resources._2b;
                    }
                }
                return;
            }
            switch (this.ficha.tipo)
            {
                case TipoFicha.Peon:
                    Peon peon =(Peon)this.ficha;
                    imagenPeon(peon);
                    break;

                case TipoFicha.Torre:
                    Torre torre = (Torre)this.ficha;
                    imagenTorre(torre);
                    break;

                case TipoFicha.Caballo:
                    Caballo caballo = (Caballo)this.ficha;
                    imagenCaballo(caballo);
                    break;

                case TipoFicha.Alfil:
                    Alfil alfil = (Alfil)this.ficha;
                    imagenAlfil(alfil);
                    break;

                case TipoFicha.Reina:
                    Reina reina = (Reina)this.ficha;
                    imagenReina(reina);
                    break;

                case TipoFicha.Rey:
                    Rey rey = (Rey)this.ficha;
                    imagenRey(rey);
                    break;  

            }
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
        public PosicionMatriz posReyBlanco;
        public PosicionMatriz posReyNegro;
        /*
         * El objeto Tablero se inicializa con la posición de las fichas en el tablero 
         * correspondiente a una nueva partida
         */
        public Tablero()
        {
            // Se inicializan las posiciones de los reyes
            posReyBlanco = new PosicionMatriz(7, 4);
            posReyNegro = new PosicionMatriz(0, 4);
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
                    casillaAux.asignarImagen();
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
                case 6:
                    color = Color.Blanco;
                    // se ubican los peones blancos
                    return new Peon(color, posicion);

                case 7:
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

        /*
         * Método que hace una copia de la disposición actual del tablero de modo que se
         * crea un objeto nuevo y no una referencia a la posicion actual del tablero, para
         * poder hacer cambios en la copia sin que se afecte al tablero original
         */
        public void copiarTablero(Tablero tablero)
        {
            for (int i = 0; i < tablero.posiciones.GetLength(0); i++)
            {
                for (int j = 0; j < tablero.posiciones.GetLength(1); j++)
                {
                    Casilla casAux = tablero.posiciones[i, j];
                    Casilla casilla = new Casilla(casAux.color);
                    // Se realiza copia para que sea un nuevo objeto sin referenciar al original
                    casilla.copiarCasilla(casAux);
                    this.posiciones[i, j] = casilla;
                }
            }
        }

    }
}
