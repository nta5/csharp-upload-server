using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

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

    private byte[] GetBytesFromImage(String imageFile)
    {
        MemoryStream ms = new MemoryStream();
        Image img = Image.FromFile(imageFile);
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

        return ms.ToArray();
    }

    public string uploadFile()
    {
        try
        {
            Socket mySocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mySocket.Connect(ipe);
            if (mySocket.Connected)
            {
                FileStream fileStream;
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
                string folderPath = currentPath + @"\" + "images";
                string filePath = currentPath + @"\" + "imagesInfor.txt";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                if (!Directory.Exists(filePath))
                {
                    fileStream = new FileStream("imagesInfor.txt", FileMode.Append, FileAccess.Write);
                    fileStream.Close();
                }
                using (StreamWriter outputFile = new StreamWriter("imagesInfor.txt", true))
                {
                    outputFile.WriteLine(myPath + @"*" + myCaption + @"*" + myDate + @"*");
                }

                //byte[] img = GetBytesFromImage(myPath);
                //Bitmap bitmap = new Bitmap(myPath);
                //Console.WriteLine(bitmap);
                //System.IO.File.WriteAllBytes(folderPath, img);


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
            return "Successfully updated image " + myCaption;
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

        

        Console.WriteLine(myClient.uploadFile(pathNew));
        //the following call is just to block the main thread so that the results are listed to the screen
        //Console.Read();
        Console.WriteLine("Press any key to close the console...");
        Console.ReadKey();
    }
}
