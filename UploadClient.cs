using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class DirClient
{
    private IPEndPoint ipe;

    public DirClient(string ipaddr, int port)
    {
        this.ipe = new IPEndPoint(IPAddress.Parse(ipaddr), port);
    }
    
//    public static void writeInFile(string describe){
//        try {
//            using StreamWriter file = new("responses.txt", append: true);
//            file.WriteLine(describe);
//            System.out.println(describe);
//
//        } catch (Exception e) {
//            Console.WriteLine(e);
//        }
//    }

    public bool checkPath(string path)
    {
        FileInfo file = new FileInfo(path);
        if (file.Exists.Equals(true))
        {
            return true;
        }
        return false;
    }

    public string getListing(string path)
    {
        string a = "";
        string myPath = "";
        string myCaption = "";
        string myDate = "";
        try
        {
            Socket mySocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mySocket.Connect(ipe);
            if (mySocket.Connected)
            {
                myPath = path;
                while (!checkPath(myPath))
                {
                    Console.WriteLine("Path invalid!");
                    Console.WriteLine("Enter correct path:");
                    string modifyPath = Console.ReadLine();
                    myPath = modifyPath;
                }
                Console.WriteLine("Enter caption:");
                myCaption = Console.ReadLine();
                Console.WriteLine("Enter date:");
                myDate = Console.ReadLine();

                Console.WriteLine("final #path: " + myPath + " #caption: " + myCaption + " #date: " + myDate);

                //Byte[] bytesSent = Encoding.ASCII.GetBytes(myPath + '\0');
                //Console.WriteLine("bytesSent: " + bytesSent);
                //mySocket.Send(bytesSent, bytesSent.Length, 0);
                //Byte[] bytesReceived = new Byte[1];
                //while (true)
                //{
                //    if ((mySocket.Receive(bytesReceived, bytesReceived.Length, 0) == 0) ||
                //              (Encoding.ASCII.GetString(bytesReceived, 0, 1)[0] == '\0'))
                //    {
                //        break;
                //    }
                //    a += Encoding.ASCII.GetString(bytesReceived, 0, 1);
                //}
            }
            
            mySocket.Close();
            return "Successfully updated image" + myCaption;
        }
        catch (ArgumentNullException e)
        {
            throw new Exception(@"[ArguementNullException From Sync.DirClient]", e);
        }
        catch (SocketException e)
        {
            throw new Exception(@"[SocketException From Sync.DirClient]", e);
        }
    }
    static void Main(string[] args)
    {
        DirClient myDirClient = new DirClient("127.0.0.1", 8888);

        Console.WriteLine("Enter path:");
        string pathNew = Console.ReadLine();

        Console.WriteLine(myDirClient.getListing(pathNew));
        //the following call is just to block the main thread so that the results are listed to the screen
        //Console.Read();
        Console.WriteLine("Press any key to close the console...");
        Console.ReadKey();
    }
}
