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

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
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
            string testsr = "$jsdhfkjhsd,123,sdjhgsafd,sadfsdf,8*";
            byte summ = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < testsr.Length-1; i++)
            {
                sb.Append((char)testsr[i]);
                summ ^= Convert.ToByte(testsr[i]);
            }
            String result = sb.ToString();
            tbData.AppendText(result + "\r\n" + summ.ToString() );
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (inPortNameComboBox.Text == "COM3" && outPortNameComboBox.Text == "COM2")
            {
                btnStart.Enabled = false;
                _spManager.StartListening();
            }
        }   

      
    }
}
