using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PiedraPapelTijera
{

    public class TCPCliente
    {

        TcpClient socket = null;
        NetworkStream nwStream = null;
        StreamWriter stWriter = null;
        StreamReader stReader = null;
        public static int Main(String[] args)
        {
            PartidaCliente partidaCliente = new PartidaCliente();
            TCPCliente pptCliente = new TCPCliente();
            string servidor = "127.0.0.1";
            Int32 puerto = 13000;

            pptCliente.ConectarServidor(servidor, puerto, partidaCliente);
            pptCliente.ControlDatos(partidaCliente);
            pptCliente.CerrarConexion();

            Console.WriteLine("\n Fin del juego");
            Console.Read();
            return 0;
        }
        public TCPCliente()
        {

        }

        private void ConectarServidor(String servidor, Int32 puerto, PartidaCliente partidaCliente)
        {
            try
            {
                Console.WriteLine("");
                Console.WriteLine("CLIENTE.- Piedra, Papel, Tijera");
                Console.WriteLine("===============================");
                Console.WriteLine("");

                this.socket = new TcpClient(servidor, puerto);
                Console.WriteLine("Se ha creado el Socket Cliente");
                nwStream = socket.GetStream();
                this.stWriter = new StreamWriter(nwStream);
                this.stReader = new StreamReader(nwStream);
                Console.WriteLine("Se han creados los Buffers de escritura y lectura.");

                Console.WriteLine("");
                Console.WriteLine("****************************");
                Console.WriteLine("** PIEDRA, PAPEL ó TIJERA **");
                Console.WriteLine("****************************");
                Console.WriteLine("");
                
                // Se solicita el nombre del jugador y se envia al servidor
                Console.WriteLine(stReader.ReadLine());
                string nombreJugador = Console.ReadLine();
                stWriter.WriteLine(nombreJugador);
                stWriter.Flush();

                // Recibimos la confirmacion de servidor
                nombreJugador = stReader.ReadLine();
                if (nombreJugador.Equals("RECHAZADO")) // En el caso de que se haya rechazado, se muestra por pantalla
                {
                    Console.WriteLine("Lo sentimos, se ha alcanzado el número máximo de jugadores");
                }
                else
                {
                    Console.WriteLine("Bienvenid@ {0}, tu inscripción fue correcta!!!", nombreJugador);
                    partidaCliente.Nombre = nombreJugador;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void ControlDatos(PartidaCliente partidaCliente)
        {
            if (!partidaCliente.Nombre.Equals("")) // Si el jugador es aceptado continua el juego
            {
                bool finJuego = false;
                while (!finJuego)
                {
                    bool opcionCorrectaPrincipal = false;
                    string opcionesPrincipal = "-123";
                    string opcionPrincipal = string.Empty;

                    // Se muestra el menu principal hasta que la opcion sea una correcta
                    while (!opcionCorrectaPrincipal)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Elige la opción que desees: ");
                        Console.WriteLine("----------------------------");
                        Console.WriteLine(" 1: Jugar");
                        Console.WriteLine(" 2: Puntuación");
                        Console.WriteLine(" 3: Mostrar resultado");
                        Console.WriteLine("-1: Salir");
                        Console.WriteLine("----------------------------");
                        Console.Write("Tu opción: ");
                        opcionPrincipal = Console.ReadLine();

                        if (opcionesPrincipal.Contains(opcionPrincipal))
                        {
                            opcionCorrectaPrincipal = true;
                        }
                    }

                    string respuestaServidor = string.Empty;
                    
                    // Se analiza la opcion elegida
                    switch (opcionPrincipal)
                    {
                        case "1": // Si ha elegido "Jugar" se muestra el submenu
                            bool opcionCorrecta = false;
                            string opciones = "123";
                            string opcion = string.Empty;

                            // Se seguira mostrando hasta que la opcion elegida sea válida
                            while (!opcionCorrecta)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Escoge piedra, papel o tijera: ");
                                Console.WriteLine("-------------------------------");
                                Console.WriteLine(" 1: Piedra");
                                Console.WriteLine(" 2: Papel");
                                Console.WriteLine(" 3: Tijera");
                                Console.WriteLine("-------------------------------");
                                Console.Write("Tu elección: ");
                                opcion = Console.ReadLine();

                                if (opciones.Contains(opcion))
                                {
                                    opcionCorrecta = true;
                                }
                            }

                            // Se analiza ahora la opcion elegida en el submenu asignando su valor
                            string eleccion = string.Empty;
                            switch (opcion)
                            {
                                case "1":
                                    eleccion = "Piedra";
                                    break;

                                case "2":
                                    eleccion = "Papel";
                                    break;

                                case "3":
                                    eleccion = "Tijera";
                                    break;
                            }

                            Console.WriteLine("Has elegido {0}", eleccion); // Se muestra por pantalla la opcion elegida y se envia al servidor
                            stWriter.WriteLine(eleccion);
                            stWriter.Flush();

                            // Se espera la respuesta del servidor
                            respuestaServidor = stReader.ReadLine();
                            if (respuestaServidor.Contains("<F>")) // Si corresponde con el final de la partida se muestra por pantalla y se marca "finJuego"
                            {
                                Console.WriteLine("");
                                Console.WriteLine("No se puede seguir jugando.");
                                Console.WriteLine("Su oponente ha abandonado la partida.");
                                Console.WriteLine(respuestaServidor.Substring(3));
                                finJuego = true;
                            }
                            else // Si no es el final se muestra la informacion recbida del servidor
                            {
                                Console.WriteLine(respuestaServidor);
                            }
                            
                            break;

                        case "2": // Si se ha elegido mostrar puntuacion:
                                  // Se envia la opcion al servidor y se espera la respuesta
                                  // Se analiza la informacion de la puntuacion recibida del servidor y se muestra en detalle por pantalla
                            Console.WriteLine("Has elegido puntuación.");
                            stWriter.WriteLine("PUNTUACION");
                            stWriter.Flush();

                            respuestaServidor = stReader.ReadLine();
                            int indexJ1 = respuestaServidor.IndexOf("<J1>");
                            int indexP1 = respuestaServidor.IndexOf("<P1>");
                            int indexJ2 = respuestaServidor.IndexOf("<J2>");
                            int indexP2 = respuestaServidor.IndexOf("<P2>");

                            partidaCliente.NombreJugador01 = respuestaServidor.Substring(indexJ1 + 4, indexP1 - indexJ1 - 4);
                            partidaCliente.PuntuacionJugador01 = respuestaServidor.Substring(indexP1 + 4, indexJ2 - indexP1 - 4);
                            partidaCliente.NombreJugador02 = respuestaServidor.Substring(indexJ2 + 4, indexP2 - indexJ2 - 4);
                            partidaCliente.PuntuacionJugador02 = respuestaServidor.Substring(indexP2 + 4);

                            Console.WriteLine("Puntuación jugador: " + partidaCliente.NombreJugador01 + "; número de puntos: " + partidaCliente.PuntuacionJugador01);
                            Console.WriteLine("Puntuación jugador: " + partidaCliente.NombreJugador02 + "; número de puntos: " + partidaCliente.PuntuacionJugador02);

                            break;

                        case "3": // Si se ha elegido mostrar puntuacion:
                                  // Se envia la opcion al servidor y se espera la respuesta
                                  // Se analiza la informacion del ganador recibida del servidor y se muestra por pantalla
                            Console.WriteLine("Has elegido mostrar resultado.");
                            stWriter.WriteLine("RESULTADO");
                            stWriter.Flush();

                            respuestaServidor = stReader.ReadLine();

                            partidaCliente.UltimoGanador = respuestaServidor;
                            Console.WriteLine("El ganador de la última jugada has sido: " + partidaCliente.UltimoGanador);

                            break;

                        case "-1": // Se envia el fin de juego al servidor y se muestra la respuesta recibida
                            stWriter.WriteLine("FINJUEGO");
                            stWriter.Flush();

                            respuestaServidor = stReader.ReadLine();

                            Console.WriteLine(respuestaServidor.Substring(3));

                            finJuego = true;
                            break;
                    }
                }
            }
        }

        private void CerrarConexion()
        {
            this.stReader.Close();
            this.stWriter.Close();
            this.nwStream.Close();
            this.socket.Close();
        }
    }
}