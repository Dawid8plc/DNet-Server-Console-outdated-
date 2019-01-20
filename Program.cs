using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace DNetServer
{
    public class Program
    {
        //Player 1 Specific
        public static TcpClient Player1;
        public static NetworkStream P1Stream;
        public static int P1X;


        //Player 2 Specific
        public static TcpClient Player2;
        public static NetworkStream P2Stream;
        public static int P2X;

        //Config file variables
        public static string IP;
        public static int Port;
        

        public static List<string> connections = new List<string>();
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting up DNet_Server...");


            //load this shit
            string[] TestDialog = new string[] { "Shit=10" };

            var dic = File.ReadAllLines("CONFIG.txt").Select(l => l.Split(new[] { '=' })).ToDictionary(s => s[0].Trim(), s => s[1].Trim());


            var PIECEOFSHIT = TestDialog.Select(l => l.Split(new[] { '=' })).ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            string ANOTHERPIECEOFSHIT = PIECEOFSHIT["Shit"];
            Console.WriteLine(ANOTHERPIECEOFSHIT);

            string serverIP = dic["ip"];
            int serverPort = Int32.Parse(dic["port"]);
            Console.WriteLine(serverIP);
            IP = serverIP;
            Port = serverPort;


            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse(serverIP);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, serverPort);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    System.Console.WriteLine("Listening... ");

                    Thread thread = new Thread(new ThreadStart(Control));

                    thread.Start();
                    Console.WriteLine("Type Help for a list of commands");


                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    Player1 = server.AcceptTcpClient();

                    EndPoint ep = Player1.Client.RemoteEndPoint;
                    string eps = ep.ToString();
                    connections.Add(eps);
                    System.Console.WriteLine(eps + " Player 1 Connected! Waiting for the second player...");
                    P1Stream = Player1.GetStream();
                    Byte[] player1indicator = System.Text.Encoding.ASCII.GetBytes("You are Player 1");
                    P1Stream.Write(player1indicator, 0, player1indicator.Length);



                    Player2 = server.AcceptTcpClient();

                    EndPoint ep2 = Player2.Client.RemoteEndPoint;
                    string eps2 = ep2.ToString();
                    connections.Add(eps2);
                    System.Console.WriteLine(eps + " Player 2 Connected! What now...?");
                    P2Stream = Player2.GetStream();
                    Byte[] player2indicator = System.Text.Encoding.ASCII.GetBytes("You are Player 2");
                    P1Stream.Write(player2indicator, 0, player2indicator.Length);

                    data = null;

                    // Get a stream object for reading and writing
                    
                    P2Stream = Player2.GetStream();

                    Byte[] gamestarted = System.Text.Encoding.ASCII.GetBytes("Game/Server/Started");

                    System.Console.WriteLine("Telling the Clients that the game started...");
                    P1Stream.Write(gamestarted, 0, gamestarted.Length);
                    P2Stream.Write(gamestarted, 0, gamestarted.Length);



                    Thread PThread1 = new Thread(new ThreadStart(PListen1));

                    PThread1.Start();

                    Thread PThread2 = new Thread(new ThreadStart(PListen2));

                    PThread2.Start();

                    break;

                    // Loop to receive all the data sent by the client.
                    //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    //{
                    //    // Translate data bytes to a ASCII string.
                    //    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    //    System.Console.WriteLine("Received: {0}", data);

                    //    // Process the data sent by the client.
                    //    //data = data.ToUpper();




                    //    int e = 0;
                    //    while (e == 0)
                    //    {
                    //        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    //        System.Console.WriteLine("Received: {0}", data);



                    //        data = Console.ReadLine();
                    //        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    //        // Send back a response.
                    //        stream.Write(msg, 0, msg.Length);
                    //        System.Console.WriteLine("Sent: {0}", data);
                    //    }



                    //}

                    // Shutdown and end connection
                    //Player1.Close();
                    //Player2.Close();
                }
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("SocketException: {0}", e);


            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            System.Console.WriteLine("Server ready");
        }



        public string returnPath()
        {
            string folder = Environment.CurrentDirectory;
            return folder;
        }

        public static void Listen()
        {

        }

        public static void Control()
        {
            int i = 1;
            while (i == 1)
            {
                string input = Console.ReadLine();
                if (input == "Connections")
                {
                    Console.WriteLine("Currently connected IPs:");
                    Console.WriteLine(String.Join(", ", connections.ToArray()));
                }
                else if (input == "Exit")
                {
                    Environment.Exit(0);
                }
                else if (input == "Help")
                {
                    Console.WriteLine("Available commands:");
                    Console.WriteLine("Help (Shows this message), ServerConfig (Gives you the information about the server), Connections (Shows currently connected IPs), Exit (Closes the Server)");
                }
                else if (input == "ServerConfig")
                {
                    Console.WriteLine("Information about this server:");
                    Console.WriteLine("IP: " + IP);
                    Console.WriteLine("Port: " + Port);
                }
                //else if (input)
                else
                {
                    Console.WriteLine(input + " is not a valid command");
                }
            }

        }
    



        
        

        public static void PListen1()
        {
            Byte[] bytes = new Byte[256];
            String data = null;

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = P1Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                System.Console.WriteLine(data);

                // Process the data sent by the client.
                //data = data.ToUpper();



                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    // Send back a response.
                    P2Stream.Write(msg, 0, msg.Length); 
                



            }
        }

        public static void PListen2()
        {
            Byte[] bytes = new Byte[256];
            String data = null;

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = P2Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                System.Console.WriteLine(data);

                // Process the data sent by the client.
                //data = data.ToUpper();





                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    // Send back a response.
                    P1Stream.Write(msg, 0, msg.Length);
                



            }
        }
    }
}
