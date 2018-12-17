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

        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            scale.Connection = serialComboBox.Text;
            //Log(serialComboBox.Text);

            int connectResult = scale.OpenConnection();
            if (connectResult == 0)
            {
                messageLabel.Text = "connect success";
            }
            else
            {
                messageLabel.Text = "connect error" + connectResult.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            serialComboBox.Items.Clear();
            serialComboBox.Items.AddRange(ports);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int res = scale.ReadWeight();

            if (res == 0)
            {
                label1.Text = scale.Weight.ToString();

                switch (scale.Division)
                {
                    case 0: // миллиграммы
                        label1.Text += " мг"; 
                        break;
                    case 1: // граммы
                        label1.Text += " гр";
                        break;
                    case 2: // киллограммы
                        label1.Text += " кг";
                        break;
                }

                messageLabel.Text = "read weight success";
            }
            else
            {
                messageLabel.Text = "read weight error" + res.ToString();
            }
            
        }
    }
}
