using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

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

    public string getListing(string path)
    {
        string a = "";
        try
        {
            Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine(path);
            s.Connect(ipe);
            if (s.Connected)
            {
                Byte[] bytesSent = Encoding.ASCII.GetBytes(path + '\0');
                s.Send(bytesSent, bytesSent.Length, 0);
                Byte[] bytesReceived = new Byte[1];
                while (true)
                {
                    if ((s.Receive(bytesReceived, bytesReceived.Length, 0) == 0) ||
                              (Encoding.ASCII.GetString(bytesReceived, 0, 1)[0] == '\0'))
                    {
                        break;
                    }
                    a += Encoding.ASCII.GetString(bytesReceived, 0, 1);
                }
            }
            s.Close();
            return a;
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
        Console.WriteLine(myDirClient.getListing("/"));
        //the following call is just to block the main thread so that the results are listed to the screen
        Console.Read();
    }
}
