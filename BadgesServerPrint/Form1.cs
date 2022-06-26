using BadgesServerPrint.Classes;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

//delegát pro zápis aktivit, ze všech vláken
delegate void DelDgwAddText(string textAddDgw);

namespace BadgesServerPrint
{
    public partial class Form1 : Form
    {
        const int portServer = 49600;
        const int countRowsDgw = 100;
        private SocketListener server;

        private Task task;
        public Form1()
        {
            InitializeComponent();

            server = new SocketListener(this);
            
            task = new Task(()=>server.StartListening (portServer));
            task.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblConnect.Text= "Info connect = Type: TCP / IP; IP: " + server.LocalIP.ToString() + "; Port: " + server.LocalPort;
            lblVersion.Text = "Version:" + Application.ProductVersion; 
        }

        /// <summary>
        ///zapíše záznam do datagridViewru
        /// </summary>
        public void AddDgwAction(string Description)
        {
            this.BeginInvoke((Action)delegate ()
            {
                if (dataGridView1.RowCount > countRowsDgw)
                    dataGridView1.Rows.RemoveAt(0);

                dataGridView1.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.sss"), Description);
            });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                server.BblListen = false ;
                task.Dispose();
            }
            catch (Exception ex)
            {
                EventLoging.Error(2, ex.ToString());
            }
        }
    }
}
