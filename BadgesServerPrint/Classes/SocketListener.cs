using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Linq;

namespace BadgesServerPrint.Classes
{
    public class SocketListener
    {
        private DelDgwAddText DelDgwAdd{get; set;}
        private Form1 mainForm{get; set;}
        private Socket hostSocket{get; set;}
        public bool BblListen{get;set;}
        public IPAddress LocalIP
        {
            get=> Dns.GetHostAddresses(Dns.GetHostName()).First(IPA=>IPA.AddressFamily==AddressFamily.InterNetwork);
        }

        private int _localPort { get; set; }
        /// <summary>
        ///nastaví preferovaný naslouchací port zařízení
        ///defaultně je nastavený port 49600- (v rozsahu 49152 až 65535, vyhrazené pro dynamické přidělování a soukromé využití, nejsou pevně přiděleny žádné aplikaci)
        /// </summary>
        public int LocalPort
        {
            get => _localPort;
        }

        public SocketListener(Form1 fm)
        {
            BblListen = true;
            this.mainForm = fm;
            DelDgwAdd = new DelDgwAddText(fm.AddDgwAction);
        }

        public void StartListening(int localPort)
        {
            _localPort = localPort;
            Socket receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostIpEndPoint = new IPEndPoint(IPAddress.Parse(LocalIP.ToString()), LocalPort);
            receiveSocket.Bind(hostIpEndPoint);
            receiveSocket.Listen(1);
            EventLoging.Info(1, "Start server." + LocalIP.ToString() + ":" + LocalPort.ToString());
            DelDgwAdd("Start server:" + LocalIP.ToString() +":" + LocalPort.ToString());
            

            while (BblListen)
            {
                hostSocket = receiveSocket.Accept();
                Thread thread = new Thread(new ThreadStart(threadImage));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void threadImage()
        {
            try
            {
                hostSocket.ReceiveTimeout=10000;
                byte[] m = new byte[17];// velikost chartu se udává podle poštu znaků ((počet znaků*7bytů)+10)
                int dataSizeMessangeClient = hostSocket.Receive(m, 0, m.Length, SocketFlags.None);
                string messangeClient = Encoding.ASCII.GetString(m, 0, dataSizeMessangeClient);
                int countPagesPrint = int.Parse(messangeClient);

                byte[] maxSizeMessage = new byte[1024*1024*2];
                int dataSize = hostSocket.Receive(maxSizeMessage);
                
                if (dataSize > 0)
                {
                    MemoryStream ms = new MemoryStream(maxSizeMessage, 0, dataSize, true);
                    Image img = Image.FromStream(ms);

                    ms.Close();
                    DelDgwAdd("Load image.");
                  
                    EventLoging.Info(2, "Load image.");
                    AppPrint mPrint = new AppPrint(mainForm,img, countPagesPrint);
                    DelDgwAdd("Start print.");
                    EventLoging.Info(3, "Start print.");
                }
            }
            catch (Exception ex)
            {
                DelDgwAdd("Error:"+ ex);
                EventLoging.Error(1, ex.ToString());
            }
        }
    }
}
