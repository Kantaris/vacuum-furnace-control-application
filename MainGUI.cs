using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Xml;

namespace FurnaceController
{
    public partial class MainGUI : Form
    {
        BrazeSettings cu90settings = new BrazeSettings();
        BrazeSettings cu70settings = new BrazeSettings();
        BrazeSettings cu50settings = new BrazeSettings();
        BrazeSettings cuslowsettings = new BrazeSettings();
        BrazeSettings manualsettings = new BrazeSettings();
        ProcessController processC;

        public MainGUI()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            processC = new ProcessController();
            processC.pressureErrorEvent += new ProcessController.PressureErrorEventDelegate(processC_pressureErrorEvent);
            processC.innerHeaterErrorEvent += new ProcessController.InnerHeaterErrorEventDelegate(processC_innerHeaterErrorEvent);
            processC.middleHeaterErrorEvent += new ProcessController.MiddleHeaterErrorEventDelegate(processC_middleHeaterErrorEvent);
            processC.outerHeaterErrorEvent += new ProcessController.OuterHeaterErrorEventDelegate(processC_outerHeaterErrorEvent);
            processC.powerUpdateEvent += new ProcessController.PowerUpdateEventDelegate(processC_powerUpdateEvent);
            processC.heaterRestartEvent += new ProcessController.HeaterRestartEventDelegate(processC_heaterRestartEvent);
            processC.temperatureUpdateEvent += new ProcessController.TemperatureUpdateEventDelegate(processC_temperatureUpdateEvent);
            processC.stageEvent += new ProcessController.StageEventDelegate(processC_stageEvent);
            processC.logEvent += new ProcessController.LogEventDelegate(processC_logEvent);
            processC.pressureNUpdateEvent += new ProcessController.PressureNUpdateEventDelegate(processC_pressureNUpdateEvent);
            processC.pressureVUpdateEvent += new ProcessController.PressureVUpdateEventDelegate(processC_pressureVUpdateEvent);
            processC.updateTimeEvent += new ProcessController.UpdateTimeEventDelegate(processC_updateTimeEvent);
            processC.completeEvent += new ProcessController.CompleteEventDelegate(processC_completeEvent);

            //settings for Cu-70min mode
            cu70settings.modeName = "Cu - 70min";
            cu70settings.speedStartL1 = -1; // -1 = Full Speed
            cu70settings.speedL1L2 = -1;
            cu70settings.speedL2Braze = -1;
            cu70settings.syncL1 = 1060;
            cu70settings.syncL2 = 1060;
            cu70settings.brazeTemp = 1121;
            cu70settings.brazeTime = 70;
            cu70settings.pressure1 = 1.5;
            cu70settings.pressure2 = 1.8;
            cu70settings.pressure3 = 2.3;

            //settings for Cu-50min mode
            cu50settings.modeName = "Cu - 50min";
            cu50settings.speedStartL1 = -1;
            cu50settings.speedL1L2 = -1;
            cu50settings.speedL2Braze = -1;
            cu50settings.syncL1 = 1060;
            cu50settings.syncL2 = 1060;
            cu50settings.brazeTemp = 1121;
            cu50settings.brazeTime = 50;
            cu50settings.pressure1 = 1.5;
            cu50settings.pressure2 = 1.8;
            cu50settings.pressure3 = 2.3;

            //settings for Cu-90min mode
            cu90settings.modeName = "Cu - 90min";
            cu90settings.speedStartL1 = -1; // -1 = Full Speed
            cu90settings.speedL1L2 = -1;
            cu90settings.speedL2Braze = -1;
            cu90settings.syncL1 = 1060;
            cu90settings.syncL2 = 1060;
            cu90settings.brazeTemp = 1121;
            cu90settings.brazeTime = 90;
            cu90settings.pressure1 = 1.5;
            cu90settings.pressure2 = 1.8;
            cu90settings.pressure3 = 2.3;

            //settings for Cu-slow mode
            cuslowsettings.modeName = "Cu - Slow - 70min";
            cuslowsettings.speedStartL1 = -1;
            cuslowsettings.speedL1L2 = 4.7;
            cuslowsettings.speedL2Braze = 2.5;
            cuslowsettings.syncL1 = 200;
            cuslowsettings.syncL2 = 1060;
            cuslowsettings.brazeTemp = 1121;
            cuslowsettings.brazeTime = 70;
            cuslowsettings.pressure1 = 1.5;
            cuslowsettings.pressure2 = 1.8;
            cuslowsettings.pressure3 = 2.0;
        }

