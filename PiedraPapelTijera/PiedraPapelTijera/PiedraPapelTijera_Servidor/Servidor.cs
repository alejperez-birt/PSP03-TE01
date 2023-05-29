using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PiedraPapelTijera
{

    public class TCPServidor
    {
        private Object o = new object();

        public static int Main(String[] args)
        {
            TCPServidor pptServidor = new TCPServidor();
            Partida partida = new Partida();

            TcpClient socketCliente = null;
            NetworkStream nwStream = null;
            StreamWriter stWriter = null;
            StreamReader stReader = null;

            string servidor = "127.0.0.1";
            IPAddress ipServidor = IPAddress.Parse(servidor);
            Int32 puerto = 13000;

            Console.WriteLine("");
            Console.WriteLine("SERVIDOR.- Piedra, Papel, Tijera");
            Console.WriteLine("================================");
            Console.WriteLine("");

            TcpListener listener = new TcpListener(ipServidor, puerto);
            Console.WriteLine("Se ha creado el Socket Listener");
            listener.Start(2);

            while (true)
            {
                socketCliente = listener.AcceptTcpClient();
                Console.WriteLine("");
                Console.WriteLine("Se ha establecido la conexión con el cliente.");

                Thread hilo = new Thread(() => pptServidor.ControlDatos(socketCliente, partida));
                hilo.Start();
            }

            socketCliente.Close();

            Console.WriteLine("Fin del juego");
            Console.Read();
            return 0;
        }
        public TCPServidor()
        {

        }

        private void ControlDatos(TcpClient socketCliente, Partida partida)
        {
            string eleccionJugador = string.Empty;
            bool finJuego = false;

            NetworkStream nwStream = socketCliente.GetStream();
            StreamWriter stWriter = new StreamWriter(nwStream);
            StreamReader stReader = new StreamReader(nwStream);
            Console.WriteLine("Se han creado los Buffers de entrada y salida.");
            stWriter.WriteLine("Indica el nombre con el que quieres inscribirte al juego: ");
            stWriter.Flush();

            // Se reciben los nombres de los jugadores y se asignan a la partida
            string nombreJugador = stReader.ReadLine();
            string respuesta = string.Empty;

            if (partida.NombreJugador01.Equals(""))
            {
                partida.NombreJugador01 = nombreJugador;
                Console.WriteLine("JUGADOR 01: {0}", partida.NombreJugador01);
                respuesta = partida.NombreJugador01;
            }
            else if (partida.NombreJugador02.Equals(""))
            {
                partida.NombreJugador02 = nombreJugador;
                Console.WriteLine("JUGADOR 02: {0}", partida.NombreJugador02);
                respuesta = partida.NombreJugador02;
            }
            else // Si ya hay dos jugadores en la partida se rechaza al siguiente
            {
                respuesta = "RECHAZADO";
                finJuego = true;
            }

            stWriter.WriteLine(respuesta);
            stWriter.Flush();

            try
            {
                while(!finJuego)
                {
                    try
                    {
                        eleccionJugador = stReader.ReadLine(); // Se recibe la información introducida por el cliente
                        
                        lock (o)
                        {
                            if (partida.FinJuego) // Si un jugador ha terminado ya la partida, se marca el fin de juego para el resto
                            {
                                eleccionJugador = "FINJUEGO";
                            }

                            // Se analiza la eleccion del cliente
                            switch (eleccionJugador)
                            {
                                case "PUNTUACION": // Se guarda en respuesta el estado actual de la partida
                                    respuesta = "<J1>" + partida.NombreJugador01 + "<P1>" + partida.PuntuacionJugador01 + "<J2>" + partida.NombreJugador02 + "<P2>" + partida.PuntuacionJugador02;
                                    break;

                                case "RESULTADO": // Se guarda en respuesta el ganador de la ultima jugada
                                    respuesta = partida.UltimoGanador;
                                    break;

                                case "FINJUEGO": // Se guarda en respuesta un resumen de la partida antes de terminar

                                    respuesta = "<F>";
                                    if (partida.PuntuacionJugador01 > partida.PuntuacionJugador02)
                                    {
                                        respuesta += partida.NombreJugador01 + " es el ganador de la partida!!!";
                                    }
                                    else if(partida.PuntuacionJugador02 > partida.PuntuacionJugador01)
                                    {
                                        respuesta += partida.NombreJugador02 + " es el ganador de la partida!!!";
                                    }
                                    else
                                    {
                                        respuesta += "La partida ha terminado en empate!!!";
                                    }

                                    partida.FinJuego = true;
                                    finJuego = true;
                                    break;

                                default: // Si no es una de las anteriores es porque el cliente está escogiendo una opcion de Jugar
                                    respuesta = "* Jugada número: " + partida.Jugadas + ". "; // Se añade a respuesta el número de jugada

                                    // Se asigna la eleccion del jugador controlando si ya ha introducido una opción en esta jugada
                                    // Si ya es así se añade esta informacion a respuesta
                                    if (nombreJugador.Equals(partida.NombreJugador01))
                                    {
                                        if (partida.EleccionJugador01.Equals(""))
                                        {
                                            partida.EleccionJugador01 = eleccionJugador;
                                            Console.WriteLine(partida.NombreJugador01 + ": " + partida.EleccionJugador01);
                                        }
                                        else
                                        {
                                            respuesta = "Pero no puedes volver a jugar hasta que tu oponente elija en la partida anterior. ";
                                        }
                                    }
                                    else
                                    {
                                        if (partida.EleccionJugador02.Equals(""))
                                        {
                                            partida.EleccionJugador02 = eleccionJugador;
                                            Console.WriteLine(partida.NombreJugador02 + ": " + partida.EleccionJugador02);
                                        }
                                        else
                                        {
                                            respuesta = "Pero no puedes volver a jugar hasta que tu oponente elija en la partida anterior. ";
                                        }
                                    }

                                    // Si los dos jugadores han hecho su eleccion en la jugada se procesa la informacion y se determina el ganador de esa jugada
                                    if(!partida.EleccionJugador01.Equals("") && !partida.EleccionJugador02.Equals(""))
                                    {
                                        switch (partida.EleccionJugador01)
                                        {
                                            case "Piedra":
                                                switch (partida.EleccionJugador02)
                                                {
                                                    case "Piedra":
                                                        partida.UltimoGanador = "Empate";
                                                        break;

                                                    case "Papel":
                                                        partida.UltimoGanador = partida.NombreJugador02;
                                                        partida.PuntuacionJugador02++;
                                                        break;

                                                    case "Tijera":
                                                        partida.UltimoGanador = partida.NombreJugador01;
                                                        partida.PuntuacionJugador01++;
                                                        break;
                                                }
                                                break;

                                            case "Papel":
                                                switch (partida.EleccionJugador02)
                                                {
                                                    case "Piedra":
                                                        partida.UltimoGanador = partida.NombreJugador01;
                                                        partida.PuntuacionJugador01++;
                                                        break;

                                                    case "Papel":
                                                        partida.UltimoGanador = "Empate";
                                                        break;

                                                    case "Tijera":
                                                        partida.UltimoGanador = partida.NombreJugador02;
                                                        partida.PuntuacionJugador02++;
                                                        break;
                                                }
                                                break;

                                            case "Tijera":
                                                switch (partida.EleccionJugador02)
                                                {
                                                    case "Piedra":
                                                        partida.UltimoGanador = partida.NombreJugador02;
                                                        partida.PuntuacionJugador02++;
                                                        break;

                                                    case "Papel":
                                                        partida.UltimoGanador = partida.NombreJugador01;
                                                        partida.PuntuacionJugador01++;
                                                        break;

                                                    case "Tijera":
                                                        partida.UltimoGanador = "Empate";
                                                        break;
                                                }
                                                break;
                                        }

                                        // Se muestra por consola la informacion de esa partida y la puntuacion de los jugadores
                                        partida.Jugadas++;
                                        partida.EleccionJugador01 = "";
                                        partida.EleccionJugador02 = "";
                                        Console.WriteLine("");
                                        Console.WriteLine("Ultimo ganador de partida: " + partida.UltimoGanador);
                                        Console.WriteLine("Puntuación de " + partida.NombreJugador01 + ": " + partida.PuntuacionJugador01);
                                        Console.WriteLine("Puntuación de " + partida.NombreJugador02 + ": " + partida.PuntuacionJugador02);
                                        Console.WriteLine("");
                                    }
                                    else // Si solo un jugador ha hecho su eleccion....
                                    {
                                        respuesta += "Esperamos la jugada de tu oponente.";
                                    }

                                    break;

                            }

                            // Enviamos la informacion al cliente (respuesta)
                            stWriter.WriteLine(respuesta);
                            stWriter.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Out.Flush();
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                stReader.Close();
                stWriter.Close();
                nwStream.Close();
            }
        }
    }
}