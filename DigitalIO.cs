using System;
using System.Collections.Generic;
using System.Text;
using NationalInstruments.DAQmx;
using System.Windows.Forms;

namespace FurnaceController
{
    public class DigitalIO
    {
        string[] doline = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);
        string[] diline = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DILine, PhysicalChannelAccess.External);
        DigitalSingleChannelWriter writer;
        DigitalSingleChannelReader reader;
        Task doTask = new Task();
        Task diTask = new Task();

        bool ddata = false;

        public DigitalIO(int channel)
        {
            try
            {
            
                    doTask.DOChannels.CreateChannel(doline[channel], "", ChannelLineGrouping.OneChannelForAllLines);
                    diTask.DIChannels.CreateChannel(diline[channel], "", ChannelLineGrouping.OneChannelForAllLines);
                 
                    writer = new DigitalSingleChannelWriter(doTask.Stream);
                    reader = new DigitalSingleChannelReader(diTask.Stream);
                    
                
            }

            catch (Exception ex)
            {
                if (MessageBox.Show("Can't connect to USB-DAQ. Continue? ", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel){
                    Environment.Exit(0);
                }
            }

        }
        public void writeValue( bool data)
        {
            ddata = data;
            bool[] dataArray = new bool[1];
            dataArray[0] = !data;
            writer.WriteSingleSampleMultiLine(true, dataArray);
        }

        public bool getWrittenValue()
        {
            return ddata;
        }

        public bool readValue(){
            bool readData;
             readData = reader.ReadSingleSampleSingleLine();
             return !readData;
        }
    }
}
