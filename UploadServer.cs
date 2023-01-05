using System; using System.IO; using System.Collections.Generic; using System.Linq; using System.Text; 
using System.Threading.Tasks; using System.Threading; using System.Net; using System.Net.Sockets;
public class DirListing {
        Socket cls = null;
        public DirListing(Socket socket) { this.cls = socket; }
        public void threadMethod() {
            Byte[] bytesReceived = new Byte[1];
            string a = "";
            while (true) {
                if ((cls.Receive(bytesReceived, bytesReceived.Length, 0) == 0) ||
                 (Encoding.ASCII.GetString(bytesReceived, 0, 1)[0] == '\0')) {
                    break;
                }
                a += Encoding.ASCII.GetString(bytesReceived, 0, 1);
            }
            Console.WriteLine(a);
            try
            {
                DirectoryInfo di = new DirectoryInfo(a);
                FileInfo[] fiArr = di.GetFiles();
                string files = "";
                foreach (FileInfo fri in fiArr) { files = files + fri.Name; }
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(files + '\0');
                cls.Send(msg, msg.Length, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Directory exception: {0}", e);
            }
            cls.Close();
        }
        static void Main(string[] args) {
            try {
                int port = 8888;  IPAddress address = IPAddress.Parse("127.0.0.1");
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

