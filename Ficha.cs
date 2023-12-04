using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6_Ajedrez
{
    /*
     * Enumerador para definir los diferentes tipos de fichas
     */

    enum TipoFicha
    {
        Peon,
        Torre,
        Caballo,
        Alfil,
        Reina,
        Rey
    };
    /*
     * Enumerador para trabajar los colores de las fichas
     */
    enum Color
    {
        Blanco,
        Negro
    }
    /*
     * Clase abstracta que representa cualquier ficha en el tablero de ajedrez
     * Define funciones comunes a todas las fichas tales como el moverse hacia otra
     * posición y el desaparecer del tablero. El atributo principal de una ficha
     * es su posición en el tablero
     */
    internal class Ficha
    {
        public TipoFicha tipo { get; set; }
        public Color color { get; set; }
        public PosicionMatriz posicion { get; set; }
        public List<ParOrdenado> movimientos { get; set; } 

        public Ficha(Color color, PosicionMatriz posicion)
        {
            this.color = color;
            this.posicion = posicion;
            // Inicializacion de la lista de movimientos
            movimientos = new List<ParOrdenado>();
        }
        /*
         * Método que realiza una copia de una ficha a un nuevo objeto de tipo ficha
         * de modo que el nuevo objeto no sea una referencia y no afecte las propiedades de
         * la ficha original al cambiar
         */
        public virtual void copiarFicha(ref Ficha fichaCopia)
        {
        }

        /*
         * Comprobación si el movimiento en tablero, el cual es referenciado por el par ordenado
         * "movimiento" es una jugada valida. Esto se hace buscando en la lista de movimientos
         * posibles que se han asignado a la ficha el par ordenado dado como parametro y retorna
         * true en caso que se encuentre en la lista y false en caso contrario
         */
        public virtual bool esMovimientoVal(ParOrdenado movimiento)
        {
            // Se compara el movimiento con cada uno de los movimientos en la lista
            if(this.movimientos != null)
            {
                int n = movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        ParOrdenado parValido = movimientos[i];
                        if (parValido.esIgual(movimiento))
                        {
                            return true;
                        }
                    }
                }
            }
            // Si ha llegado hasta aqui es porque no ha encontrado el movimiento en la lista
            return false;
        }
        /*
         * Método que mueve una ficha de una posicion A hacia una posicion B en caso que el
         * movimiento corresponda a una jugada valida. Si se puede ejecutar el movimiento se
         * actualizan las casillas correspondientes del tablero La casilla A se le asigna ficha
         * nula pues esta queda vacia, y se asigna la ficha en la posición B. Los parametros
         * de entrada son el par ordenado que representa el movimiento y un objeto tipo 
         * tablero que registra el juego. Se retorna true en caso exitoso, en caso contrario
         * false
         */
        public virtual void mover(ParOrdenado movimiento, Tablero tablero)
        {
           if(esMovimientoVal(movimiento))
            {
                int filSal = movimiento.x.fila;
                int colSal = movimiento.x.columna;
                Ficha ficha = tablero.posiciones[filSal, colSal].ficha;
                tablero.posiciones[filSal, colSal].ficha = null;
                tablero.posiciones[filSal, colSal].marcada = false;
                tablero.posiciones[filSal, colSal].asignarImagen();
                int filLleg = movimiento.y.fila;
                int colLleg = movimiento.y.columna;
                Ficha fichaCap = tablero.posiciones[filLleg, colLleg].ficha;
                if(fichaCap != null)
                {
                    tablero.ultFichaCap = fichaCap;
                }
                else
                {
                    tablero.ultFichaCap = null;
                }
                tablero.posiciones[filLleg, colLleg].ficha = ficha;
                tablero.posiciones[filLleg, colLleg].marcada = false;
                tablero.posiciones[filLleg, colLleg].asignarImagen();
                this.posicion = new PosicionMatriz(filLleg, colLleg);
                // Se desmarcan las celdas de los movimientos posibles que habian
                int n = movimientos.Count;
                if(n > 0)
                {
                    for(int i = 0; i < n; i++)
                    {
                        PosicionMatriz casilla = movimientos[i].y;
                        int fila = casilla.fila;
                        int col = casilla.columna;
                        tablero.posiciones[fila, col].marcada = false;
                        tablero.posiciones[fila, col].asignarImagen();
                    }
                }
               
            }
        }

        /*
         * Método que agrega un movimiento posible a la lista de acuerdo a los parametros
         * enteros que representan las posiciones de llegada o destino de la ficha
         */
        public void agregarMov(int filaLleg, int colLleg)
        {
            PosicionMatriz lleg = new PosicionMatriz(filaLleg, colLleg);
            ParOrdenado mov = new ParOrdenado(posicion, lleg);
            // Se agrega a la lista
            movimientos.Add(mov);
        }
        /*
         * Método que elimina todos los elementos de la lista de movimientos posibles de una
         * ficha tras algun movimiento en el tablero pues esto cambia las condiciones de juego
         * cada jugada es una disposicion de fichas diferente en el tablero, en cada jugada
         * se evaluan movimientos nuevos
         */
        public virtual void limpiarMovimientos()
        {
            if(this.movimientos != null)
            {
                this.movimientos.Clear();
            }
        }

        /*
         * Método que evalua si el rey del mismo color al turno actual queda en jaque si
         * se moviese alguna de las piezas circundantes hacia cierta posicion. Se hace el 
         * estudio de acuerdo a la dispocision de las fichas que se encuentra según el 
         * tablero dado como parametro
         */
        public bool enJaqueTrasMover(Tablero tabReferencia, ParOrdenado mov)
        {
            // se efectua el movimiento deseado sobre el tablero de referencia
            bool result = false; 
            Ficha ficha =
                tabReferencia.posiciones[this.posicion.fila, this.posicion.columna].ficha;  
            if(ficha != null)
            {
                // Se agrega primero el movimiento para que se reconozca como valido
                ficha.movimientos.Add(mov);
                ficha.mover(mov, tabReferencia);
                // Se chequea si el rey queda en jaque
                PosicionMatriz posRey = tabReferencia.posReyBlanco;
                if (ficha.color == Color.Negro)
                {
                    posRey = tabReferencia.posReyNegro;
                }
                Rey rey =
                    (Rey)tabReferencia.posiciones[posRey.fila, posRey.columna].ficha;
                if (rey.quedaEnJaque(posRey.fila, posRey.columna, tabReferencia))
                {
                    result = true;
                }
                // Se hace una sola copia del tablero para evaluar todas las posiciones 
                // De movimiento de la ficha, se debe regresar a la posicion inicial
                ficha.limpiarMovimientos();
                tabReferencia.posiciones[mov.x.fila, mov.x.columna].ficha = ficha;
                if(tabReferencia.ultFichaCap != null)
                {
                    tabReferencia.posiciones[mov.y.fila, mov.y.columna].ficha =
                               tabReferencia.ultFichaCap;
                }
                ficha.limpiarMovimientos();
                return result;
            }
            return false;
        }

        /*
         * Método virtual que sera sobrecargado en las clases hrederas para encontrar los 
         * diferentes movimientos de una ficha según su posicion actual en el tablero y la
         * disposición actual del tablero que se recupera a partir del parametro de tipo Tablero
         */

        public virtual void encontrarMovimientos(Tablero tablero) 
        { 

        }

    }

    /*
     * SubClase que define las funciones principales de un peon
     */
    internal class Peon : Ficha
    {
        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Peon(Color color, PosicionMatriz posicion) : base(color, posicion) 
        {
            // Se adiciona el atributo de tipo ficha Peon
            this.tipo = TipoFicha.Peon;
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            fichaCopia = new Peon(this.color, this.posicion);
        }
        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            // Variables auxiliares
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            int fila = posicion.fila;
            int col = posicion.columna;
            int filaSuperior = 0;
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            // y la fila superior correspondiente
            Color colFichaCont = Color.Negro;
            if(this.color == Color.Negro)
            {
                filaSuperior = 7;
                colFichaCont = Color.Blanco;
            }
            // Se obtiene la posicion relativa entre la fila de la posicion actual
            // y la fila superior correspondiente
            int relPos = Math.Abs(fila - filaSuperior);
            if(this.color == Color.Negro)
            {
                fila++;
            }
            else
            {
                fila--;
            }
            Ficha ficha;
            // El peon solo encuentra movimientos mientras no halla llegado a la 
            // fila "superior" contraria    
            if (relPos > 0)
            {
                // Se comprueba si puede capturar fichas moviendo en las diagonales
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila, col - 1].ficha;
                    y = new PosicionMatriz(fila, col - 1);
                    mov = new ParOrdenado(x, y);
                    if (ficha != null)
                    {
                        if (ficha.color == colFichaCont && !enJaqueTrasMover(tabRef, mov))
                        {
                            // La casilla tiene una ficha contraria entonces puede
                            // mover y capturarla
                            base.agregarMov(fila, col - 1);
                        }
                    }
                }
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila, col + 1].ficha;
                    y = new PosicionMatriz(fila, col + 1);
                    mov = new ParOrdenado(x, y);
                    if (ficha != null)
                    {
                        if (ficha.color == colFichaCont && !enJaqueTrasMover(tabRef, mov))
                        {
                            // La casilla tiene una ficha contraria que se puede
                            // capturar
                            base.agregarMov(fila, col + 1);
                        }
                    }
                }
                // Se chequea si puede mover hacia la siguiente fila correspondiente
                ficha = tablero.posiciones[fila, col].ficha;
                y = new PosicionMatriz(fila, col);
                mov = new ParOrdenado(x, y);
                if (ficha == null && !enJaqueTrasMover(tabRef, mov))
                {
                    // Se podrá mover a dicha casilla si esta se encuentra vacia
                    base.agregarMov(fila, col);
                    // Si es el primer movimiento podria mover dos casillas adelante
                    // siempre y cuando se encuentre vacia, esto ocurre cuando la posicion
                    // relativa es 6
                    if (relPos == 6)
                    {
                        if(this.color == Color.Negro)
                        {
                            fila++;
                        }
                        else
                        {
                            fila--;
                        }
                        ficha = tablero.posiciones[fila, col].ficha;
                        y = new PosicionMatriz(fila, col);
                        mov = new ParOrdenado(x, y);
                        if (ficha == null && !enJaqueTrasMover(tabRef, mov))
                        {
                            base.agregarMov(fila, col);
                        }
                    }
                }
                
            }
          
        }
    }
    /*
     * SubClase que define las funciones principales para una torre
     */
    internal class Torre : Ficha
    {
        // atributos adicionales para controlar la posibilidad del movimiento de
        // enroque con el rey
        public bool haMovido;

        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Torre(Color color, PosicionMatriz posicion) : base(color, posicion)
        {
            // Se define además el tipo de ficha
            this.tipo = TipoFicha.Torre;
            this.haMovido = false;
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            Torre copia = new Torre(this.color, this.posicion);
            copia.haMovido = this.haMovido;
            fichaCopia = (Torre)copia;
        }

        /*
         * Método que evalua todas las casillas a las que puede mover una torre hacia la
         * derecha
         */
        public void buscarMovDerecha(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            while (col < 7 && ficha == null) 
            {
                col++;
                ficha = tablero.posiciones[fila, col].ficha;
                y = new PosicionMatriz(fila, col);
                mov = new ParOrdenado(x, y);
                if(ficha != null && ficha.color == this.color)
                {
                    return;
                }
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if(ficha == null && !jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
                else if (jaqueAlMover)
                {
                    return;
                }
            }
            // Al encontrar una ficha se comprueba si es del oponente y por lo tanto se
            // puede capturar
            if(ficha != null && ficha.color == colFichaCont)
            {
                y = new PosicionMatriz(ficha.posicion.fila, ficha.posicion.columna);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
            }
        }

        /*
         * Método que evalua todas las casillas a las que puede mover una torre hacia la
         * izquierda
         */
        public void buscarMovIzquierda(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            while (col > 0 && ficha == null)
            {
                col--;
                ficha = tablero.posiciones[fila, col].ficha;
                if (ficha != null && ficha.color == this.color)
                {
                    return;
                }
                y = new PosicionMatriz(fila, col);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (ficha == null && !jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
                else if (jaqueAlMover)
                {
                    return;
                }
            }
            // Al encontrar una ficha se comprueba si es del oponente y por lo tanto se
            // puede capturar
            if (ficha != null && ficha.color == colFichaCont)
            {
                y = new PosicionMatriz(ficha.posicion.fila, ficha.posicion.columna);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
                
            }
        }

        /*
         * Método que evalua todas las casillas a las que puede mover una torre hacia arriba
         */
        public void buscarMovArriba(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            while (fila > 0 && ficha == null)
            {
                fila--;
                ficha = tablero.posiciones[fila, col].ficha;
                if (ficha != null && ficha.color == this.color)
                {
                    return;
                }
                y = new PosicionMatriz(fila, col);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (ficha == null && !jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
                else if (jaqueAlMover)
                {
                    return;
                }
            }
            // Al encontrar una ficha se comprueba si es del oponente y por lo tanto se
            // puede capturar
            if (ficha != null && ficha.color == colFichaCont)
            {
                y = new PosicionMatriz(ficha.posicion.fila, ficha.posicion.columna);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
            }
        }

        /*
         * Método que evalua todas las casillas a las que puede mover una torre hacia arriba
         */
        public void buscarMovAbajo(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            while (fila < 7 && ficha == null)
            {
                fila++;
                ficha = tablero.posiciones[fila, col].ficha;
                if (ficha != null && ficha.color == this.color)
                {
                    return;
                }
                y = new PosicionMatriz(fila, col);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (ficha == null && !jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
                else if (jaqueAlMover)
                {
                    return;
                }
            }
            // Al encontrar una ficha se comprueba si es del oponente y por lo tanto se
            // puede capturar
            if (ficha != null && ficha.color == colFichaCont)
            {
                y = new PosicionMatriz(ficha.posicion.fila, ficha.posicion.columna);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    base.agregarMov(fila, col);
                }
            }
        }
        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            // Variables auxiliares
            int fila = posicion.fila;
            int col = posicion.columna;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            // Se encuentran las casillas hacia la derecha donde puede mover la torre
            buscarMovDerecha(tablero, fila, col, colFichaCont);
            // Se encuentran las casillas hacia la izquierda donde puede mover la torre
            buscarMovIzquierda(tablero, fila, col, colFichaCont);
            // Se encuentran las casillas hacia arriba donde puede mover la torre
            buscarMovArriba(tablero, fila, col, colFichaCont);
            // Se encuentran las casillas hacia abajo puede mover la torre
            buscarMovAbajo(tablero, fila, col, colFichaCont);
        }
    }
    /*
     * SubClase que define las funciones principales para un caballo
     */
    internal class Caballo : Ficha
    {
        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Caballo(Color color, PosicionMatriz posicion) : base(color, posicion) 
        {
            this.tipo = TipoFicha.Caballo;
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            fichaCopia = new Caballo(this.color, this.posicion);
        }
        /*
         * Método que evalua si el caballo puede moverse dos casillas hacia la derecha y luego
         * una hacia arriba o hacia abajo. Los parametros correspondientes son la fila y columna
         * de la posicion actual del caballo, y el color de la ficha contraria. Cada movimiento
         * posible encontrado es agregado a la lista de movimientos
         */
        public void buscarMovDerecha(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            if (col < 6)
            {
                //mover luego hacia abajo
                if (fila < 7)
                {
                    ficha = tablero.posiciones[fila + 1, col + 2].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila + 1, col + 2);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila + 1, col + 2);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila + 1, col + 2);
                            }
                        }
                    }   
                   
                }
                // mover luego hacia arriba
                if (fila > 0)
                {
                    ficha = tablero.posiciones[fila - 1, col + 2].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila - 1, col + 2);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila - 1, col + 2);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila - 1, col + 2);
                            }
                        }
                    }
                        
                   
                }
            }
        }
        /*
         * Método que evalua si el caballo puede moverse dos casillas hacia la izquierda y luego
         * una hacia arriba o hacia abajo. Los parametros correspondientes son la fila y columna
         * de la posicion actual del caballo, y el color de la ficha contraria. Cada movimiento
         * posible encontrado es agregado a la lista de movimientos
         */
        public void buscarMovIzquierda(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            if (col > 1)
            {
                //mover luego hacia abajo
                if (fila < 7)
                {
                    ficha = tablero.posiciones[fila + 1, col - 2].ficha;
                    
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila + 1, col - 2);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila + 1, col - 2);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila + 1, col - 2);
                            }
                        }
                    }
                        
                }
                // mover luego hacia arriba
                if (fila > 0)
                {
                    ficha = tablero.posiciones[fila - 1, col - 2].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila - 1, col - 2);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila - 1, col - 2);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila - 1, col - 2);
                            }
                        }
                    }
                        
                }
            }
        }
        /*
         * Método que evalua si el caballo puede moverse dos casillas hacia abajo y luego una
         * hacia la derecha o izquierda. Los parametros correspondientes son la fila y columna
         * de la posicion actual del caballo, y el color de la ficha contraria. Cada movimiento
         * posible encontrado es agregado a la lista de movimientos
         */
        public void buscarMovAbajo(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            if (fila < 6)
            {
                //mover luego hacia la izquierda
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila + 2 , col -1].ficha;
                    if(ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila + 2, col - 1);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila + 2, col - 1);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila + 2, col - 1);
                            }
                        }
                    }
                    
                }
                // mover luego hacia la derecha
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila + 2, col +1].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila + 2, col + 1);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila + 2, col + 1);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila + 2, col + 1);
                            }
                        }
                       
                    }
                }
            }
        }

        /*
        * Método que evalua si el caballo puede moverse dos casillas hacia arriba y luego una
        * hacia la derecha o izquierda. Los parametros correspondientes son la fila y columna
        * de la posicion actual del caballo, y el color de la ficha contraria. Cada movimiento
        * posible encontrado es agregado a la lista de movimientos
        */
        public void buscarMovArriba(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            if (fila > 1)
            {
                //mover luego hacia la izquierda
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila - 2, col - 1].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila - 2, col - 1);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila - 2, col - 1);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila - 2, col - 1);
                            }
                        }
                    }
                        
                }
                // mover luego hacia la derecha
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila - 2, col + 1].ficha;
                    if (ficha == null || ficha.color == colFichaCont)
                    {
                        y = new PosicionMatriz(fila - 2, col + 1);
                        mov = new ParOrdenado(x, y);
                        bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                        if (!jaqueAlMover)
                        {
                            if (ficha != null && ficha.color == colFichaCont)
                            {
                                // Si la casilla esta llena, puede mover y capturar en caso que
                                // la ficha sea del color contrario
                                base.agregarMov(fila - 2, col + 1);
                            }
                            else if (ficha == null)
                            {
                                // la casilla esta vacia se puede mover
                                base.agregarMov(fila - 2, col + 1);
                            }
                        }
                    }
                        
                }
            }
        }

        /*
         * Método que encuentra las diferentes posiciones dentro del tablero de juego suministrado
         * como paramétro, hacia las cuales se puede mover a partir de su posición actual
         */
        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            // Variables auxiliares
            int fila = posicion.fila;
            int col = posicion.columna;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            // Se comprueba la posibilidad de mover, dos casillas a la derecha y una arriba o
            // abajo
            buscarMovDerecha(tablero, fila, col, colFichaCont);
            // Se comprueba la posibilicad de mover, dos casillas hacia la izquierda y luego
            // una hacia arriba o hacia abajo
            buscarMovIzquierda(tablero, fila, col, colFichaCont);
            // Se comprueba la posibilidad de mover, dos casillas arriba y luego una hacia la
            // izquierda o derecha
            buscarMovArriba(tablero, fila, col, colFichaCont);
            // Se comprueba la posibilidad de mover, dos casillas abajo y luego una hacia la
            // izquierda o derecha
            buscarMovAbajo(tablero, fila, col, colFichaCont);
        }
    }
    /*
     * SubClase que define las funciones principales para un alfil
     */
    internal class Alfil : Ficha
    {
        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Alfil(Color color, PosicionMatriz posicion) : base(color, posicion)
        {
            this.tipo = TipoFicha.Alfil;
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            fichaCopia = new Alfil(this.color, this.posicion);
        }
        /*
         * Método que busca las posiciones posibles en que se puede mover un alfil sobre la
         * correspondiente diagonal ascendente formada a partir de su posición, Se evalua hacia
         * derecha e izquierda. Los parametros son el tablero de juego, la posicion establecida
         * por la fila y columna y el color de la ficha contraria
         */
        public void buscarMovDiagAsc(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            // Se evalua movimientos hacia la derecha y arriba
            int cont = 1;
            while(fila -cont >= 0 && col +cont <8 && ficha == null)
            {

                ficha = tablero.posiciones[fila - cont, col + cont].ficha;
                y = new PosicionMatriz(fila - cont, col + cont);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    if (ficha == null)
                    {
                        // Se agrega la nueva posicion cuando la celda esta vacia
                        base.agregarMov(fila - cont, col + cont);
                    }
                    else if (ficha.color == colFichaCont)
                    {
                        // si la ficha que ocupa la celda es contraria se agrega la posicion
                        base.agregarMov(fila - cont, col + cont);
                    }
                }
                else
                {
                    cont = fila + 1;
                }   
                cont++;
            }
            // Se evalua movimientos hacia la izquierda y abajo
            ficha = null;
            cont = 1;
            while (fila + cont < 8 && col - cont >= 0 && ficha == null)
            {

                ficha = tablero.posiciones[fila + cont, col - cont].ficha;
                y = new PosicionMatriz(fila + cont, col - cont);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    if (ficha == null)
                    {
                        // Se agrega la nueva posicion cuando la celda esta vacia
                        base.agregarMov(fila + cont, col - cont);
                    }
                    else if (ficha.color == colFichaCont)
                    {
                        // si la ficha que ocupa la celda es contraria se agrega la posicion
                        base.agregarMov(fila + cont, col - cont);
                    }
                }
                else
                {
                    cont = col + 1;
                } 
                cont++;
            }
        }

        /*
      * Método que busca las posiciones posibles en que se puede mover un alfil sobre la
      * correspondiente diagonal descendente formada a partir de su posición, Se evalua hacia
      * derecha e izquierda. Los parametros son el tablero de juego, la posicion establecida
      * por la fila y columna y el color de la ficha contraria
      */
        public void buscarMovDiagDesc(Tablero tablero, int fila, int col, Color colFichaCont)
        {
            Ficha ficha = null;
            // Tablero de referencia (copia del tablero actual)
            Tablero tabRef = new Tablero();
            tabRef.copiarTablero(tablero);
            PosicionMatriz x = new PosicionMatriz(fila, col);
            PosicionMatriz y;
            ParOrdenado mov;
            // Se evalua movimientos hacia la derecha y abajo
            int cont = 1;
            while (fila + cont < 8 && col + cont < 8 && ficha == null)
            {

                ficha = tablero.posiciones[fila + cont, col + cont].ficha;
                y = new PosicionMatriz(fila + cont, col + cont);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    if (ficha == null)
                    {
                        // Se agrega la nueva posicion cuando la celda esta vacia
                        base.agregarMov(fila + cont, col + cont);
                    }
                    else if (ficha.color == colFichaCont)
                    {
                        // si la ficha que ocupa la celda es contraria se agrega la posicion
                        base.agregarMov(fila + cont, col + cont);
                    }
                }
                else
                {
                    cont = 8 - fila;
                }   
                cont++;
            }
            // Se evalua movimientos hacia la izquierda y arriba
            ficha = null;
            cont = 1;
            while (fila - cont >= 0 && col - cont >= 0 && ficha == null)
            {

                ficha = tablero.posiciones[fila - cont, col - cont].ficha;
                y = new PosicionMatriz(fila - cont, col - cont);
                mov = new ParOrdenado(x, y);
                bool jaqueAlMover = enJaqueTrasMover(tabRef, mov);
                if (!jaqueAlMover)
                {
                    if (ficha == null)
                    {
                        // Se agrega la nueva posicion cuando la celda esta vacia
                        base.agregarMov(fila - cont, col - cont);
                    }
                    else if (ficha.color == colFichaCont)
                    {
                        // si la ficha que ocupa la celda es contraria se agrega la posicion
                        base.agregarMov(fila - cont, col - cont);
                    }
                }
                else
                {
                    cont = fila + 1;
                } 
                cont++;
            }
        }

        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            // Variables auxiliares
            int fila = posicion.fila;
            int col = posicion.columna;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            // Se evalua la diagonal ascendente correspondiente
            buscarMovDiagAsc(tablero, fila, col, colFichaCont);
            // Se evalua la diagonal descendente correspondiente
            buscarMovDiagDesc(tablero, fila, col, colFichaCont);

        }
    }
    /*
     * SubClase que define las funciones principales para la reina
     */
    internal class Reina : Ficha
    {
        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Reina(Color color, PosicionMatriz posicion) : base(color, posicion) 
        {
            this.tipo = TipoFicha.Reina;
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            fichaCopia = new Reina(this.color, this.posicion);
        }

        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            Ficha alfilRef = new Alfil(this.color, this.posicion);
            Ficha torreRef = new Torre(this.color, this.posicion);
            alfilRef.encontrarMovimientos(tablero);
            torreRef.encontrarMovimientos(tablero);
            if(alfilRef.movimientos != null)
            {
                int n = alfilRef.movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        this.movimientos.Add(alfilRef.movimientos[i]);
                    }
                }
            }
            if(torreRef != null)
            {
                int n = torreRef.movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        this.movimientos.Add(torreRef.movimientos[i]);
                    }
                }
            }
            
        }
    }
    /*
     * SubClase que define las funciones principales para el rey
     */
    internal class Rey : Ficha
    {
        // atributos adicionales para controlar movimientos y estados especiales del rey
        // tales como la posibilidad de enrocar o su estado de jaque
        public bool estadoEnJaque { get; set; }
        public bool enJaque { get; set; }
        public bool haMovido { get; set; }

        // Se separa en un vector nuevo los posibles enroques. son solo dos
        public List<ParOrdenado> movEnroque { get; set; }
        /*
         * El constructor es el mismo de la clase base Ficha
         */
        public Rey(Color color, PosicionMatriz posicion) : base(color, posicion) 
        {
            this.tipo = TipoFicha.Rey;
            this.estadoEnJaque = false;
            this.enJaque = false;
            this.haMovido = false;
            // Se inicializa la lista de enroques posibles
            movEnroque = new List<ParOrdenado>();
        }

        public override void copiarFicha(ref Ficha fichaCopia)
        {
            Rey reyCopia = new Rey(this.color, this.posicion);
            reyCopia.estadoEnJaque = this.estadoEnJaque;
            reyCopia.enJaque = this.enJaque;
            fichaCopia = (Rey)reyCopia;
        }
        /*
         * Metodo que limpia la lista de movimientos posibles. Sobrecarga el metodo base
         * para limpiar las posibilidades de enroque de la lista
         */
        public override void limpiarMovimientos()
        {
            if(this.movimientos != null)
            {
                this.movimientos.Clear();
            }
            if(this.movEnroque != null)
            {
                this.movEnroque.Clear();
            }
        }

        /*
         * Métod de evaluacion de movimientos validos del rey, se sobrecarga pues se debe
         * tener en cuenta la lista de movimientos del tipo enroque que se han separado
         * en una lista aparte
         */
        public override bool esMovimientoVal(ParOrdenado movimiento)
        {
            // Se compara el movimiento con cada uno de los movimientos en la lista
            if(this.movimientos != null)
            {
                int n = movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        ParOrdenado parValido = movimientos[i];
                        if (parValido.esIgual(movimiento))
                        {
                            return true;
                        }
                    }
                }
            }
            // Si no encontro en la lista de movimientos se examinan los movimientos de enroque
            if(this.movEnroque != null)
            {
                int n = movEnroque.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        ParOrdenado parValido = movEnroque[i];
                        if (parValido.esIgual(movimiento))
                        {
                            return true;
                        }
                    }
                }
            }
            // Si ha llegado hasta aqui es porque el movimiento no se encuentra en ninguna lista
            return false;
        }
        /*
         * Método que evalua si un movimiento dado esta en la lista de enroques
         */
        public bool enListaEnroques(ParOrdenado movimiento)
        {
            int n = this.movEnroque.Count;
            if(n > 0)
            {
                for(int i=0; i<n; i++)
                {
                    ParOrdenado enroque = this.movEnroque[i];
                    if (movimiento.esIgual(enroque))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*
         * Sobrecarga del metodo de mover ficha, que evalua si el movimiento es de enroque
         * o no
         */
        public override void mover(ParOrdenado movimiento, Tablero tablero)
        {
            base.mover(movimiento, tablero);
            // Actualiza informacion del rey para el juego
            if (!this.haMovido)
            {
                this.haMovido = true;
            }
            // Actualiza posicion del Rey en el objeto tablero
            if(this.color == Color.Blanco)
            {
                tablero.posReyBlanco = this.posicion;
            }
            else
            {
                tablero.posReyNegro = this.posicion;
            }
            // Si la jugada es de enroque, se debe mover la torre apropiada
            if (enListaEnroques(movimiento))
            {
                int fila = 7;
                if(this.color == Color.Negro)
                {
                    fila = 0;
                }
                // variables auxiliares para manejar la torre
                int col = this.posicion.columna;
                Torre torreEnr;
                ParOrdenado mov;
                PosicionMatriz posTorreEnrocada;
                // Se identifica que torre fue la participante del enroque
                if (col < 4)
                {
                    torreEnr = (Torre)tablero.posiciones[fila, 0].ficha;
                    posTorreEnrocada = new PosicionMatriz(fila, col + 1);
                }
                else
                {
                    posTorreEnrocada = new PosicionMatriz(fila, col - 1);
                    torreEnr = (Torre)tablero.posiciones[fila, 7].ficha;
                }
                // Se hace el movimiento de la torre respectiva
                PosicionMatriz posTorre = torreEnr.posicion;
                // se actualizan los datos en el tablero de Juego para hacer 
                // seguimiento
                tablero.torreEnrocada = posTorre;
                tablero.enroque = true;
                mov = new ParOrdenado(posTorre, posTorreEnrocada);
                torreEnr.movimientos.Add(mov);
                torreEnr.mover(mov, tablero);
                // Se limpia la lista de movimientos de la torre
                torreEnr.limpiarMovimientos();
            }
        }

        /*
         * Método que agrega un movimiento de enroque identificado mediante los parametros
         * de fila y columna de la nueva posicion del rey. El movimiento es agregado a la
         * lista de enroques
         */
        public void agregarEnroque(int filaLleg, int colLleg)
        {
            PosicionMatriz lleg = new PosicionMatriz(filaLleg, colLleg);
            ParOrdenado mov = new ParOrdenado(posicion, lleg);
            // Se agrega a la lista
            movEnroque.Add(mov);
        }
        
        /*
         * Método que comprueba si existen fichas contrarias controlando la posicion a la 
         * que se pretende mover el Rey, buscando por la diagonal ascendente. Los parametros
         * son la fila y columna de la posicion deseada y el tablero de juego actual
         */
        public bool jaqueDiagAsc(int fila, int col, Tablero tablero)
        {
            int filaPeon = fila - 1;
            int colPeon = col + 1;
            Color colFichaCont = Color.Negro;
            if(this.color == Color.Negro)
            {
                filaPeon = fila + 1;
                colPeon = col - 1;
                colFichaCont = Color.Blanco;
            }
            Ficha ficha = null;
            // Comprueba si existe un peon que ataca la posicion dada
            if(filaPeon>=0 && filaPeon < 8 && colPeon >=0 && colPeon < 8)
            {
                ficha = tablero.posiciones[filaPeon, colPeon].ficha;
                if(ficha != null && ficha.tipo == TipoFicha.Peon && ficha.color == colFichaCont)
                {
                    return true;
                }
            }
            // Si no hay peones, se busca un alfil o reina contrarios en dicha diagonal
            // hacia arriba - derecha
            ficha = null;
            int cont = 1;
            while(fila - cont >= 0 && col + cont < 8 && ficha == null)
            {
                ficha = tablero.posiciones[fila - cont, col + cont].ficha;
                cont++;
            }
            // Si al terminar el ciclo encontro alfil o reina contraria queda en jaque
            if(ficha != null)
            {
                if(ficha.tipo == TipoFicha.Alfil || ficha.tipo == TipoFicha.Reina)
                {
                    if(ficha.color == colFichaCont)
                    {
                        return true;
                    }
                    
                }
            }
            // hacia abajo - izquieda
            ficha = null;
            cont = 1;
            while (fila + cont < 8 && col - cont >= 0 && ficha == null)
            {
                ficha = tablero.posiciones[fila + cont, col - cont].ficha;
                cont++;
            }
            // Si al terminar el ciclo encontro alfil o reina contraria queda en jaque
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Alfil || ficha.tipo == TipoFicha.Reina)
                {
                    if(ficha.color == colFichaCont)
                    {
                        return true;
                    }
                    
                }
            }
            // Si ha llegado hasta aqui es que no encontro amenaza de jaque
            return false;
        }

        /*
         * Método que comprueba si existen fichas contrarias controlando la posicion a la 
         * que se pretende mover el Rey, buscando por la diagonal ascendente. Los parametros
         * son la fila y columna de la posicion deseada y el tablero de juego actual
         */
        public bool jaqueDiagDesc(int fila, int col, Tablero tablero)
        {
            int filaPeon = fila - 1;
            int colPeon = col - 1;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                filaPeon = fila + 1;
                colPeon = col + 1;
                colFichaCont = Color.Blanco;
            }
            Ficha ficha = null;
            // Comprueba si existe un peon que ataca la posicion dada
            if (filaPeon >= 0 && filaPeon < 8 && colPeon >=0 && colPeon < 8)
            {
                ficha = tablero.posiciones[filaPeon, colPeon].ficha;
                if (ficha != null && ficha.tipo == TipoFicha.Peon && ficha.color == colFichaCont)
                {
                    return true;
                }
            }
            // Si no hay peones, se busca un alfil o reina contrarios en dicha diagonal
            // hacia abajo - derecha
            ficha = null;
            int cont = 1;
            while (fila + cont < 8 && col + cont < 8 && ficha == null)
            {
                ficha = tablero.posiciones[fila + cont, col + cont].ficha;
                cont++;
            }
            // Si al terminar el ciclo encontro alfil o reina contraria queda en jaque
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Alfil || ficha.tipo == TipoFicha.Reina)
                {
                    if(ficha.color == colFichaCont)
                    {
                        return true;
                    }
                }
            }
            // hacia arriba - izquieda
            ficha = null;
            cont = 1;
            while (fila - cont >= 0 && col - cont >= 0 && ficha == null)
            {
                ficha = tablero.posiciones[fila - cont, col - cont].ficha;
                cont++;
            }
            // Si al terminar el ciclo encontro alfil o reina contraria queda en jaque
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Alfil || ficha.tipo == TipoFicha.Reina)
                {
                    if(ficha.color == colFichaCont)
                    {
                        return true;
                    } 
                }
            }
            // Si ha llegado hasta aqui es que no encontro amenaza de jaque
            return false;
        }
        /*
        * Método que comprueba si existen fichas contrarias controlando la posicion a la 
        * que se pretende mover el Rey, buscando horizontalmente. Los parametros
        * son la fila y columna de la posicion deseada y el tablero de juego actual
        */
        public bool jaqueHorizontal(int fila, int col, Tablero tablero)
        {
            int auxCol = col;
            Color colFichaCont = Color.Negro;
            if(this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            Ficha ficha = null;
            // Se comprueba en direccion derecha
            while(auxCol < 7 && ficha == null)
            {
                ficha = tablero.posiciones[fila, auxCol + 1].ficha;
                auxCol++;
            }
            // Se comprueba si la ficha encontrada es un rey o reina de color contrario
            if(ficha != null)
            {
                if(ficha.tipo == TipoFicha.Torre || ficha.tipo== TipoFicha.Reina)
                {
                    if(ficha.color == colFichaCont)
                    {
                        return true;
                    }
                }
            }
            // Se comprueba en direccion izquieda
            auxCol = col;
            while (auxCol > 0 && ficha == null)
            {
                ficha = tablero.posiciones[fila, auxCol - 1].ficha;
                auxCol--;
            }
            // Se comprueba si la ficha encontrada es un rey o reina de color contrario
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Torre || ficha.tipo == TipoFicha.Reina)
                {
                    if (ficha.color == colFichaCont)
                    {
                        return true;
                    }
                }
            }
            // Si llega hasta aqui no se ha topado con fichas contrarias amenazantes
            return false;
        }

        /*
        * Método que comprueba si existen fichas contrarias controlando la posicion a la 
        * que se pretende mover el Rey, buscando horizontalmente. Los parametros
        * son la fila y columna de la posicion deseada y el tablero de juego actual
        */
        public bool jaqueVertical(int fila, int col, Tablero tablero)
        {
            int auxFila = fila;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            Ficha ficha = null;
            // Se comprueba hacia arriba
            while (auxFila > 0 && ficha == null)
            {
                ficha = tablero.posiciones[auxFila -1, col].ficha;
                auxFila--;
            }
            // Se comprueba si la ficha encontrada es un rey o reina de color contrario
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Torre || ficha.tipo == TipoFicha.Reina)
                {
                    if (ficha.color == colFichaCont)
                    {
                        return true;
                    }
                }
            }
            // Se comprueba hacia abajo
            auxFila = fila;
            while (auxFila < 7 && ficha == null)
            {
                ficha = tablero.posiciones[auxFila + 1, col].ficha;
                auxFila++;
            }
            // Se comprueba si la ficha encontrada es un rey o reina de color contrario
            if (ficha != null)
            {
                if (ficha.tipo == TipoFicha.Torre || ficha.tipo == TipoFicha.Reina)
                {
                    if (ficha.color == colFichaCont)
                    {
                        return true;
                    }
                }
            }
            // Si llega hasta aqui no se ha topado con fichas contrarias amenazantes
            return false;
        }
        /*
         * Método que comprueba si existen caballos controlando la posicion a la 
        * que se pretende mover el Rey, buscando horizontalmente. Los parametros
        * son la fila y columna de la posicion deseada y el tablero de juego actual
        */
        public bool jaqueCaballo(int fila, int col, Tablero tablero)
        {
            Color colFichaCont = Color.Negro;
            Ficha ficha;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            // Se comprueba hacia arriba
            if(fila > 1)
            {
                // Se comprueba si existe un caballo dos posiciones arriba y una a la derecha
                if(col < 7)
                {
                    ficha = tablero.posiciones[fila - 2, col + 1].ficha;
                    if(ficha != null)
                    {
                        if(ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
                // Se comprueba si existe un caballo dos posiciones arriba y una a la izquierda
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila - 2, col - 1].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
            }

            // Se comprueba hacia abajo
            if (fila < 6)
            {
                // Se comprueba si existe un caballo dos posiciones abajo y una a la derecha
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila + 2, col + 1].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
                // Se comprueba si existe un caballo dos posiciones abajo y una a la izquierda
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila + 2, col - 1].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
            }
            // Se comprueba hacia la derecha
            if (col < 6)
            {
                // Se comprueba si existe un caballo dos posiciones a la derecha y una arriba
                if (fila > 0)
                {
                    ficha = tablero.posiciones[fila - 1, col + 2].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
                // Se comprueba si existe un caballo dos posiciones a la derecha y una abajo
                if (fila < 7)
                {
                    ficha = tablero.posiciones[fila + 1, col + 2].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
            }
            // Se comprueba hacia la izquierda
            if (col > 1)
            {
                // Se comprueba si existe un caballo dos posiciones a la izuierda y una arriba
                if (fila > 0)
                {
                    ficha = tablero.posiciones[fila - 1, col - 2].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
                // Se comprueba si existe un caballo dos posiciones a la derecha y una abajo
                if (fila < 7)
                {
                    ficha = tablero.posiciones[fila + 1, col - 2].ficha;
                    if (ficha != null)
                    {
                        if (ficha.tipo == TipoFicha.Caballo && ficha.color == colFichaCont)
                        {
                            return true;
                        }
                    }
                }
            }
            // Si llega hasta aqui no se ha topado con fichas contrarias amenazantes
            return false;
        }

        /*
         * Método que comprueba si al mover el rey en la posicion marcada por los parametros
         * enteros de fila y columna entregados este queda en jaque o atacado por una ficha 
         * contraria segun la posicion actual del tablero de juego entregada por el parametro
         * tablero
         */
        public bool quedaEnJaque(int fila, int col, Tablero tablero)
        {
            // Se evalua jaque por la direccion de la diagonal ascendente
            if(jaqueDiagAsc(fila, col, tablero))
            {
                return true;
            }
            // Se evalua jaque por la direccion de la diagonal descendente
            if (jaqueDiagDesc(fila, col, tablero))
            {
                return true;
            }
            // Se evalua jaque de manera horizontal
            if(jaqueHorizontal(fila, col, tablero))
            {
                return true;
            }
            // Se evalua jaque de manera vertical
            if(jaqueVertical(fila, col, tablero))
            {
                return true;
            }
            // Se evalua jaque por un caballo
            if (jaqueCaballo(fila, col, tablero))
            {
                return true;
            }
            // Si llega aqui es que no ha encontrado amenaza en ninguna direccion
            return false;
        }

        /*
         * Método que evalua si existe la posibilidad de enrocar con alguna de las 
         * torres
         */
        public void posibilidadEnroque(int fila, int col, Tablero tablero)
        {
            if (!haMovido && !estadoEnJaque)
            {
                int colTorreDer = 7;
                int colTorreIzq = 0;
                int incr = 1;
                if(this.color == Color.Negro)
                {
                    colTorreDer = colTorreIzq;
                    colTorreIzq = 7;
                    incr = -1;
                }
                // examina si puede enrocar con torre a la "derecha"
                int relPos = Math.Abs(col - colTorreDer);
                int cont = incr;
                Ficha ficha = null;
                while(relPos > 0 && ficha == null)
                {
                    relPos--;
                    ficha = tablero.posiciones[fila, col + cont].ficha;
                    // Evalua si el rey es atacado mientras enroca, en tal caso no puede
                    // enrocar
                    if(relPos > 0 && quedaEnJaque(fila, col + cont, tablero))
                    {
                        relPos = -1;
                    }
                    cont += incr;
                }
                // Si logra encontrar todas las casillas vacias hasta la torre se puede 
                // enrocar
                if(relPos == 0 && ficha.tipo == TipoFicha.Torre)
                {
                    // El enroque es posible solo si la torre no ha movido
                    Torre torreEnr = (Torre)ficha;
                    if (!torreEnr.haMovido)
                    {
                        int nuevaColRey = col + 2;
                        if (this.color == Color.Negro)
                        {
                            nuevaColRey = col - 2;
                        }
                        this.agregarEnroque(fila, nuevaColRey);
                    }   
                }
                // examina si puede enrocar con torre a la "izquierda"
                incr = (-1) * incr;
                cont = incr;
                relPos = Math.Abs(col - colTorreIzq);
                ficha = null;
                while (relPos > 0 && ficha == null)
                {
                    relPos--;
                    ficha = tablero.posiciones[fila, col + cont].ficha;
                    // Evalua si el rey es atacado mientras enroca, en tal caso no puede
                    // enrocar
                    if (relPos > 0 && quedaEnJaque(fila, col + cont, tablero))
                    {
                        relPos = -1;
                    }
                    cont += incr;
                }
                // Si logra encontrar todas las casillas vacias hasta la torre se puede 
                // enrocar
                if (relPos == 0 && ficha.tipo == TipoFicha.Torre)
                {
                    // El enroque es posible solo si la torre no ha movido
                    Torre torreEnr = (Torre)ficha;
                    if (!torreEnr.haMovido)
                    {
                        int nuevaColRey = col - 2;
                        if (this.color == Color.Negro)
                        {
                            nuevaColRey = col + 2;
                        }
                        this.agregarEnroque(fila, nuevaColRey);
                    }
                        
                }
            }
        }


        public override void encontrarMovimientos(Tablero tablero)
        {
            // Se limpia la lista si se encuentra llena para luego actualizar
            limpiarMovimientos();
            // variables auxiliares
            int fila = posicion.fila;
            int col = posicion.columna;
            Color colFichaCont = Color.Negro;
            if (this.color == Color.Negro)
            {
                colFichaCont = Color.Blanco;
            }
            Ficha ficha;
            // Se comprueba si se puede enrocar
            posibilidadEnroque(fila, col, tablero);
            // Se comprueba si puede mover lateralmente a la derecha o iquierda
            if(col >0 && col < 7)
            {
                // derecha
                ficha = tablero.posiciones[fila, col + 1].ficha;
                // se chequea que al mover no quede en jaque
                if(!quedaEnJaque(fila, col + 1, tablero))
                {
                    // Casilla esta vacia
                    if (ficha == null)
                    {
                        base.agregarMov(fila, col +1);
                    }
                    // Captura ficha contraria
                    else if (ficha.color == colFichaCont)
                    {
                        base.agregarMov(fila, col +1);
                    }
                }
                // izquierda
                ficha = tablero.posiciones[fila, col - 1].ficha;
                // se chequea que al mover no quede en jaque
                if (!quedaEnJaque(fila, col - 1, tablero))
                {
                    // Casilla esta vacia
                    if (ficha == null)
                    {
                        base.agregarMov(fila, col - 1);
                    }
                    // Captura ficha contraria
                    else if (ficha.color == colFichaCont)
                    {
                        base.agregarMov(fila, col - 1);
                    }
                }
            }
            // Se comprueba si existe posibilidad de mover hacia arriba o en diagonal arriba
            if(fila > 0)
            {
                // Movimiento hacia arriba
                ficha = tablero.posiciones[fila - 1, col].ficha;
                // Se chequea que al mover no quede en jaque
                if(!quedaEnJaque(fila -1, col, tablero))
                {
                    // Casilla esta vacia
                    if(ficha == null)
                    {
                        base.agregarMov(fila - 1, col);
                    }
                    // Captura ficha contraria
                    else if(ficha.color == colFichaCont)
                    {
                        base.agregarMov(fila - 1, col);
                    }
                }
                // movimiento en diagonal - izquierda
                if(col > 0)
                {
                    ficha = tablero.posiciones[fila - 1, col -1].ficha;
                    // Se chequea que al mover no quede en jaque
                    if(!quedaEnJaque(fila -1, col -1, tablero))
                    {
                        // Casilla esta vacia
                        if (ficha == null)
                        {
                            base.agregarMov(fila - 1, col -1);
                        }
                        // Captura ficha contraria
                        else if (ficha.color == colFichaCont)
                        {
                            base.agregarMov(fila - 1, col - 1);
                        }
                    }
                }
                // movimiento en diagonal - derecha
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila - 1, col + 1].ficha;
                    // Se chequea que al mover no quede en jaque
                    if(!quedaEnJaque(fila -1, col +1, tablero)){
                    // Casilla esta vacia
                        if (ficha == null)
                        {
                            base.agregarMov(fila - 1, col +1);
                        }
                        // captura ficha contraria
                        else if (ficha.color == colFichaCont)
                        {
                            base.agregarMov(fila - 1, col +1);
                        }
                    }
                }
            }
            // Se comprueba si existe posibilidad de mover hacia abajo o en diagonal abajo
            if (fila < 7)
            {
                // Movimiento hacia abajo
                ficha = tablero.posiciones[fila + 1, col].ficha;
                // Se chequea que al mover no quede en jaque
                if(!quedaEnJaque(fila +1, col, tablero))
                {
                    // Casilla esta vacia
                    if (ficha == null)
                    {
                        base.agregarMov(fila + 1, col);
                    }
                    else if (ficha.color == colFichaCont)
                    {
                        base.agregarMov(fila + 1, col);
                    }
                }
                // movimiento en diagonal (abajo) - izquierda
                if (col > 0)
                {
                    ficha = tablero.posiciones[fila + 1, col - 1].ficha;
                    // Se chequea que al mover no quede en jaque
                    if(!quedaEnJaque(fila + 1, col - 1, tablero))
                    {
                        // Casilla esta vacia
                        if (ficha == null)
                        {
                            base.agregarMov(fila + 1, col - 1);
                        }
                        else if (ficha.color == colFichaCont)
                        {
                            base.agregarMov(fila + 1, col - 1);
                        }
                    }
                }
                // movimiento en diagonal (abajo) - derecha
                if (col < 7)
                {
                    ficha = tablero.posiciones[fila + 1, col + 1].ficha;
                    // Se chequea que al mover no quede en jaque
                    if(!quedaEnJaque(fila + 1, col +1, tablero))
                    {
                        // Casilla esta vacia
                        if (ficha == null)
                        {
                            base.agregarMov(fila + 1, col + 1);
                        }
                        else if (ficha.color == colFichaCont)
                        {
                            base.agregarMov(fila + 1, col + 1);
                        }
                    }
                }
            }

        }
    }
}
