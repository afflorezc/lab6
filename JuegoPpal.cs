using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab6_Ajedrez
{
    public partial class JuegoPpal : Form
    {
        public OperBasicas operBas;
        const String nombreArchivo = "Puntajes.txt";
        Tablero tableroJuego;
        bool juegoActivo = false;
        public Jugador jugadorUno;
        public Jugador jugadorDos;
        FormNombres formNombres;
        FormSettings formSettings;
        Color turnoActual = Color.Blanco;
        PosicionMatriz fichaSeleccionada;
        PictureBox[,] matPictureBox;

        /*
         * Método de inicialización del objeto de tipo formulario principal.
         * Se identifica archivo existente de puntajes para ser mostrados en la pantalla
         */
        public JuegoPpal()
        {
            //Método generado automaticamente
            InitializeComponent();
            // se identifica si existen puntajes registrados y se cargan
            operBas = new OperBasicas();
            if (operBas.existenArchivos())
            {
                cargarPuntajes();
            }
            formNombres = new FormNombres(this);
            formSettings = new FormSettings(this);
        }

        /*
         * Método que abre y lee el archivo de texto que almacena la información de los mejores
         * puntajes registrados
         */
        private void cargarPuntajes()
        {

            String[] lineasArchivo = operBas.leerArchivo(nombreArchivo);
            if (lineasArchivo != null)
            {
                int n = lineasArchivo.Length;
                for (int i = 0; i < n; i++)
                {
                    String linea = lineasArchivo[i];
                    int inicio = linea.IndexOf('|');
                    int longitud = inicio - 1;
                    String nombre = linea.Substring(0, longitud);
                    String subCadena = linea.Substring(inicio + 1);
                    inicio = subCadena.IndexOf('|');
                    longitud = inicio - 1;
                    String puntajeJug = subCadena.Substring(0, longitud);
                    String fecha = subCadena.Substring(inicio + 1);
                    puntajesGridView.Rows.Add(i + 1, nombre, puntajeJug, fecha);
                }
            }

        }
        /*
         * Método que guarda o crea el archivo de puntajes de acuerdo a la información registrada
         * y actualizada de puntajes del dataGridView
         */
        private void guardarArchivo()
        {
            if (!operBas.existenArchivos())
            {
                operBas.crearArchivo(nombreArchivo);
            }
            int n = puntajesGridView.RowCount;
            int m = puntajesGridView.ColumnCount;
            // se recorren cada una de las celdas y se adicionan los datos 
            for (int i = 0; i < n; i++)
            {
                String linea = "";
                for (int j = 0; j < m; j++)
                {
                    linea = linea + puntajesGridView.Rows[i].Cells[j].Value.ToString();
                    if (j < m - 1)
                    {
                        linea = linea + "|";
                    }
                }
                operBas.escribirLinea(nombreArchivo, linea);
            }

        }

        /*
         * Método que actualiza la nueva información de un jugador cuando ha terminado un juego
         * y que logra posicionarse entre los 5 mejores puntajes actuales
         * 
         */
        private void registrarPuntaje()
        {
            int n = puntajesGridView.RowCount;
            // identificar primero que jugador gana la partida
            String nombreJug = jug1Label.Text;
            String textoPuntaje = Jug1Puntaje.Text;
            int puntaje = int.Parse(textoPuntaje);
            int pos = nombreJug.IndexOf(":");
            nombreJug = nombreJug.Substring(pos + 2);
            // Se registra la fecha del juego
            DateTime fecha = DateTime.Now;
            if (n > 1)
            {
                for (int i = 1; i < n; i++)
                {
                    int regPuntaje
                        = int.Parse(puntajesGridView.Rows[i].Cells[2].Value.ToString());
                    if (puntaje > regPuntaje)
                    {
                        puntajesGridView.Rows.Insert(i, i + 1, nombreJug, puntaje, fecha);
                        guardarArchivo();
                        return;
                    }
                }
            }
            else
            {
                puntajesGridView.Rows.Add("1", nombreJug, puntaje, fecha);
                guardarArchivo();
            }
        }
        /*
         * Método que da inicio a un nuevo juego, se abre un input box que solicita los
         * nombres de los dos jugadores que se enfrentaran en una partida, luego asigna
         * el color de fichas para cada uno de los jugadores e inicializa un objeto de tipo
         * tablero y se muestra en pantalla
         */
        private void nuevoJuegoBtn_Click(object sender, EventArgs e)
        {
            // Si hay un juego en curso se pregunta si desea finalizar, en dicho caso no se
            // registra ningún puntaje pues la partida no ha terminado satisfactoriamente en 
            // un jaque mate por parte de alguno de los oponentes
            if (juegoActivo)
            {
                // Cambiar por mensaje de que hay un juego activo?
                DialogResult decision = new DialogResult();
                decision = operBas.mensajeDesicion("Terminar juego", "En este momento" +
                    " hay un juego en curso. ¿Desea finalizar este juego e iniciar otro?");
                if (decision == DialogResult.No)
                {
                    return;
                }
            }
            // Se abre ventana tipo InputBox para ingresar el nombre del Jugador
            // en el inputForm se crean las variables de tipo jugador y se inicializan 
            // su puntaje al igual que el color asignado para la partida (este se hace de
            // manera aleatoria)
            if (true) //result == DialogResult.OK)
            {
                // pide los nombres de los jugadores
                formNombres.ShowDialog();
                // se activa un juego
                juegoActivo = true;
                jug1Label.Text = $"Jugador 1: {jugadorUno.nombre}";
                jug2Label.Text = $"Jugador 2: {jugadorDos.nombre}";
                Jug1Puntaje.Text = $"Puntaje: {jugadorUno.puntajePartida}";
                jug2Puntaje.Text = $"Puntaje: {jugadorDos.puntajePartida}";
                // Se inicializa la ficha seleccionad en matriz con un valor equivalente al
                // "vacio" (no fichas seleccionadas)
                fichaSeleccionada = new PosicionMatriz(-1, -1);
                tableroJuego = new Tablero();
                pintarTablero();
            }
        }

        /*
        * Método que inicializa el tablero de juego de acuerdo a la matriz del objeto
        * tipo tablero al asignar valores a una matriz de objetos tipo PictureBox
        * que dibujaran cada casilla con su respectiva ficha en caso de poseerla
        */
        public void pintarTablero()
        {

            //se inicializa la matriz de imágenes
            matPictureBox = new PictureBox[8, 8];

            // Creación y ubicación de cada una de las imagenes en cada celda (i, j)
            // Del espacio (TableLayoutPanel) destinado para tal fin
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image recurso = tableroJuego.posiciones[i, j].imagen;
                    asignarPicture(i, j, recurso);
                }
            }

        }

        /*
        * Método que hace una asignación de una PictureBox a la matriz de imagenes en la
        * posición establecida
        */
        private void asignarPicture(int i, int j, Image imagenRecurso)
        {
            // se identifica el tamaño en pixeles de la celda en el tablelayoutPanel
            TableLayoutRowStyleCollection estilosFil = tableroPpal.RowStyles;
            TableLayoutColumnStyleCollection estilosCol = tableroPpal.ColumnStyles;
            int alto = (int) estilosFil[i].Height;
            int ancho = (int) estilosCol[j].Width;
            PictureBox pictureAux = new PictureBox();
            pictureAux.Image = imagenRecurso;
            pictureAux.Location = new System.Drawing.Point(0, 0);
            pictureAux.Name = $"pictureBox{i},{j}";
            pictureAux.Size = new System.Drawing.Size(ancho, alto);
            // cada imagen se ubica extendida a todo el tamaño de cada celda
            // sin margenes
            pictureAux.Margin = new Padding(0);
            pictureAux.Anchor = (AnchorStyles.Left | AnchorStyles.Right |
                                  AnchorStyles.Top | AnchorStyles.Bottom);
            pictureAux.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // Se establece el mismo manejador de eventos de clic para cada PictureBox
            pictureAux.Click += PictureBox_Click;
            // si ya existe una imagen en el TableLayoutPanel se elimina para
            // luego reemplazar
            if (tableroPpal.Controls.Contains(matPictureBox[i, j]))
            {
                tableroPpal.Controls.Remove(matPictureBox[i, j]);
            }
            matPictureBox[i, j] = pictureAux;
            // Se adiciona la nueva imagen
            tableroPpal.Controls.Add(pictureAux, j, i);
        }
        /*
         * Método que redibuja las casillas del juego para representar los posibles
         * movimientos que posee una ficha seleccionada
         */
        public void marcarCasillas(int fila, int col)
        {
            // Se calculan las posiciones de posible movimiento de la ficha
            Ficha ficha = tableroJuego.posiciones[fila, col].ficha;
            ficha.encontrarMovimientos(tableroJuego);
            // Se marcan las posiciones donde puede mover
            if(ficha.movimientos != null)
            {
                int n = ficha.movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        PosicionMatriz mov = ficha.movimientos[i].y;
                        int f = mov.fila;
                        int c = mov.columna;
                        // Marca la casilla
                        tableroJuego.posiciones[f, c].marcada = true;
                        tableroJuego.posiciones[f, c].asignarImagen();
                        asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                    }
                }
            }
            // Si la ficha es rey se comprueban sus jugadas de enroque
            if (ficha.tipo == TipoFicha.Rey)
            {
                Rey rey = (Rey)ficha;
                if(rey.movEnroque != null)
                {
                    int n = rey.movEnroque.Count;
                    if (n > 0)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            PosicionMatriz mov = rey.movEnroque[i].y;
                            int f = mov.fila;
                            int c = mov.columna;
                            // Marca la casilla
                            tableroJuego.posiciones[f, c].marcada = true;
                            tableroJuego.posiciones[f, c].asignarImagen();
                            asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                        }
                    }
                }
            }
            
        }
        /*
         * Método que redibuja las casillas del tablero de juego que fueron marcadas
         * como posibles movimientos de juego en cierto momento
         */
        public void desmarcarCasillas(int fila, int columna)
        {
            Ficha ficha = tableroJuego.posiciones[fila, columna].ficha;
            if(ficha.movimientos != null)
            {
                int n = ficha.movimientos.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        PosicionMatriz casilla = ficha.movimientos[i].y;
                        int f = casilla.fila;
                        int c = casilla.columna;
                        tableroJuego.posiciones[f, c].marcada = false;
                        tableroJuego.posiciones[f, c].asignarImagen();
                        asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                    }
                }
            }
            // si la ficha es un rey se desmarcan los enroques
            if (ficha.tipo == TipoFicha.Rey)
            {
                Rey rey = (Rey)ficha;
                if (rey.movEnroque != null)
                {
                    int n = rey.movEnroque.Count;
                    if (n > 0)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            PosicionMatriz mov = rey.movEnroque[i].y;
                            int f = mov.fila;
                            int c = mov.columna;
                            // Marca la casilla
                            tableroJuego.posiciones[f, c].marcada = false;
                            tableroJuego.posiciones[f, c].asignarImagen();
                            asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                        }
                    }
                }
            }

        }
        /*
         * Método que evalua si el rey contrario queda en jaque luego de una jugada
         */
        public void evaluarJaque()
        {
            PosicionMatriz posRey;
            if(turnoActual == Color.Blanco)
            {
                posRey = tableroJuego.posReyNegro;
            }
            else
            {
                posRey = tableroJuego.posReyBlanco;
            }
            Rey rey = (Rey) tableroJuego.posiciones[posRey.fila, posRey.columna].ficha;
            if(rey.quedaEnJaque(posRey.fila, posRey.columna, tableroJuego))
            {
                this.operBas.jaqueSonido();
                rey.enJaque = true;
                if (!rey.estadoEnJaque)
                {
                    rey.estadoEnJaque = true;
                }
                tableroJuego.posiciones[posRey.fila, posRey.columna].asignarImagen();
                Image imagen = tableroJuego.posiciones[posRey.fila, posRey.columna].imagen;
                asignarPicture(posRey.fila, posRey.columna, imagen);
            }
        }
        /*
       * Método que evalua cuando se hace click sobre un objeto del tipo PictureBox
       * del tablero de juego, es decir, cuando se selecciona una ficha del juego con
       * el objetivo de aplicar una jugada a partir de intercambiar esta celda a sus
       * posiciones vecinas (Manejador generico del evento click)
       */
        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                String nombre = pictureBox.Name;
                // establece la posicion que se ha seleccionado
                String posicion = nombre.Substring(10);
                int posComa = posicion.IndexOf(",");
                int fila = int.Parse(posicion.Substring(0, posComa));
                int col = int.Parse(posicion.Substring(posComa + 1));
                // Si hay una ficha seleccionada se intenta mover hacia alguna
                // posicion valida
                if (fichaSeleccionada.fila > -1 && fichaSeleccionada.columna > -1)
                {
                    // Si la selecion actual es la misma ficha previamente
                    // seleccionada, no se debe hacer nada
                    if(fila == fichaSeleccionada.fila && col == fichaSeleccionada.columna)
                    {
                        return;
                    }
                    // Si ya existe una ficha seleccionada se intenta mover a la posición
                    // señalada, si no se puede mover se deselecciona y limpia su lista
                    // de movimientos posibles
                    PosicionMatriz lleg = new PosicionMatriz(fila, col);
                    Ficha ficha =  tableroJuego.posiciones[fichaSeleccionada.fila,
                                      fichaSeleccionada.columna].ficha;
                    ParOrdenado mov = new ParOrdenado(ficha.posicion, lleg);
                    if (ficha.esMovimientoVal(mov))
                    {
                        // Si se mueve de manera exitosa se dibujan todas las celdas
                        ficha.mover(mov, tableroJuego);
                        // Se reasigna la imagen a la casilla donde estaba la ficha
                        int f = fichaSeleccionada.fila;
                        int c = fichaSeleccionada.columna;
                        asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                        // Si el movimiento fue de enroque se reasigna la imagen de la
                        // posicion inicial de la torre
                        if (tableroJuego.enroque)
                        {
                            f = tableroJuego.torreEnrocada.fila;
                            c = tableroJuego.torreEnrocada.columna;
                            asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                            // Se actualizan las variables
                            tableroJuego.torreEnrocada.fila = -1;
                            tableroJuego.torreEnrocada.columna = -1;
                            tableroJuego.enroque = false;
                        }
                        // Se desmarcan las casillas
                        desmarcarCasillas(fila, col);
                        // Se limpia la lista de movimientos de la ficha movida
                        tableroJuego.posiciones[fila, col].ficha.limpiarMovimientos();
                        // Se evalua si se da un jaque
                        evaluarJaque();
                        // Se cambia el turno
                        if (turnoActual == Color.Blanco)
                        {
                            turnoActual = Color.Negro;
                        }
                        else
                        {
                            turnoActual = Color.Blanco;
                        }
                        // Ya no existen fichas seleccionadas
                        fichaSeleccionada.fila = -1;
                        fichaSeleccionada.columna = -1;
                        // activar sonido mover ficha 
                        this.operBas.moverFichaSonido();

                    }
                    else
                    {
                        // Si no es movimiento valido comprobar si se selecciono
                        // otra ficha y cambiar la seleccion por dicha ficha
                        ficha = tableroJuego.posiciones[fila, col].ficha;
                        if(ficha != null && ficha.color == turnoActual)
                        {
                            int f = fichaSeleccionada.fila;
                            int c = fichaSeleccionada.columna;
                            fichaSeleccionada.fila = fila;
                            fichaSeleccionada.columna = col;
                            tableroJuego.posiciones[f, c].marcada = false;
                            tableroJuego.posiciones[f, c].asignarImagen();
                            asignarPicture(f, c, tableroJuego.posiciones[f, c].imagen);
                            // Se limpian las posiciones posibles jugadas y se limpia
                            // la lista de movimientos
                            desmarcarCasillas(f, c);
                            // Se limpia la lista de movimientos
                            tableroJuego.posiciones[f, c].ficha.limpiarMovimientos();
                            // Se marca la casilla nueva seleccionada al igual que sus
                            // movimientos posibles
                            tableroJuego.posiciones[fila, col].marcada = true;
                            tableroJuego.posiciones[fila, col].asignarImagen();
                            asignarPicture(fila, col, tableroJuego.posiciones[fila, col].imagen);
                            marcarCasillas(fila, col);
                            // activar sonido seleccionar ficha
                        }
                        else
                        {
                            // activar sonido error
                            this.operBas.errorSonido();
                        }
                    }
                    
                }
                else
                {
                    // Se selecciona la celda clickeada (si es ficha valida del turno
                    // en juego
                    Ficha ficha = tableroJuego.posiciones[fila, col].ficha;
                    if(ficha != null && ficha.color == turnoActual)
                    {
                        fichaSeleccionada.fila = fila;
                        fichaSeleccionada.columna = col;
                        // Se marca la ficha
                        tableroJuego.posiciones[fila, col].marcada = true;
                        tableroJuego.posiciones[fila, col].asignarImagen();
                        asignarPicture(fila, col, tableroJuego.posiciones[fila, col].imagen);
                        // Se calculan las posiciones de posible movimiento de la ficha
                        marcarCasillas(fila, col);
                        
                    }

                }
            }

        }

        private void confBtn_Click(object sender, EventArgs e)
        {
            formSettings.ShowDialog();
        }
    }

 
}
