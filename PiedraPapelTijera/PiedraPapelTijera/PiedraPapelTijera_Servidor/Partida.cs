using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiedraPapelTijera
{
    public class Partida
    {
        string nombreJugador01 = string.Empty;
        string eleccionJugador01 = string.Empty;
        int puntuacionJugador01 = 0;
        string nombreJugador02 = string.Empty;
        string eleccionJugador02 = string.Empty;
        int puntuacionJugador02 = 0;
        int jugadas = 1;
        string ultimoGanador = string.Empty;
        bool finJuego = false;
        public Partida() { }

        public string NombreJugador01 { get => nombreJugador01; set => nombreJugador01 = value; }
        public string EleccionJugador01 { get => eleccionJugador01; set => eleccionJugador01 = value; }
        public int PuntuacionJugador01 { get => puntuacionJugador01; set => puntuacionJugador01 = value; }
        public string NombreJugador02 { get => nombreJugador02; set => nombreJugador02 = value; }
        public string EleccionJugador02 { get => eleccionJugador02; set => eleccionJugador02 = value; }
        public int PuntuacionJugador02 { get => puntuacionJugador02; set => puntuacionJugador02 = value; }
        public int Jugadas { get => jugadas; set => jugadas = value; }
        public string UltimoGanador { get => ultimoGanador; set => ultimoGanador = value; }
        public bool FinJuego { get => finJuego; set => finJuego = value; }
    }
}
