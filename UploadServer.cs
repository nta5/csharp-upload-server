using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static ServletRequest;

public class UploadServlet
{

    Socket cls = null;

    public UploadServlet(Socket socket) { this.cls = socket; }

    public void threadMethod()
    {
        Byte[] bytesReceived = new Byte[1];
        string req = "";
        ArrayList reqByte = new ArrayList();
        IAsyncResult result;

        Action action = () =>
        {
            // Console.WriteLine("Async thread");
            while (true)
            {
                if ((cls.Receive(bytesReceived, bytesReceived.Length, 0) == 0)) {
                    break;
                }
                req += Encoding.ASCII.GetString(bytesReceived, 0, 1);
                reqByte.Add(bytesReceived[0]);

                if(req.StartsWith("G") && req.Contains("Accept-Language"))
                {
                    Console.WriteLine("GET request");
                    break;
                }

                if(req.StartsWith("P") && countBoundary("------WebKitFormBoundary", req) >= 4)
                {
                    Console.WriteLine("POST request");
                    break;
                }
            }
        };


        result = action.BeginInvoke(null, null);
        result.AsyncWaitHandle.WaitOne();

        ServletRequest servletRequest = new ServletRequest(req, reqByte);
        // Console.WriteLine(req);
        // Console.WriteLine("Length: " + req.Length);
        string res;

        if (req.StartsWith("G"))
        {
            res = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-length: 513\r\n\r\n<!DOCTYPE html>\r\n<html>\n   <head>\n       <title>File Upload Form</title>\n   </head>\n   <body>\n       <h1>Upload file</h1>\n       <form method=\"POST\" action=\"upload\" enctype=\"multipart/form-data\">\n           <input type=\"file\" name=\"fileName\"/><br/><br/>\n           Caption: <input type=\"text\" name=\"caption\"<br/><br/><br/>\n           Date: <input type=\"date\" name=\"date\"<br/><br/><br/>\n           <input type=\"submit\" value=\"Submit\"/>\n       </form>\n   </body>\n</html>\r\n\r\n";
            Byte[] msg = System.Text.Encoding.ASCII.GetBytes(res + '\0');
            cls.Send(msg, msg.Length, 0);
        }
        else if (req.StartsWith("P"))
        {
            string folderPath = Directory.GetCurrentDirectory() + "/images/";
            using (var fs = new FileStream(folderPath + servletRequest.getFileName() + "_" + servletRequest.getCaption() +"_" + servletRequest.getDate() + ".png", FileMode.Create, FileAccess.Write))
            {
                ArrayList imgByte = servletRequest.getBytes();
                Byte[] bytes = (Byte[])imgByte.ToArray(typeof(Byte));
                fs.Write(bytes, 0, bytes.Length);
            }

            res = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE html>\r\n<html>\n   <head>\n       <title>File Upload</title>\n   </head>\n   <body>\n       ";
            res += "<h1>Files uploaded:</h1>\n";

            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] fiArr = di.GetFiles();
            foreach (FileInfo fri in fiArr)
            {
                res += "<p>- " + fri.Name + "</p>\n";
            }
            res += "</body>\n</html>\r\n\r\n";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(res + '\0');
            cls.Send(msg, msg.Length, 0);
        } 
        else if(req.StartsWith("C"))
        {
            string folderPath = Directory.GetCurrentDirectory() + "/images/";
            using (var fs = new FileStream(folderPath + servletRequest.getFileName() + "_" + servletRequest.getCaption() +"_" + servletRequest.getDate() + ".png", FileMode.Create, FileAccess.Write))
            {
                ArrayList imgByte = servletRequest.getBytes();
                Byte[] bytes = (Byte[]) imgByte.ToArray(typeof(Byte));
                fs.Write(bytes, 0, bytes.Length);
            }

            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] fiArr = di.GetFiles();
            StringBuilder json = new StringBuilder();
            json.Append("{\n");
            foreach (FileInfo fri in fiArr) { 
                String[] content = fri.Name.Split('_');
                if (content.Length < 3) continue;
                json.Append("  {\n");
                json.Append("  \"fileName\": \"" + fri.Name.Trim() + "\",\n");
                json.Append("  \"caption\": \"" + content[1].Trim() + "\",\n");
                json.Append("  \"date\": \"" + content[2].Split('.')[0].Trim() + "\"\n");
                json.Append("  },\n");
            }
            json.Append("}\n");
            json.Append("\0");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(json.ToString());
            cls.Send(msg, msg.Length, 0);
        }
        cls.Close();
    }

    public int countBoundary(string boundary, string req)
    {
        string[] lines = req.Split('\n');
        int count = 0;

        for(int i = 0; i < lines.Length; i++){
            if(lines[i].Contains(boundary)) count++;
        }

        return count;
    }

    static void Main(string[] args)
    {
        try
        {
            int port = 8888;
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(ipe); s.Listen(10);
            while (true)
            {
                Socket cls = s.Accept();
                UploadServlet uploadServlet = new UploadServlet(cls);
                Thread thread = new Thread(new ThreadStart(uploadServlet.threadMethod));
                thread.Start();
            }
            s.Close();
        }
        catch (SocketException e)
        {
            Console.WriteLine("Socket exception: {0}", e);
        }
    }
}

