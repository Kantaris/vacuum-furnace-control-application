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
    public partial class SignalViewer : Form
    {

        public delegate void SignalEventDelegate(object sender, int[] e);
        public event SignalEventDelegate SignalEvent;

        public SignalViewer()
        {
            InitializeComponent();
            labelPower1.Text = "0.00";
            labelPower2.Text = "0.00";
            labelPower3.Text = "0.00";
        }

        internal void setSignalNP(double total)
        {
            labelNormPressure.Text = Math.Round(total, 2).ToString("#0.00");
        }

        internal void setSignalVP(double total)
        {
            labelVacuumPressure.Text = Math.Round(total, 2).ToString("#0.00");
        }

        internal void setSignalVentValve(bool p)
        {
            rBtnVentiValveOn.Checked = p;
            rBtnVentiValveOff.Checked = !p;
        }

        internal void setSignalVacuumValve(bool p)
        {
            rBtnVacuValveOn.Checked = p;
            rBtnVacuValveOff.Checked = !p;
        }

        internal void setSignalVacuumPump(bool p)
        {
            rBtnVacuumOn.Checked = p;
            rBtnVacuumOff.Checked = !p;
        }

        internal void setSignalHeater(bool p)
        {
            manoverOn.Checked = p;
            manoverOff.Checked = !p;
        }

        internal void setSignalBooster(bool p)
        {
            rBtnRootsOn.Checked = p;
            rBtnRootsOff.Checked = !p;
        }

        internal void setSignalCoolingFan(bool p)
        {
            rBtnCoolOn.Checked = p;
            rBtnCoolOff.Checked = !p;
        }

        internal void setSignalSmallN(bool p)
        {
            rBtnSmallOn.Checked = p;
            rBtnSmallOff.Checked = !p;
        }

        internal void setSignalLargeN(bool p)
        {
            rBtnLargeOn.Checked = p;
            rBtnLargeOff.Checked = !p; 
        }

        internal void setSignalT(int i, double p)
        {
            if(i == 0)
            { 
                labelPower1.Text = Math.Round(p, 2).ToString("#0.00");
            }
            else if(i == 1)
            { 
                labelPower2.Text = Math.Round(p, 2).ToString("#0.00");
            }
            else
            { 
                labelPower3.Text = Math.Round(p, 2).ToString("#0.00");
            }
        }

        internal void setSignalTemp(int i, double p)
        {
            if (i == 0)
            {
                labelTemperature1.Text = Math.Round(p, 2).ToString("#0.00");
            }
            else if (i == 1)
            {
                labelTemperature2.Text = Math.Round(p, 2).ToString("#0.00");
            }
            else
            {
                labelTemperature3.Text = Math.Round(p, 2).ToString("#0.00");
            }
        }

        Login lg;
        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg = new Login();
            if (lg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                groupBox5.Enabled = true;
                groupBox6.Enabled = true;
                groupBox7.Enabled = true;
                groupBox8.Enabled = true;
                groupBox9.Enabled = true;
                groupBox10.Enabled = true;
                groupBox11.Enabled = true;
                groupBox12.Enabled = true;
            }
        }

        private void manoverOn_CheckedChanged(object sender, EventArgs e)
        {
            if (manoverOn.Checked)
            {
                int[] send = { (int)ReportType.HeaterPowerSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.HeaterPowerSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnVacuumOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnVacuumOn.Checked)
            {
                int[] send = { (int)ReportType.VacuumPumpSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.VacuumPumpSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnSmallOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnSmallOn.Checked)
            {
                int[] send = { (int)ReportType.SmallN2ValveSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.SmallN2ValveSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnVacuValveOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnVacuValveOn.Checked)
            {
                int[] send = { (int)ReportType.VacuumValveSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.VacuumValveSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnVentiValveOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnVentiValveOn.Checked)
            {
                int[] send = { (int)ReportType.VentValveSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.VentValveSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnRootsOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnRootsOn.Checked)
            {
                int[] send = { (int)ReportType.BoosterPumpSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.BoosterPumpSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnLargeOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnLargeOn.Checked)
            {
                int[] send = { (int)ReportType.LargeN2ValveSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.LargeN2ValveSignal, 0 };
                SignalEvent(this, send);
            }
        }

        private void rBtnCoolOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnCoolOn.Checked)
            {
                int[] send = { (int)ReportType.CoolingFanSignal, 1 };
                SignalEvent(this, send);
            }
            else
            {
                int[] send = { (int)ReportType.CoolingFanSignal, 0 };
                SignalEvent(this, send);
            }
        }
        
    }
}
