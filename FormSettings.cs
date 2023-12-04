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

namespace Lab6_Ajedrez
{
    public partial class FormSettings : Form
    {
        MediaPlayer reproductor = new MediaPlayer();
        String path = Directory.GetCurrentDirectory() + "\\" + "music" + "\\";
        String cancionReproduciendo;
        double volumenAux = 0.5;
        double volumen = 0.5;


        public FormSettings()
        {
            InitializeComponent();
            reproductor.MediaEnded += new EventHandler(reproductor_MediaEnded);
        }


        /*
         * metodo establecerVolumenMusica
         * cambia el volumen de la cancion que se esta reproduciendo, usando la variable global de volumen
         */
        void establecerVolumenMusica() {
            reproductor.Volume = this.volumen;
        }


        /*
        void ponerMusica(String cancion) {
         * metodo 
         * recibe como parametro un String cancion que indica el nombre de la cancion a reproducir
         * asigna el nombre de la cancion a la variable global cancionReproduciendo para saber cual se esta
         * reproduciendo, reemplaza los espacios del nombre con guiones y lo concatena con la ruta en que esta
         * si alguna cancion se esta reproduciendo la quita, le pasa la ruta de la cancion al reproductor, la reproduce
         * y asigna el volumen
         */
        void ponerMusica(String cancion) {

            this.cancionReproduciendo = cancion;

            cancion = cancion.Replace(" ", "-");
            cancion = path + cancion + ".mp3";

            reproductor.Close();

            Uri URL = new Uri(cancion); 
            reproductor.Open(URL);
            reproductor.Play();

            establecerVolumenMusica();
        }


        /*
         * evento click de buttonComfirmar
         * obtiene el valor actual del comboBoxMusic que contiene las cnaciones
         * valida si se selecciono alguna cancion y si esta no esta sonando ya, si
         * no esta sonando la reproduce.
         * sino se selecciono nunguna cancion quita la que se este reproduciendo y luego
         * cierra el form
         */
        private void buttonComfirmar_Click(object sender, EventArgs e)
        {
            this.volumenAux = this.volumen;

            String cancion = comboBoxMusic.Text;
            if (cancion == "waltz of the flowers" || cancion == "nocturne op 9 no 2" || cancion == "dance of the sugar plum fairy" || cancion == "liebestraume no 3 in a flat major")
            {
                if (cancion != cancionReproduciendo)
                {

                    ponerMusica(cancion);
                }
            }
            else{ 
                reproductor.Close();
            }
            this.Close();

        }


        /*
         * evento ValueChanged del trackBarVolume
         * obtiene el volumen del trackvarVolume y lo establece en el reproductor
         */
        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            double volumen = double.Parse(""+trackBarVolume.Value);
            // se divide entre 100 ya que el volumen va de 0 a 1 mientras que el
            // trackBar va de 1 a 100
            this.volumen = volumen / 100; 
            establecerVolumenMusica();
        }


        /*
         * evento click del buttonCancelar
         * pone como texto del comboBoxMusic la cancion que se esta reproduciendo, reestablece el volumen a como
         * estaba antes de abrir el form y cierra el formulario
         */
        private void buttonCancelar_Click_1(object sender, EventArgs e)
        {
            this.Close();
            comboBoxMusic.Text = cancionReproduciendo;
            this.volumen = this.volumenAux;
            // como el volumen del trackBar va de 1 a 100 y el de volumen aux va de 0 a 1
            // lo multiplica por 100
            trackBarVolume.Value = int.Parse(""+(volumenAux * 100)); 
            establecerVolumenMusica();
        }


        /*
         * evento MediaEnded del reproductor
         * si se acaba la cancion que esta sonando la vulve a poner
         */
        void reproductor_MediaEnded(object sender, EventArgs e) {
            ponerMusica(this.cancionReproduciendo);
        }

    }
}
