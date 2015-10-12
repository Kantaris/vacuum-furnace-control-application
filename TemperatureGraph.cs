using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FurnaceController
{
    public partial class TemperatureGraph : Form
    {
        bool formClosing = false;
        public TemperatureGraph()
        {
            InitializeComponent();
            chart1.Titles[0].Text = "Temperatures - Braze #" + Properties.Settings.Default.brazeNbr + " " + DateTime.Now.ToString("yyyy-MM-dd");
            chart1.ChartAreas[0].AxisX.Title = "Time (Seconds)";
            chart1.ChartAreas[0].AxisY.Title = "Temperature (°C)";
            chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 8);
        }

        internal void setTemps(double tempSection1, double tempSection2, double tempSection3)
        {
            if (!formClosing)
            {
                chart1.Series[0].Points.AddY(tempSection1);
                chart1.Series[1].Points.AddY(tempSection2);
                chart1.Series[2].Points.AddY(tempSection3);
            }
        }

        private void TemperatureGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;
        }
    }
}
