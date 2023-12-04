using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_Ajedrez
{
    /*
     * Clase para representar un jugador de la partida de ajedrez en curso.
     * Esta clase permitira almacenar los datos respecto al nombre del jugador, el puntaje
     * máximo alcanzado si se trata de un jugador anterior, el puntaje actual y los datos
     * de su desempeño: partidas ganadas, partidas perdidas, partidas en tablas
     */
    public class Jugador
    {
        public String nombre { get; set; }
        public int maxPuntaje { get; set; }
        public int puntajePartida { get; set; }
        public int partidasGanadas { get; set; }
        public int partidasPerdidas { get; set; }
        public int partidasTablas { get; set; }

        List<Ficha> fichasEnJuego;
        List<Ficha> fichasPerdidas;

        Color fichaAsignada { get; set; }

        public Jugador(String nombre)
        {
            this.nombre = nombre;
            this.puntajePartida = 0;
        }
    }
}
