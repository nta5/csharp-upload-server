using System; 
using System.IO; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 
using System.Threading; 
using System.Net; 
using System.Net.Sockets;

public class DirListing {

        Socket cls = null;

        public DirListing(Socket socket) { this.cls = socket; }

        public void threadMethod() {
                    Byte[] bytesReceived = new Byte[1];
                    string req = "";
                    bool stop = false;
                    IAsyncResult result;
                    Action action = () =>
                    {
                        while (!stop) {
                            // Commented out to test if the second clause after || prevents the Post req from printing 
                            // if ((cls.Receive(bytesReceived, bytesReceived.Length, 0) == 0) ||
                            // (Encoding.ASCII.GetString(bytesReceived, 0, 1)[0] == '\0')) {
                            //     break;
                            // }
                            if ((cls.Receive(bytesReceived, bytesReceived.Length, 0) == 0)) {
                                break;
                            }
                            req += Encoding.ASCII.GetString(bytesReceived, 0, 1);
                        }
                    };
                    result = action.BeginInvoke(null, null);
                    if (!result.AsyncWaitHandle.WaitOne(1000)) stop = true;

                    Console.WriteLine(req);
                    string res;
                    if(req.StartsWith("G")){
                        res = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-length: 513\r\n\r\n<!DOCTYPE html>\r\n<html>\n   <head>\n       <title>File Upload Form</title>\n   </head>\n   <body>\n       <h1>Upload file</h1>\n       <form method=\"POST\" action=\"upload\" enctype=\"multipart/form-data\">\n           <input type=\"file\" name=\"fileName\"/><br/><br/>\n           Caption: <input type=\"text\" name=\"caption\"<br/><br/><br/>\n           Date: <input type=\"date\" name=\"date\"<br/><br/><br/>\n           <input type=\"submit\" value=\"Submit\"/>\n       </form>\n   </body>\n</html>\r\n\r\n";
                        cls.Send(System.Text.Encoding.ASCII.GetBytes(res + '\0'), 0);
                    }else{
                        res = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE html>\r\n<html>\n   <head>\n       <title>File Upload</title>\n   </head>\n   <body>\n       <h1>This is after POST</h1>\n </body>\n</html>\r\n\r\n";
                        cls.Send(System.Text.Encoding.ASCII.GetBytes(res + '\0'), 0);
                    }
                    cls.Close();
        }

        static void Main(string[] args) {
            try {
                int port = 8888;  
                IPAddress address = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.Bind(ipe); s.Listen(10);
                while (true) {
                    Socket cls = s.Accept();
                    DirListing dirListing = new DirListing(cls);
                    Thread thread = new Thread(new ThreadStart(dirListing.threadMethod));
                        thread.Start();
                }
                s.Close();
            }
            catch (SocketException e) {
                Console.WriteLine("Socket exception: {0}", e);
            }
        }
}

