// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Procon.Console
{
    using Procon.Core;
    class Program
    {
        static void Main(string[] args)
        {
            bool startTCP = false;
            bool stopTCP = false;
            int index = 0;

            //help command
            if (args.Contains("-help") == true || args.Contains("--help") == true)
            {
                System.Console.WriteLine("Usage: Procon.Console.exe [[start|stop] [port] [shutdownsignal]]");
                System.Console.WriteLine("Example: Procon.Console.exe start 10000 STOPME");
                System.Console.WriteLine("Example: Procon.Console.exe stop 10000 STOPME");
                System.Console.WriteLine("Example: Procon.Console.exe start STOPME");
                return;
            }

            // search for parameter to classic console or the tcp_Console
            if (args.Contains("start") == true || args.Contains("stop") == true)
            {
                if (args.Contains("start") == true)
                {
                    index = Array.IndexOf(args, "start");
                    startTCP = true;
                }
                else if(args.Contains("stop") == true)
                {
                    index = Array.IndexOf(args, "stop");
                    stopTCP = true;
                }
                int port = 9001;
                index++; //increment index
                if (args.Count() >= index +1)
                {
                    if (Int32.TryParse(args[index], out port) == true)
                    {
                        port = Convert.ToInt32(args[index]);
                        index++; //increment index
                    }
                    else
                    {
                        System.Console.WriteLine("The port parameter is invaild, Now using default port 9001");
                        port = 9001;
                    }
                }
                else
                {
                    System.Console.WriteLine("No port parameter found, Now using default port 9001");
                    port = 9001;
                }
                string strShutdownSignal = "PROCONSTOP"; //Shutdown Signal
                if (args.Count() >= index + 1)
                {
                    strShutdownSignal = args[index];
                    System.Console.WriteLine("The shutdownsignal is: {0}", strShutdownSignal);
                }
                else
                {
                    System.Console.WriteLine("No Shutdown parameter found!, Now using default: {0}", strShutdownSignal);
                }
                if(stopTCP)
                {
                    //Stop the Console server
                    try
                    {
                        TcpClient client = new TcpClient(IPAddress.Loopback.ToString(), port);
                        NetworkStream stream = client.GetStream();
                        // Send the message to the connected TcpServer. 
                        Byte[] data = System.Text.Encoding.ASCII.GetBytes(strShutdownSignal);
                        stream.Write(data, 0, data.Length);
                        System.Console.WriteLine("Sending shutdown signal: {0}", strShutdownSignal);
                        // Buffer to store the response bytes.
                        data = new Byte[256];
                        String responseData = String.Empty;
                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        //System.Console.WriteLine("Received: {0}", responseData);
                        if (String.Equals(responseData, "OK"))
                        {
                            System.Console.WriteLine("The server accepted the shutdownsignal!");
                        }
                        else
                        {
                            System.Console.WriteLine("The server reject the shutdownsignal!");
                        }
                        // Close everything.
                        stream.Close();
                        client.Close();
                        return;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error: {0}", e.Message);
                    }
                }
                // search for parameter to classic console or the tcp_Console
                else if(startTCP)
                {
                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String data = "";
                    bool stop = false;
                    System.Console.WriteLine("Starting Haisan Instance..");
                    if (args.Length > 0)
                    {
                        System.Console.WriteLine("Starting with arguments: {0}", String.Join(" . ", args));
                    }
                    Instance haisan = new Instance() { Arguments = new List<String>(args) }.Execute();
                    try
                    {
                        TcpListener listener = new TcpListener(IPAddress.Loopback, port);   // Bind the listener to the loopback address (eg. 127.0.0.1 + port)
                        listener.Start(); // Start the listener
                        System.Console.WriteLine("Waiting for a shutdownsignal on TCP-Port " + port + "... ");
                        while (stop == false)
                        {
                            // Perform a blocking call to accept requests.
                            TcpClient client = listener.AcceptTcpClient();
                            //System.Console.WriteLine("Connected!");
                            data = "";
                            NetworkStream stream = client.GetStream();
                            int i;
                            // Loop to receive all the data sent by the client.
                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                // Translate data bytes to a ASCII string.
                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                                //System.Console.WriteLine(String.Format("Received: {0}", data));
                                stop = String.Equals(data.Trim(), strShutdownSignal);
                                if (stop == true)
                                {
                                    data = "OK";
                                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                                    // Send back a response.
                                    stream.Write(msg, 0, msg.Length);
                                    System.Console.WriteLine("Received shutdown request, Sending: {0}", data);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            stream.Close();
                            client.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.Write("Error: {0}", e.Message);
                    }
                    System.Console.WriteLine("Shutting down Haisan instance..");
                    haisan.Dispose();
                    System.Console.WriteLine("Closing");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else
            {
                //classic Console
                System.Console.WriteLine("Type 'exit' to close this application");
                System.Console.WriteLine("Starting Haisan Instance..");
                if (args.Length > 0)
                {
                    System.Console.WriteLine("Starting with arguments: {0}", String.Join(" . ", args));
                }
                Instance haisan = new Instance() { Arguments = new List<String>(args) }.Execute();
                while (String.Compare(System.Console.ReadLine(), "exit", true) != 0) ;
                System.Console.WriteLine("Shutting down Haisan instance..");
                haisan.Dispose();
                System.Console.WriteLine("Closing");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
