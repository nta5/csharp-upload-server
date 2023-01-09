using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Web;

public class UploadClient
{
    private IPEndPoint ipe;

    public UploadClient(string ipaddr, int port)
    {
        this.ipe = new IPEndPoint(IPAddress.Parse(ipaddr), port);
    }

    public bool checkPath(string path)
    {
        FileInfo file = new FileInfo(path);
        if (file.Exists.Equals(true))
        {
            return true;
        }
        return false;
    }

    public void uploadFile()
    {
        try
        {
            Socket mySocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mySocket.Connect(ipe);
            if (mySocket.Connected)
            {
                string myCaption = "";
                string myDate = "";

                Console.WriteLine("Enter path:");
                string myPath = Console.ReadLine();
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

                string currentPath = Directory.GetCurrentDirectory();
                // Mac user
                string folderPath = currentPath + @"/" + "images";
                // Windows user
                // string folderPath = currentPath + @"\" + "images";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                byte[] img = File.ReadAllBytes(myPath);
                // string imgName = myPath.Split('/')[myPath.Split('/').Length - 1];
                // string saveName = "/" + imgName.Split('.')[0] + "_" + myDate.Replace(' ', '-') + "_" + myCaption.Replace(' ', '-') + ".png";
                // using (var fs = new FileStream(folderPath + saveName, FileMode.Create, FileAccess.Write))
                // {
                //     fs.Write(img, 0, img.Length);
                // }
                Console.WriteLine("Successfully updated image " + myCaption);
                mySocket.Send(img);
                mySocket.Shutdown(SocketShutdown.Send);

                Byte[] bytesReceived = new Byte[1];
                string res = "";
                while (true)
                {
                   if ((mySocket.Receive(bytesReceived, bytesReceived.Length, 0) == 0) || (Encoding.ASCII.GetString(bytesReceived, 0, 1)[0] == '\0'))
                   {
                       break;
                   }
                   res += Encoding.ASCII.GetString(bytesReceived, 0, 1);
                }
                Console.WriteLine(res);
            }
            mySocket.Close();
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
        UploadClient myClient = new UploadClient("127.0.0.1", 8888);
        myClient.uploadFile();
        Console.WriteLine("Press any key to close the console...");
        Console.ReadKey();
    }
}
