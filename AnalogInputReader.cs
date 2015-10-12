using System;
using NationalInstruments.DAQmx;
using System.Windows.Forms;
using System.Collections;

namespace FurnaceController
{
    public class AnalogInputReader
    {
        AnalogMultiChannelReader myAnalogReader;
        string[] aiChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);
        Task myTask = new Task();
        Task runningTask;
        AsyncCallback myAsyncCallback;
        bool[] ao = new bool[4];
        int[] dataAI;
        double sampleRate;
        int samplesPerCallback;
        double[,] data;
        int nb = 0;
        public delegate void NewSamplesEventDelegate(object sender, EventArgs e);
        public event NewSamplesEventDelegate NewSamplesEvent;

        public AnalogInputReader(double sampleRate, int samplesPerCallback)
        {
       
                this.sampleRate = sampleRate;
                this.samplesPerCallback = samplesPerCallback;
				try{
					dataAI = new int[aiChannels.Length];      
				}
				catch (IndexOutOfRangeException ie)
				{
					if (aiChannels.Length < 1)
					{
						MessageBox.Show("Can't connect to USB-DAQ");
					}
					else
						MessageBox.Show(ie.Message);
				}
				catch (DaqException exception)
				{
					//Display Errors
					MessageBox.Show(exception.Message);
					myTask.Dispose();

					runningTask = null;
				}

        }

        public void addInputChannel(int channel, double minVoltage, double maxVoltage)
        {
            try
            {
                dataAI[channel] = nb;
                nb++;
                myTask.AIChannels.CreateVoltageChannel(aiChannels[channel], "",
                              AITerminalConfiguration.Rse, minVoltage, maxVoltage, AIVoltageUnits.Volts);
            }
          
            catch (Exception exception)
            {
                //Display Errors
                if (MessageBox.Show("Can't connect to USB-DAQ. Continue? ", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
                myTask.Dispose();

                runningTask = null;
            }
        }

        public void startSampling(){
            try
            {
                myTask.Timing.ConfigureSampleClock("", sampleRate, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

                //Verify the Task
                myTask.Control(TaskAction.Verify);

                runningTask = myTask;
                myAnalogReader = new AnalogMultiChannelReader(myTask.Stream);

                myAnalogReader.SynchronizeCallbacks = true;


                myAsyncCallback = new AsyncCallback(AnalogInCallback);
                myAnalogReader.BeginReadMultiSample(Convert.ToInt32(samplesPerCallback), myAsyncCallback,
                    myTask);
            }
           
            catch (Exception exception)
            {
                //Display Errors
                if (MessageBox.Show("Can't connect to USB-DAQ. Continue? ", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
                myTask.Dispose();

                runningTask = null;
            }

        }

        private void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (runningTask == ar.AsyncState)
                {
                    //Read the available data from the channels
                    data = myAnalogReader.EndReadMultiSample(ar);
                  
                   
                    NewSamplesEventDelegate temp = NewSamplesEvent;
                    if (temp != null)
                    {

                        temp(this, null);
                    }

                    myAnalogReader.BeginReadMultiSample(Convert.ToInt32(samplesPerCallback), myAsyncCallback, myTask);
                }
            }
            catch (DaqException exception)
            {
                //Display Errors
                MessageBox.Show(exception.Message);
                myTask.Dispose();
                runningTask = null;

            }

        }
        
        public double[] getSamples(int channel)
        {
            ArrayList n = new ArrayList();
            channel = dataAI[channel];
            for (int currentChannelIndex = 0; currentChannelIndex < data.GetLength(1); currentChannelIndex++)
            {
                n.Add(data.GetValue(channel, currentChannelIndex));
            }

            return (double[])n.ToArray(typeof(double));
        }
    }
}
