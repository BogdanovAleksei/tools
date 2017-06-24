using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;
using System.IO.Ports;
using System.Configuration;


namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
     //   private System.Windows.Forms.NotifyIcon notifyIcon1;
        public MainForm()
        {
            InitializeComponent();

            UserInitialization();
/*
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            notifyIcon1.Icon = new Icon(this.Icon, 40, 40);
            notifyIcon1.Text = "Плыву по течению";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
*/
        }

      
        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            inPortNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            inBaudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            inDataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            inParityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            inStopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            outPortNameComboBox.DataSource = mySerialSettings.OutPortNameCollection;
            outBaudRateComboBox.DataSource = mySerialSettings.outBaudRateCollection;
            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {

            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string readData = Encoding.ASCII.GetString(e.Data);
           // string str = _spManager.Check(readData);
            tbData.AppendText("Read on " + inPortNameComboBox.Text + " : " + readData + "\r\n");
            tbData.AppendText(_spManager.Check(readData) + "\r\n");

         //   _spManager.Write("Debug: " + readData + ";\r\n");
                
            tbData.ScrollToCaret();

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (inPortNameComboBox.Text != outPortNameComboBox.Text)
            {
                btnStop.Enabled = true;
                btnStart.Enabled = false;
                _spManager.StartListening();
               // this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                MessageBox.Show(this,"Selected ports must not be the same!","Warning");
            }
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

         }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /// <summary>
            /// Автозапуск программы с заданными портами
            /// COM3 для входящего порта
            /// COM2 для исходящего порта
            /// </summary>

            
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            if (mySerialSettings.PortNameCollection.Contains(mySerialSettings.inPortNemeDefault)) inPortNameComboBox.Text = mySerialSettings.inPortNemeDefault;
            if (mySerialSettings.PortNameCollection.Contains(mySerialSettings.outPortNemeDefault)) outPortNameComboBox.Text = mySerialSettings.outPortNemeDefault;
            if (inPortNameComboBox.Text == mySerialSettings.inPortNemeDefault && outPortNameComboBox.Text == mySerialSettings.outPortNemeDefault)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                _spManager.StartListening();
                this.WindowState = FormWindowState.Minimized;
                notifyIcon1.BalloonTipText = "Редирект портов запущен!";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipTitle = "Внимание!";
                notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            // Activate the form.
            this.Activate();
        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            // Activate the form.
            this.Activate();
        }

        private void toolStripExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
