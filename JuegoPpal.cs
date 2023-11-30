using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6_Ajedrez
{
    public partial class JuegoPpal : Form
    {
        OperBasicas operBas;
        const String nombreArchivo = "Puntajes.txt";

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


    }

 
}