        void processC_completeEvent(object sender, EventArgs e, string errorMessage, bool success)
        {
            statusLabel.Text = errorMessage;
            buttonStart.Enabled = success;
            if (coolingFansCheckBox.Checked)
            {
                processC.startCoolingFan();
            }
            else
            {
                processC.stopCoolingFan();
            }
            List<string> logList = new List<string>();
            List<bool> errorTypeList = new List<bool>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                logList.Add(listView1.Items[i].Text);
                if (listView1.Items[i].ForeColor == Color.Red)
                {
                    errorTypeList.Add(true);
                }
                else
                {
                    errorTypeList.Add(false);
                }
            }
            string xmlString = processC.writeXml(logList, errorTypeList);
            File.WriteAllText(Application.StartupPath + "\\stats\\" + (Properties.Settings.Default.brazeNbr - 1) + ".xml", xmlString);

        }

        void processC_updateTimeEvent(object sender, EventArgs e, string elapsedTime, string brazingTime, int progress)
        {
            if (progress <= 100)
            {
                progressBar1.Value = progress;
            }
            elapsedtimelabel.Text = "Elapsed time: " + elapsedTime;
            brazetimeLabel.Text = "Brazing time: " + brazingTime;
        }

        void processC_pressureVUpdateEvent(object sender, EventArgs e, float value)
        {
            labelVacuumPressure.Text = value.ToString("0.00E+00");
        }

        void processC_pressureNUpdateEvent(object sender, EventArgs e, float value)
        {
            labelNormPressure.Text = value.ToString("0.00E+00");
        }

        void processC_stageEvent(object sender, EventArgs e, string text)
        {
            currentStepLabel.Text = text;
        }

        void processC_logEvent(object sender, EventArgs e, string logText)
        {
            listView1.Items.Add(logText);
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
        }

        void processC_temperatureUpdateEvent(object sender, EventArgs e, float[] temps)
        {
            labelTemperature1.Text = Convert.ToInt32(temps[0]).ToString();
            labelTemperature2.Text = Convert.ToInt32(temps[0]).ToString();
            labelTemperature3.Text = Convert.ToInt32(temps[0]).ToString();
        }

        void processC_heaterRestartEvent(object sender, EventArgs e)
        {
            listView1.Items.Add(DateTime.Now.ToString() + " Pressure lower, starting heaters again");
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            toolStripStatusLabel1.Text = "";
        }

        void processC_powerUpdateEvent(object sender, EventArgs e, string[] power)
        {
            labelPower1.Text = power[0];
            labelPower2.Text = power[1];
            labelPower3.Text = power[2];
        }

        void processC_outerHeaterErrorEvent(object sender, EventArgs e, string logError, string errorMessage)
        {
            colorBoxOuter.BackColor = Color.Red;
            errorReport(logError, errorMessage);
        }

        void processC_middleHeaterErrorEvent(object sender, EventArgs e, string logError, string errorMessage)
        {
            colorBoxOuter.BackColor = Color.Red;
            errorReport(logError, errorMessage);
        }

        void processC_innerHeaterErrorEvent(object sender, EventArgs e, string logError, string errorMessage)
        {
            colorBoxOuter.BackColor = Color.Red;
            errorReport(logError, errorMessage);
        }

        void processC_pressureErrorEvent(object sender, EventArgs e, string logError, string errorMessage)
        {
            colorBoxPressure.BackColor = Color.Red;
            errorReport(logError, errorMessage);
        }

        private void errorReport(string logError, string errorMessage)
        {
            listView1.Items.Add(logError).ForeColor = Color.Red;
            toolStripStatusLabel1.Text = errorMessage;
            toolStripStatusLabel1.ForeColor = Color.Red;
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            processC.startProcess();
            listView1.Items.Clear();
            if (!processC.checkPressureError())
            {
                if (MessageBox.Show(this, "Pressure meters are showing abnormal values for start. Start anyway?", "Abnormal pressure", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    colorBoxPressure.BackColor = Color.Red;
                    listView1.Items.Add(DateTime.Now.ToString() + " Pressure at abnormal levels at start up").ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = "Pressure at abnormal levels at start up. Not starting";
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    return;
                }
            }

            string brazeText = "Braze #" + Properties.Settings.Default.brazeNbr + " " + DateTime.Now.ToString("yyyy-MM-dd");
            brazeStatLabel.Text = brazeText;
            Properties.Settings.Default.brazeNbr++;
            Properties.Settings.Default.Save();
            statusLabel.Text = "Status: Running";
            startTimeLabel.Text = "Start time: " + DateTime.Now.ToString("HH:mm:ss");

            processC.setStartValues(brazeText);

            if (comboBox1.SelectedIndex == 0)
            {
                processC.setProcessSettings(cu70settings);
                listView1.Items.Add(DateTime.Now.ToString() + " The process of Cu - 70min is started.");
            }
            else if (comboBox1.SelectedIndex == 1)
            {

                processC.setProcessSettings(cu50settings);
                listView1.Items.Add(DateTime.Now.ToString() + " Starting the Cu - 50min process.");
            }
            else if (comboBox1.SelectedIndex == 2)
            {

                processC.setProcessSettings(cu90settings);
                listView1.Items.Add(DateTime.Now.ToString() + " Starting the Cu - 90min process.");
            }
            else if (comboBox1.SelectedIndex == 3)
            {

                processC.setProcessSettings(cuslowsettings);
                listView1.Items.Add(DateTime.Now.ToString() + " Starting the Cu_slow process.");
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                manualsettings.modeName = "Manual Settings";
                processC.setProcessSettings(manualsettings);
                listView1.Items.Add(DateTime.Now.ToString() + " Starting the manually set up process.");
                listView1.Items.Add("Braze time: " + manualsettings.brazeTime + " minutes");
                listView1.Items.Add("Braze temperature: " + manualsettings.brazeTemp + " °C");
                listView1.Items.Add("Sync level 1: " + manualsettings.syncL1 + " °C");
                listView1.Items.Add("Sync level 2: " + manualsettings.syncL2 + " °C");
                if (manualsettings.speedStartL1 > 0)
                {
                    listView1.Items.Add("Heating speed to level 1: " + manualsettings.speedStartL1 + " °C / minute");
                }
                else
                {
                    listView1.Items.Add("Heating speed to level 1: Full speed ");
                }
                if (manualsettings.speedL1L2 > 0)
                {
                    listView1.Items.Add("Heating speed between level 1 and 2: " + manualsettings.speedL1L2 + " °C / minute");
                }
                else
                {
                    listView1.Items.Add("Heating speed between level 1 and 2: Full speed ");
                }
                if (manualsettings.speedL2Braze > 0)
                {
                    listView1.Items.Add("Heating speed between level 2 and braze temperature: " + manualsettings.speedL2Braze + " °C / minute");
                }
                else
                {
                    listView1.Items.Add("Heating speed between level 2 and braze temperature: Full speed ");
                }
                listView1.Items.Add("Pressure between 1020°C and 930°C: " + manualsettings.pressure1 + " Bar");
                listView1.Items.Add("Pressure between 930°C and 850°C: " + manualsettings.pressure2 + " Bar");
                listView1.Items.Add("Pressure between 850°C and 150°C: " + manualsettings.pressure3 + " Bar");
            }

            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            comboBox1.Enabled = false;
            buttonStart.Enabled = false;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            processC.stop();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (processC.pauseProcess())
            {
                buttonPause.Text = "CONTINUE";
            }
            else
            {
                buttonPause.Text = "PAUSE";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            processC.stop();
        }

        private void signalViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processC.signalViewerShow();
        }

        private void temperatureGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processC.temperatureGraphShow();
        }

        private void pressureGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processC.pressureGraphShow();
        }
        History hw;
        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hw = new History();
            hw.Show();
        }
        Login lg;
        Setup setupWindow;
        private void manualSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox1.Items.Count - 1)
            {
                lg = new Login();
                if (lg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    setupWindow = new Setup();
                    if (setupWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        manualsettings = setupWindow.getSettings();
                    }
                    else
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                }
            }
        }
        AboutBox ab;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ab = new AboutBox();
            ab.Show();
        }
        Users users;
        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            users = new Users();
            users.Show();
        }

        private void coolingFansCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (coolingFansCheckBox.Checked)
            {
                processC.startCoolingFan();
            }
            else
            {
                processC.stopCoolingFan();
            }
        }

        private void reopenLargeN2ValveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processC.reOpenLargeN2Valve())
            {

                listView1.Items.Add(DateTime.Now.ToString() + " The large Nitrogen valve is opened.");
                toolStripStatusLabel1.Text = "";
                colorBoxPressure.BackColor = Color.Lime;
                listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            }
        }

        private void startCoolingFanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processC.startCoolingFan();
        }

        private void stopCoolingFanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processC.stopCoolingFan();
        }

    }

    public class BrazeSettings
    {
        public double syncL1, syncL2;
        public double speedStartL1, speedL1L2, speedL2Braze;
        public double brazeTemp;
        public int brazeTime;
        public double pressure1, pressure2, pressure3;
        public double coolingFinishTemp;
        public string modeName;

    }
}