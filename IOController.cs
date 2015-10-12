using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurnaceController
{
    public enum ReportType
    {
        HeaterPowerSignal,
        VacuumValveSignal,
        VentValveSignal,
        SmallN2ValveSignal,
        LargeN2ValveSignal,
        VacuumPumpSignal,
        BoosterPumpSignal,
        CoolingFanSignal,
        SetBrazeStart,
        SetStatus,
        SetGoalTemp,
        SetPhase

    }
    class IOController
    {

        public delegate void SamplesReceivedEventDelegate(object sender, bool ventilationValveValue, bool vacuumValveValue, bool vacuumPumpValue, bool manoverValue, bool rootsPumpValue, bool coolingFanValue, bool smallNitrogenValveValue, bool largeNitrogenValveValue);
        public event SamplesReceivedEventDelegate samplesReceivedEvent;
        AnalogOutputWriter[] ao = new AnalogOutputWriter[3];
        AnalogInputReader analogInputs;
        DigitalIO ventilationValve;
        DigitalIO vacuumValve;
        DigitalIO vacuumPump;
        DigitalIO rootsPump;
        DigitalIO smallNitrogenValve;
        DigitalIO largeNitrogenValve;
        DigitalIO coolingFan;
        DigitalIO manover;

        int[] tempChannels = new int[3];

        public IOController()
        {
            ao[0] = new AnalogOutputWriter(2, 5, 10);
            ao[1] = new AnalogOutputWriter(1, 5, 10);
            ao[2] = new AnalogOutputWriter(0, 5, 10);
            analogInputs = new AnalogInputReader(1000, 1000);

            analogInputs.addInputChannel(4, 0, 10);// vacuum pressure channel
            analogInputs.addInputChannel(3, 0, 10);//normal pressure
            analogInputs.addInputChannel(0, 0, 10);//temperature input channel1
            analogInputs.addInputChannel(1, 0, 10);//temperature input channel2
            analogInputs.addInputChannel(2, 0, 10);//temperature input channel3            

            rootsPump = new DigitalIO(6);
            vacuumPump = new DigitalIO(5);//vacuumPump
            coolingFan = new DigitalIO(7);
            vacuumValve = new DigitalIO(0); //vacuum valve
            smallNitrogenValve = new DigitalIO(2);
            largeNitrogenValve = new DigitalIO(3);
            ventilationValve = new DigitalIO(1); //ventilation valve
            manover = new DigitalIO(4);
            analogInputs.NewSamplesEvent += new AnalogInputReader.NewSamplesEventDelegate(ai_NewSamplesEvent);
            analogInputs.startSampling();

            tempChannels[0] = 2;
            tempChannels[1] = 1;
            tempChannels[2] = 0;
        }

        void ai_NewSamplesEvent(object sender, EventArgs e)
        {
            if (samplesReceivedEvent != null)
            {
                
                samplesReceivedEvent(this, ventilationValve.getWrittenValue(), vacuumValve.getWrittenValue(), vacuumPump.getWrittenValue(), manover.getWrittenValue(), rootsPump.getWrittenValue(), coolingFan.getWrittenValue(), smallNitrogenValve.getWrittenValue(), largeNitrogenValve.getWrittenValue());
            }
        }

        public void turnHeatersOn()
        {
            manover.writeValue(true);
        }
        public void turnHeatersOff()
        {
            manover.writeValue(false);
        }


        internal double[] getTempSamples(int section)
        {
           return analogInputs.getSamples(tempChannels[section]);
        }

        internal double[] getNormalPressureSamples()
        {
            return analogInputs.getSamples(3);
        }

        internal double[] getVacuumPressureSamples()
        {
            return analogInputs.getSamples(4);
        }

        internal bool isLargeN2ValveOpen()
        {
            return largeNitrogenValve.getWrittenValue();
        }

        internal void openLargeN2Valve()
        {
            largeNitrogenValve.writeValue(true);
        }

        internal void closeLargeN2Valve()
        {
            largeNitrogenValve.writeValue(false);
        }

        public void stopCoolingFan()
        {
            coolingFan.writeValue(false);
        }

        public void startCoolingFan()
        {
            coolingFan.writeValue(true);
        }

        public void signalWrite(int type, bool val)
        {
            switch (type)
            {
                case (int)ReportType.VacuumPumpSignal:
                    vacuumPump.writeValue(val);
                    break;
                case (int)ReportType.BoosterPumpSignal:
                    rootsPump.writeValue(val);
                    break;
                case (int)ReportType.SmallN2ValveSignal:
                    smallNitrogenValve.writeValue(val);
                    break;
                case (int)ReportType.LargeN2ValveSignal:
                    largeNitrogenValve.writeValue(val);
                    break;
                case (int)ReportType.VacuumValveSignal:
                    vacuumValve.writeValue(val);
                    break;
                case (int)ReportType.VentValveSignal:
                    ventilationValve.writeValue(val);
                    break;
                case (int)ReportType.CoolingFanSignal:
                    coolingFan.writeValue(val);
                    break;
                case (int)ReportType.HeaterPowerSignal:
                    manover.writeValue(val);
                    break;
            }


        }

        internal void shutDown()
        {
            ao[0].setValue(10);
            ao[1].setValue(10);
            ao[2].setValue(10);
            largeNitrogenValve.writeValue(false);
            smallNitrogenValve.writeValue(false);
            manover.writeValue(false);
        }

        internal void startVacuumPump()
        {
            vacuumPump.writeValue(true);
        }

        internal void stopVacuumPump()
        {
            vacuumPump.writeValue(false);
        }

        internal void startBoosterPump()
        {
            rootsPump.writeValue(true);
        }

        internal void stopBoosterPump()
        {

            rootsPump.writeValue(false); 
        }

        internal void openSmallN2Valve()
        {
            smallNitrogenValve.writeValue(true); 
        }

        internal void closeSmallN2Valve()
        {
            smallNitrogenValve.writeValue(false);
        }

        internal void openVacuumValve()
        {
            vacuumValve.writeValue(true);
        }

        internal void closeVacuumValve()
        {
            vacuumValve.writeValue(false);
        }

        internal void openVentilationValve()
        {
            ventilationValve.writeValue(true);
        }

        internal void closeVentilationValve()
        {
            ventilationValve.writeValue(false);
        }

        internal void setHeaterPower(int i, double cOutput)
        {
            // set up in the way that a control signal of 10V means no output effect
            // 4.8V results in full output effect
            ao[i].setValue(10 - (5.2 * cOutput));
        }

        internal void stopAll()
        {
            vacuumValve.writeValue(false);
            vacuumPump.writeValue(false);
            rootsPump.writeValue(false);
            smallNitrogenValve.writeValue(false);
            largeNitrogenValve.writeValue(false);
            coolingFan.writeValue(false);
            ventilationValve.writeValue(false);
            manover.writeValue(false); 
        }
    }
}
