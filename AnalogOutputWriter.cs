using System;
using System.Collections.Generic;
using System.Text;
using NationalInstruments.DAQmx;
using System.Windows.Forms;
using System.Diagnostics;

namespace FurnaceController
{
    public class AnalogOutputWriter
    {
        AnalogSingleChannelWriter myChannelWriter;
        string[] aoChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External);
        Task myTask = new Task();

        public AnalogOutputWriter(int channel, double minVoltage, double maxVoltage)
        {

            try
            {
                myTask.AOChannels.CreateVoltageChannel(aoChannels[channel], "GenCurrent", minVoltage, maxVoltage, AOVoltageUnits.Volts);

                myChannelWriter = new AnalogSingleChannelWriter(myTask.Stream);

            }
           
            catch (Exception exception)
            {
                if (MessageBox.Show("Can't connect to USB-DAQ. Continue? ", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
            }
        }

        public void setValue(double value)
        {
            try
            {
               myChannelWriter.WriteSingleSample(true, value);
            }
            catch (DaqException exception)
            {
                if (MessageBox.Show("Can't connect to USB-DAQ. Continue? ", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
            }

        }
    }
}
