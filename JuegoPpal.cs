using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab6_Ajedrez
{
    public partial class JuegoPpal : Form
    {
        OperBasicas operBas;
        const String nombreArchivo = "Puntajes.txt";
        Tablero tableroJuego;
        bool juegoActivo = false;
        Jugador jugadorUno;
        Jugador jugadorDos;
        FormNombres formNombres;
        FormSettings formSettings;
        Color turnoActual = Color.Blanco;
        PosicionMatriz fichaSeleccionada;
        PictureBox[,] matPictureBox;
        public String nombreJugUno { get; set; }
        public String nombreJugDos { get; set; }

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
            formSettings = new FormSettings();
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
                jugadorUno= new Jugador(nombreJugUno);
                jugadorDos = new Jugador(nombreJugDos);
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
        * que dibujaran cada caramelo
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
                    //Image recurso = seleccionarRecursoCaramelo(tab.valores[i, j]);
                    //asignarPicture(i, j, recurso);
                }
            }

        }

        private void confBtn_Click(object sender, EventArgs e)
        {
            formSettings.ShowDialog();
        }
    }

 
}
