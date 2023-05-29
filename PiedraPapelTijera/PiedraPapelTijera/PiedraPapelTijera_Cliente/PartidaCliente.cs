using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PiedraPapelTijera
{

    public class PartidaCliente
    {

        private string nombre = string.Empty;

        private string ultimoGanador = string.Empty;
        private string nombreJugador01 = string.Empty;
        private string puntuacionJugador01 = string.Empty;
        private string nombreJugador02 = string.Empty;
        private string puntuacionJugador02 = string.Empty;

        public PartidaCliente()
        {

        }

        public string Nombre { get => nombre; set => nombre = value; }
        public string UltimoGanador { get => ultimoGanador; set => ultimoGanador = value; }
        public string NombreJugador01 { get => nombreJugador01; set => nombreJugador01 = value; }
        public string PuntuacionJugador01 { get => puntuacionJugador01; set => puntuacionJugador01 = value; }
        public string NombreJugador02 { get => nombreJugador02; set => nombreJugador02 = value; }
        public string PuntuacionJugador02 { get => puntuacionJugador02; set => puntuacionJugador02 = value; }
    }
}