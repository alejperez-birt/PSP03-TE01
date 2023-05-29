using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiedraPapelTijera
{
    internal class Jugador
    {
        string nombre = string.Empty;
        int puntuacion = 0;

        public Jugador(string nombre, int puntuacion)
        {
            this.Nombre = nombre;
            this.Puntuacion = 0;
        }

        public string Nombre { get => nombre; set => nombre = value; }
        public int Puntuacion { get => puntuacion; set => puntuacion = value; }
    }
}
