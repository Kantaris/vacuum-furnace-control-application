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
    public partial class Setup : Form
    {
        BrazeSettings manualsettings = new BrazeSettings();
        public Setup()
        {
            InitializeComponent();
            speed1.SelectedIndex = speed1.Items.Count - 1;
            speed2.SelectedIndex = speed2.Items.Count - 1;
            speed3.SelectedIndex = speed3.Items.Count - 1;
        }

        public BrazeSettings getSettings()
        {
            BrazeSettings manualsettings = new BrazeSettings();
            manualsettings.brazeTemp = (double)brazetemp.Value;
            manualsettings.brazeTime = (int)brazetime.Value;
            manualsettings.syncL1 = (double)sync1.Value;
            manualsettings.syncL2 = (double)sync2.Value;
            manualsettings.pressure1 = (double)pressure1.Value;
            manualsettings.pressure2 = (double)pressure2.Value;
            manualsettings.pressure3 = (double)pressure3.Value;

            if (speed1.SelectedIndex == speed1.Items.Count - 1)
            {
                manualsettings.speedStartL1 = -1;
            }
            else
            {
                manualsettings.speedStartL1 = (speed1.SelectedIndex + 1) / 2;
            }
            
            if (speed2.SelectedIndex == speed2.Items.Count - 1)
            {
                manualsettings.speedL1L2 = -1;
            }
            else
            {
                manualsettings.speedL1L2 = (speed2.SelectedIndex + 1) / 2;
            }

            if (speed3.SelectedIndex == speed3.Items.Count - 1)
            {
                manualsettings.speedL2Braze = -1;
            }
            else
            {
                manualsettings.speedL2Braze = (speed3.SelectedIndex + 1) / 2;
            }

            return manualsettings;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
