using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EnviarFoto
{
    internal class Servidor
    {
        public static int Main(String[] args)
        {

            Servidor cliente = new Servidor();
            cliente.EnviarFoto();

            Console.WriteLine("Pulsa Intro para continuar...");
            Console.ReadLine();

            return 0;
        }
        private void EnviarFoto()
        {
            Socket listener = null;
            Socket handler = null;
            
            try
            {
                // Se declara el puerto a utilizar
                int port = 13000;
                string data = null;

                // Se crea el buffer para el envío y recepción
                byte[] bytesImagen = new Byte[150000];

                Console.WriteLine("");
                Console.WriteLine("SERVIDOR.- Envio de fotos");
                Console.WriteLine("=========================");
                Console.WriteLine("");

                // Se obtiene la IP del servidor (nuestro equipo) y se crea el socket listener
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[1];

                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Iniciando el servidor...");

                // Se asocia el socket a la IP y al puerto utilizado
                IPEndPoint iPEndPoint = new IPEndPoint(ipAddress.Address, port);
                listener.Bind(iPEndPoint);
                listener.Listen(5);

                // Se establece conexión con el cliente abriendo un segundo socket para comunicarse
                handler = listener.Accept();
                Console.WriteLine("Conexión con el cliente aceptada.");
                Console.WriteLine("");

                while (true)
                {
                    data = null;

                    // Se recibe el texto con la petición de foto
                    while (true)
                    {
                        int bytesRecibidos = handler.Receive(bytesImagen);
                        data += Encoding.ASCII.GetString(bytesImagen, 0, bytesRecibidos);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Se elimina la marca <EOF> y si no es "Fin"...
                    data = data.Substring(0, data.IndexOf("<EOF>"));
                    if (data.Equals("Fin"))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Solicitud del cliente: " + data);

                        // Se genera el path de la foto a enviar (Carpeta del proyecto Servidor)
                        string path = @"../../../" + data + ".jpg";

                        // Se leen los bytes del archivo a enviar y se envian al cliente
                        bytesImagen = File.ReadAllBytes(path);
                        handler.Send(bytesImagen);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                handler.Close();
                listener.Close();
            }
        }
    }
}

