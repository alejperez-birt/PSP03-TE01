using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EnviarFoto
{
    internal class Cliente
    {
        public static int Main(String[] args)
        {

            Cliente servidor = new Cliente();
            servidor.PedirFoto();

            Console.WriteLine("Pulsa Intro para continuar...");
            Console.ReadLine();

            return 0;
        }
        private void PedirFoto()
        {
            Socket sender = null;

            try
            {
                bool finEnvio = false;

                // Se declara el puerto a utilizar
                int port = 13000;
                string data = null;

                Console.WriteLine("");
                Console.WriteLine("CLIENTE.- Envio de fotos");
                Console.WriteLine("=========================");
                Console.WriteLine("");

                // Se obtiene la IP del servidor (nuestro equipo) y se crea el socket
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[1];
                sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Iniciando el cliente...");

                // Se conecta el socket al servidor
                IPEndPoint iPEndPoint = new IPEndPoint(ipAddress.Address, port);
                sender.Connect(iPEndPoint);
                Console.WriteLine("Socket conectado a servidor " + sender.RemoteEndPoint.ToString());
                Console.WriteLine("");

                Console.WriteLine("");
                Console.WriteLine("********************");
                Console.WriteLine("** ENVIO DE FOTOS **");
                Console.WriteLine("********************");
                Console.WriteLine("");

                while (!finEnvio)
                {
                    // Se crea e inicializa el buffer para el envío y recepción
                    byte[] bytesImagen = new Byte[150000];
                    bool opcionCorrecta = false;
                    string opciones = "1234";
                    string opcion = string.Empty;
                    string opcionTexto = string.Empty;

                    while (!opcionCorrecta) // Mientras no se introduzca una opción correcta se sigue mostrando el menú
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Elige la opción que desees: ");
                        Console.WriteLine("----------------------------");
                        Console.WriteLine(" 1: Foto Monte");
                        Console.WriteLine(" 2: Foto Playa");
                        Console.WriteLine(" 3: Foto Ciudad");
                        Console.WriteLine(" 4: Salir");
                        Console.WriteLine("----------------------------");
                        Console.Write("Tu opción: ");
                        opcion = Console.ReadLine();

                        if (opciones.Contains(opcion))
                        {
                            opcionCorrecta = true;
                        }
                    }

                    // Se analiza la opcion introducida y se guarda en "opcionTexto" el nombre de la imagen a solicitar
                    switch (opcion)
                    {
                        case "1":
                            opcionTexto = "FotoMonte";
                            Console.WriteLine("Solicitando foto al servidor...");
                            break;

                        case "2":
                            opcionTexto = "FotoPlaya";
                            Console.WriteLine("Solicitando foto al servidor...");
                            break;

                        case "3":
                            opcionTexto = "FotoCiudad";
                            Console.WriteLine("Solicitando foto al servidor...");
                            break;

                        case "4":
                            opcionTexto = "Fin";
                            finEnvio = true;
                            break;
                    }

                    // Se envia al servidor el texto con la solicitud
                    byte[] solicitud = Encoding.ASCII.GetBytes(opcionTexto + "<EOF>");
                    sender.Send(solicitud);

                    // Si no se ha elegido "Salir"... 
                    if (!opcionTexto.Equals("Fin"))
                    {
                        // Se genera el path donde se guardará la imagen (Carpeta del proyecto Cliente)
                        string path = @"../../../" + opcionTexto + ".jpg";

                        int bytesRecibidos = sender.Receive(bytesImagen);
                        Array.Resize(ref bytesImagen, bytesRecibidos); // Se redimensiona el array con el tamaño de los bytes recibidos
                        File.WriteAllBytes(path, bytesImagen); // Se crea el archivo con los bytes recibidos en la carpeta especificada
                        Console.WriteLine("");
                        Console.WriteLine("Foto recibida y almacenada en la siguiente ruta:");
                        Console.WriteLine(" -> " + Path.GetFullPath(path)); // Se muestra la ruta absoluta del archivo creado
                        Console.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                sender.Close();
            }
        }
    }
}