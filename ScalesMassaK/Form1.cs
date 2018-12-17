using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScalesMassaK;
using System.IO.Ports;

namespace ScalesMassa
{
    public partial class Form1 : Form
    {
        private Scale scale = new Scale();

        private string unitStr = "";
        private string weightStr = "";

        private int countSend = 10;

        private int oldWeightValue = 0;

        private DateTime prevTime = DateTime.Now;

        private bool isConnected = false;
        private bool isSend = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            serialComboBox.Items.Clear();
            serialComboBox.Items.AddRange(ports);

            serialComboBox.Text = ScalesMassa.Properties.Settings.Default.ComPort;
        }

        private void SendValueToActiveWindow()
        {
            if (countSend < 1)
                return;

            Clipboard.SetText(weightStr);
            SendKeys.Send("^v");
            SendKeys.SendWait("{ENTER}");


            countSend--;

            label4.Text = countSend.ToString();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            try
            {

                int res = scale.ReadWeight();
                if (res == 0)
                {
                    int w = scale.Weight;
                    switch (scale.Division)
                    {
                        case 0: // миллиграммы
                            unitStr = "мг";
                            break;
                        case 1: // граммы
                            unitStr = "г";
                            break;
                        case 2: // киллограммы
                            w *= 1000; ;
                            unitStr = "г";
                            break;
                    }

                    weightStr = w.ToString();
                    label1.Text = weightStr + " " + unitStr;

                    if (oldWeightValue != w)
                    {
                        isSend = false;
                    }

                    if (scale.Stable == 1)
                    {
                        label3.Visible = true;

                        if (w > 10 && oldWeightValue != w)
                        {
                            oldWeightValue = w;
                            prevTime = DateTime.Now;
                            isSend = false;
                        }
                        else if (w > 10 && oldWeightValue == w && !isSend)
                        {
                            label5.Text = weightStr;
                            textBox1.Text += weightStr + "\r\n";
                            isSend = true;
                        }
                        /*else if (w > 10 && oldWeightValue == w && !isSend &&
                            ((DateTime.Now - prevTime).TotalMilliseconds > 2000))
                        {
                            label5.Text = weightStr;
                            textBox1.Text += weightStr + "\r\n";
                            isSend = true;
                        }*/
                    }
                    else
                    {
                        label3.Visible = false;
                        isSend = false;
                    }
                }
                else if (res == 1)
                {
                    ScaleCloseConnection();
                    int connectResult = ScaleOpenConnection();
                    if (connectResult == 0)
                    {
                        timer.Interval = 500;
                        trayIcon.Icon = ScalesMassa.Properties.Resources.connect1;
                        trayIcon.Text = "ScalesMassaK connected";
                        isConnected = true;
                        trayIcon.ShowBalloonTip(2000, "ScalesMassaK", "connected", ToolTipIcon.Info);
                    }
                    else
                    {
                        timer.Interval = 2000;
                        trayIcon.Icon = ScalesMassa.Properties.Resources.disconnect1;
                        trayIcon.Text = "ScalesMassaK disconnected";

                        if (isConnected)
                        {
                            trayIcon.ShowBalloonTip(2000, "ScalesMassaK", "disconnected", ToolTipIcon.Info);
                            isConnected = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            timer.Enabled = true;
        }

        private void serialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScalesMassa.Properties.Settings.Default.ComPort = serialComboBox.Text;
            ScalesMassa.Properties.Settings.Default.Save();
            ScaleCloseConnection();
        }

        private void ScaleCloseConnection()
        {
            scale.CloseConnection();
        }

        private int ScaleOpenConnection()
        {
            scale.Connection = ScalesMassa.Properties.Settings.Default.ComPort;
            return scale.OpenConnection();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                trayIcon.Visible = true;
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                trayIcon.Visible = false;
            }
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ScaleCloseConnection();
            trayIcon.Visible = false;
            System.Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
