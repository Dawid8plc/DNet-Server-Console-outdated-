using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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



        public static List<string> connections = new List<string>();
        public static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.0.184");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

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


                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    Player1 = server.AcceptTcpClient();
                    
                    EndPoint ep = Player1.Client.RemoteEndPoint;
                    string eps = ep.ToString();
                    connections.Add(eps);
                    System.Console.WriteLine(eps + "Player 1 Connected! Waiting for the second player...");


                    Player2 = server.AcceptTcpClient();

                    EndPoint ep2 = Player2.Client.RemoteEndPoint;
                    string eps2 = ep2.ToString();
                    connections.Add(eps2);
                    System.Console.WriteLine(eps + "Player 2 Connected! What now...?");

                    data = null;

                    // Get a stream object for reading and writing
                    P1Stream = Player1.GetStream();
                    P2Stream = Player2.GetStream();

                    Thread PThread1 = new Thread(new ThreadStart(PListen1));

                    PThread1.Start();

                    Thread PThread2 = new Thread(new ThreadStart(PListen2));

                    PThread2.Start();

                    int i;

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


            System.Console.WriteLine("\nHit enter to continue...");
            System.Console.Read();
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
            string ye = Console.ReadLine();
            if (ye == "conn")
            {
    
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
                System.Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                //data = data.ToUpper();



                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    // Send back a response.
                    P2Stream.Write(msg, 0, msg.Length); 
                    System.Console.WriteLine("Sent: {0}", data);
                



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
                System.Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                //data = data.ToUpper();





                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    // Send back a response.
                    P1Stream.Write(msg, 0, msg.Length);
                    System.Console.WriteLine("Sent: {0}", data);
                



            }
        }
    }
}
