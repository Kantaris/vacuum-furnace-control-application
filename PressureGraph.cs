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
    public partial class PressureGraph : Form
    {
        bool formClosing = false;
        public PressureGraph()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisY.IsLogarithmic = true;
            chart1.ChartAreas[0].AxisY.LogarithmBase = 10;
            chart1.Titles[0].Text = "Temperatures - Braze #" + Properties.Settings.Default.brazeNbr + " " + DateTime.Now.ToString("yyyy-MM-dd");
            chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 8);
            chart1.ChartAreas[0].AxisX.Title = "Time (Seconds)";
            chart1.ChartAreas[0].AxisY.Title = "Pressure (Bar)";
           
           // chart1.ChartAreas[0].AxisY.Minimum = 0.00000001;
        }

        internal void setPressure(double vacuumPressure, double normalPressure)
        {
            if (!formClosing)
            {
                chart1.Series[0].Points.AddY(vacuumPressure);
                if (normalPressure < 0.1)
                    normalPressure = 0.001;
                chart1.Series[1].Points.AddY(normalPressure);
            }
        }

        private void PressureGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;
        }
    }
}
