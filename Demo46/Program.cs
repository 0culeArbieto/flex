using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using General.Librerias.AccesoDatos;

namespace Demo46
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando el Servidor Socket");
            string ipServidor = "ws://192.168.1.33:9002";
            List<IWebSocketConnection> clientes = new List<IWebSocketConnection>();
            WebSocketServer servidor = new WebSocketServer(ipServidor);
            Console.WriteLine("Servidor Socket iniciado en: {0}", ipServidor);
            Console.WriteLine();
            servidor.Start(cliente =>
            {
                cliente.OnOpen = () =>
                {
                    clientes.Add(cliente);
                    Console.WriteLine("Se abrió la conexión del IP: {0}", cliente.ConnectionInfo.ClientIpAddress);
                };
                cliente.OnClose = () =>
                {
                    clientes.Remove(cliente);
                    Console.WriteLine("Se cerró la conexión del IP: {0}", cliente.ConnectionInfo.ClientIpAddress);
                };
                cliente.OnMessage = (string texto) =>
                {
                    string[] campos = texto.Split('~');
                    daSQL odaSQL = new daSQL("conNW");
                    string rpta = "";
                    if (campos[0] == "Consultar")
                    {
                        string nroPag = campos[1];
                        string nroReg = campos[2];
                        string data = nroPag + "|" + nroReg;
                        rpta = odaSQL.ejecutarComando("uspCubso2PaginarCsv", "@data", data);
                        cliente.Send(rpta);
                        Console.WriteLine("Recibido: " + texto + " Enviado: " + rpta.Length.ToString());
                    }
                    else
                    {
                        rpta = odaSQL.ejecutarCopiaMasiva("Articulo", texto, '¬', '|');
                        cliente.Send("OK");
                        Console.WriteLine("Recibido: " + texto.Length + " bytes - Enviado: OK");
                    }                    
                    Console.WriteLine("Se envió datos: ", texto);
                };
            });
            Console.WriteLine();
            Console.WriteLine("Pulsa Enter para finalizar el Servidor");
            Console.ReadLine();
        }
    }
}
