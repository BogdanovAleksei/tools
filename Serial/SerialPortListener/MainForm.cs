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
            notifyIcon1.Icon = null;
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }
            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);
            string readData = Encoding.ASCII.GetString(e.Data);
            tbData.AppendText("Read on " + inPortNameComboBox.Text + " : " + readData + "\r\n");
            tbData.AppendText(_spManager.Check(readData) + "\r\n");
            tbData.ScrollToCaret();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (inPortNameComboBox.Text != outPortNameComboBox.Text)
            {
                btnStop.Enabled = true;
                btnStart.Enabled = false;
                _spManager.StartListening();
            }
            else
            {
                MessageBox.Show(this,"Selected ports must not be the same!","Warning");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /// <summary>
            /// Автозапуск программы с заданными портами
            /// </summary>
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
                notifyIcon1.BalloonTipText = "Редирект портов запущен автоматически!";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipTitle = "Внимание!";
                notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
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
