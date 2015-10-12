using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Xml;

namespace FurnaceController
{

    class ProcessController
    {
        public delegate void PressureErrorEventDelegate(object sender, EventArgs e, string logError, 
            string errorMessage);
        public event PressureErrorEventDelegate pressureErrorEvent;
        public delegate void InnerHeaterErrorEventDelegate(object sender, EventArgs e, string logError, string errorMessage);
        public event InnerHeaterErrorEventDelegate innerHeaterErrorEvent;
        public delegate void MiddleHeaterErrorEventDelegate(object sender, EventArgs e, string logError, string errorMessage);
        public event MiddleHeaterErrorEventDelegate middleHeaterErrorEvent;
        public delegate void OuterHeaterErrorEventDelegate(object sender, EventArgs e, string logError, string errorMessage);
        public event OuterHeaterErrorEventDelegate outerHeaterErrorEvent;
        public delegate void PowerUpdateEventDelegate(object sender, EventArgs e, string[] power);
        public event PowerUpdateEventDelegate powerUpdateEvent;
        public delegate void HeaterRestartEventDelegate(object sender, EventArgs e);
        public event HeaterRestartEventDelegate heaterRestartEvent;
        public delegate void TemperatureUpdateEventDelegate(object sender, EventArgs e, float[] temps);
        public event TemperatureUpdateEventDelegate temperatureUpdateEvent;
        public delegate void LogEventDelegate(object sender, EventArgs e, string logText);
        public event LogEventDelegate logEvent;
        public delegate void StageEventDelegate(object sender, EventArgs e, string text);
        public event StageEventDelegate stageEvent;
        public delegate void PressureNUpdateEventDelegate(object sender, EventArgs e, float value);
        public event PressureNUpdateEventDelegate pressureNUpdateEvent;
        public delegate void PressureVUpdateEventDelegate(object sender, EventArgs e, float value);
        public event PressureVUpdateEventDelegate pressureVUpdateEvent;
        public delegate void UpdateTimeEventDelegate(object sender, EventArgs e, string elapsedTime, string brazeTime, int progress);
        public event UpdateTimeEventDelegate updateTimeEvent;
        public delegate void CompleteEventDelegate(object sender, EventArgs e, string errorMessage, bool success);
        public event CompleteEventDelegate completeEvent;

        public enum Phase
        {
            Idle,
            Started,
            Heating,
            Brazing,
            Cooling,
            Finished,
            Failed

        }
        Phase phase = Phase.Idle;
        TemperatureController tempC;
        IOController ioC;
        LiveUploader liveUploader;
        SignalViewer signalW;
        TemperatureGraph tGraph;
        PressureGraph pGraph;

        bool flagPower = false;
        bool flagTempError = false;
        float vacuumPressure = 0;
        float normalPressure = 0;
        float[] tempSection = new float[3];
        float[] power = new float[3];
        string currentStepString = "";
        double targetTemp;
        bool allowLargeN2valve = true;
        int largeN2OpenTime = -1;
        List<double> vpList, npList;
        List<double>[] tempList = new List<double>[3];
        int[] fullPowerCounter = new int[3];

        bool paused = false;
        bool pressurePaused = false;
        bool stoppedHeating = false;
        bool stoppedCooling = false;
        double brazeTemp = 0;
        DateTime beginBrazeTime;
        DateTime beginTime;
        int brazeTime = 0;

        BackgroundWorker processWorker = new System.ComponentModel.BackgroundWorker();

        public ProcessController()
        {
            this.processWorker.WorkerReportsProgress = true;
            this.processWorker.WorkerSupportsCancellation = true;
            this.processWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.processWorker_DoWork);
            this.processWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.processWorker_RunWorkerCompleted);
            this.processWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.processWorker_ProgressChanged);

            for (int i = 0; i < tempSection.Length; i++)
            {
                tempSection[i] = 0;
                tempList[i] = new List<double>();
                fullPowerCounter[i] = 0;
            }

            ioC = new IOController();
            ioC.samplesReceivedEvent += new IOController.SamplesReceivedEventDelegate(ioC_samplesReceivedEvent);
            liveUploader = new LiveUploader();
            liveUploader.status = "Idle";
        }

        void ioC_samplesReceivedEvent(object sender, bool ventilationValveValue, bool vacuumValveValue, 
            bool vacuumPumpValue, bool manoverValue, bool rootsPumpValue, bool coolingFanValue, 
            bool smallNitrogenValveValue, bool largeNitrogenValveValue)
        {
            getVacuumPressure();
            getNormalPressure();
            getTemperatures();
            storeMeasurements();

            updateGraphs();
            updateSignalViewer(ventilationValveValue, vacuumValveValue, vacuumPumpValue, manoverValue, 
                rootsPumpValue, coolingFanValue, smallNitrogenValveValue, largeNitrogenValveValue);

            temperatureErrorCheck(); // checks if the temperature meters are broken
            vacuumPressureCheck(); // checks if pressure is over 0.002 bar when heating
            heaterErrorCheck(); // checks if heaters are working
            normalPressureCheck(); //checks that pressure is increasing when the large N2 valve is open.

            temperatureControl();
            updateTime();
            liveUploader.uploadToWebServer();
        }

        public void signalViewerShow()
        {
            signalW = new SignalViewer();
            signalW.SignalEvent += new SignalViewer.SignalEventDelegate(signalW_SignalEvent);
            signalW.Show();
        }

        void signalW_SignalEvent(object sender, int[] e)
        {

            int type = e[0];
            bool val = (e[1] == 1);
            signalWrite(type, val);
        }

        private void updateSignalViewer(bool ventilationValveValue, bool vacuumValveValue, bool vacuumPumpValue, bool manoverValue, bool rootsPumpValue, bool coolingFanValue, bool smallNitrogenValveValue, bool largeNitrogenValveValue)
        {
            if (signalW != null)
            {
                signalW.setSignalVentValve(ventilationValveValue);
                signalW.setSignalVacuumValve(vacuumValveValue);
                signalW.setSignalVacuumPump(vacuumPumpValue);
                signalW.setSignalHeater(manoverValue);
                signalW.setSignalBooster(rootsPumpValue);
                signalW.setSignalCoolingFan(coolingFanValue);
                signalW.setSignalSmallN(smallNitrogenValveValue);
                signalW.setSignalLargeN(largeNitrogenValveValue);
            }
        }

        private void updateGraphs()
        {
            if (phase == Phase.Started || phase == Phase.Heating || phase == Phase.Brazing || phase == Phase.Cooling)
            {
                if (tGraph != null)
                    tGraph.setTemps(tempList[0][tempList[0].Count - 1], tempList[1][tempList[1].Count - 1], tempList[2][tempList[2].Count - 1]);
                if (pGraph != null)
                    pGraph.setPressure(vpList[vpList.Count - 1], npList[npList.Count - 1]);
            }
        }

        public void temperatureGraphShow()
        {
            if (phase != FurnaceController.ProcessController.Phase.Idle)
            {
                tGraph = new TemperatureGraph();
                for (int i = 0; i < tempList[0].Count; i++)
                {
                    tGraph.setTemps(tempList[0][i], tempList[1][i], tempList[2][i]);
                }
                tGraph.Show();
            }
        }

        public void pressureGraphShow()
        {
            if (phase != FurnaceController.ProcessController.Phase.Idle)
            {
                pGraph = new PressureGraph();
                for (int i = 0; i < npList.Count; i++)
                {
                    pGraph.setPressure(vpList[i], npList[i]);
                }
                pGraph.Show();
            }
        }

        private void heaterErrorCheck()
        {
            if (flagPower)
            {
                // get section with the lowest temp
                int index = getLowestTemperature();

                //check if it has reached it's goal
                if (tempSection[index] < targetTemp - 10)
                {
                    //check if heater has been running on full power for at least 60 min
                    if (fullPowerCounter[index] > 60 * 60)
                    {
                        //get what the temperature was 60 minutes ago
                        double oldValue = tempList[index][tempList[index].Count - 60 * 60];

                        if (tempSection[index] <= oldValue)
                        {
                            // old temperature is higher than current, something is wrong
                            if (index == 0)
                            {
                                if (innerHeaterErrorEvent != null)
                                {
                                    innerHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with the inner section heater, stopping", "Problem with the inner section heater");
                                }
                                liveUploader.addToLog(DateTime.Now.ToString() + " Problem with the inner section heater, stopping", true);

                            }
                            else if (index == 1)
                            {
                                if (middleHeaterErrorEvent != null)
                                {
                                    middleHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with the middle section heater, stopping", "Problem with the middle section heater");
                                }
                                liveUploader.addToLog(DateTime.Now.ToString() + " Problem with the middle section heater, stopping", true);
                            }
                            else
                            {
                                if (outerHeaterErrorEvent != null)
                                {
                                    outerHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with the outer section heater, stopping", "Problem with the outer section heater");
                                }
                                liveUploader.addToLog(DateTime.Now.ToString() + " Problem with the outer section heater, stopping", true);
                            }
                            flagPower = false;
                            ioC.turnHeatersOff();
                            stoppedHeating = true;

                        }
                    }

                }
            }
        }

        private int getLowestTemperature()
        {
            double min = 1200;
            int index = -1;
            for (int i = 0; i < tempSection.Length; i++)
            {
                if (tempSection[i] <= min)
                {
                    min = tempSection[i];
                    index = i;
                }
            }
            return index;
        }

        private void vacuumPressureCheck()
        {
            if (vacuumPressure > 0.002 && flagPower == true)
            {

                flagPower = false;
                ioC.turnHeatersOff();
                if (pressureErrorEvent != null)
                {
                    pressureErrorEvent(this, null, DateTime.Now.ToString() + " Pressure too high, closing heaters", "Pressure too high, closing heaters. Automatically starting heating once pressure goes below 0.002 Bar");
                }
                liveUploader.addToLog(DateTime.Now.ToString() + " Pressure too high, closing heaters", true);
                pressurePaused = true;
                paused = true;
            }
            if (vacuumPressure < 0.002 && pressurePaused == true)
            {
                flagPower = true;
                ioC.turnHeatersOn();
                resetFullPowerCounter();
                if (heaterRestartEvent != null)
                {
                    heaterRestartEvent(this, null);
                }
                liveUploader.addToLog(DateTime.Now.ToString() + " Pressure lower, starting heaters again", false);
                pressurePaused = false;
                paused = false;
            }
        }

        private void storeMeasurements()
        {
            if (vpList != null && phase != Phase.Finished && phase != Phase.Failed)
            {
                vpList.Add(vacuumPressure);
                npList.Add(normalPressure);
                tempList[0].Add(tempSection[0]);
                tempList[1].Add(tempSection[1]);
                tempList[2].Add(tempSection[2]);
            }
        }

        private void temperatureErrorCheck()
        {
            if (!flagTempError)
            {
                for (int i = 0; i < tempSection.Length; i++)
                {
                    if (tempSection[i] > 1195 || tempSection[i] < 5)
                    {
                        flagTempError = true;
                        string stopString = "";
                        if (!isBrazing())
                        {
                            stopString = "Stopping heating";
                        }
                        if (i == 0)
                        {
                            if (innerHeaterErrorEvent != null)
                            {
                                innerHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with inner section temperature sensor", "Problem with inner section temperature sensor." + stopString);
                            }
                            liveUploader.addToLog(DateTime.Now.ToString() + " Problem with inner section temperature sensor", true);

                        }
                        else if (i == 1)
                        {
                            if (middleHeaterErrorEvent != null)
                            {
                                middleHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with middle section temperature sensor", "Problem with middle section temperature sensor." + stopString);
                            }
                            liveUploader.addToLog(DateTime.Now.ToString() + " Problem with middle section temperature sensor", true);

                        }
                        else
                        {
                            if (outerHeaterErrorEvent != null)
                            {
                                outerHeaterErrorEvent(this, null, DateTime.Now.ToString() + " Problem with outer section temperature sensor", "Problem with outer section temperature sensor." + stopString);
                            }
                            liveUploader.addToLog(DateTime.Now.ToString() + " Problem with outer section temperature sensor", true);

                        }
                        //set the temperature of the trouble section as an average of the other 2 sections
                        tempSection[i] = (tempSection[0] + tempSection[1] + tempSection[2] - tempSection[i]) / 2;
                        stoppedHeating = true;


                    }
                }
            }
        }

        private void getTemperatures()
        {
            double[] value;
            double total;
            for (int i = 0; i < tempSection.Length; i++)
            {
                value = ioC.getTempSamples(i); //get temperature of heater section i  1000 samples
                total = 0;
                for (int j = 0; j < value.Length; j++)
                {
                    total = total + value[j];
                }


                if (value.Length > 0)
                {
                    total = total / value.Length;
                }
                if (signalW != null)
                {
                    signalW.setSignalTemp(i, total);
                }
                //convert to temperature
                tempSection[i] = (float)((total / 10) * 1200);
                liveUploader.temps[i] = (int)tempSection[i];

            }

            if (temperatureUpdateEvent != null)
            {
                temperatureUpdateEvent(this, null, tempSection);
            }

        }

        private void getNormalPressure()
        {
            double[] value;
            double total;

            value = ioC.getNormalPressureSamples();  // get normal pressure
            total = 0;
            for (int i = 0; i < value.Length; i = i + 1)
            {
                total = total + value[i];
            }
            if (value.Length > 0)
            {
                total = total / value.Length;
            }
            if (signalW != null)
            {
                signalW.setSignalNP(total);
            }
            normalPressure = (float)((total - 0.995) / (4.64 - 0.995) * 6);
            if (normalPressure < 0.1)
                normalPressure = 0.0F;
            if (pressureNUpdateEvent != null)
            {
                pressureNUpdateEvent(this, null, normalPressure);
            }
            liveUploader.nPressure = normalPressure;
        }

        private void getVacuumPressure()
        {
            double[] value;
            double total;
            value = ioC.getVacuumPressureSamples(); // get vacuum pressure 1000 samples
            total = 0;
            for (int i = 0; i < value.Length; i = i + 1)
            {
                total = total + value[i];
            }
            if (value.Length > 0)
            { // calculate average of samples
                total = total / value.Length;
            }
            if (signalW != null)
            {
                signalW.setSignalVP(total);
            }

            if (total >= 8.5)
                vacuumPressure = (float)(Math.Pow(10, (8.5 - 5.5)) * 0.001);
            else if (total < 2.5)
            {
                if (phase == Phase.Started || phase == Phase.Heating || phase == Phase.Brazing || phase == Phase.Cooling)
                {
                    if (pressureErrorEvent != null)
                    {
                        pressureErrorEvent(this, null, DateTime.Now.ToString() + " Pressure meter error ", "Pressure meter error. Stopping process");
                    }

                    stoppedCooling = true;
                    stoppedHeating = true;
                }
            }
            else
                vacuumPressure = (float)(Math.Pow(10, (total - 5.5)) * 0.001);
            if (pressureVUpdateEvent != null)
            {
                pressureVUpdateEvent(this, null, vacuumPressure);
            }
            liveUploader.vPressure = vacuumPressure;

        }

        public bool reOpenLargeN2Valve()
        {
            if (phase == Phase.Cooling)
            {
                allowLargeN2valve = true;
                if (!ioC.isLargeN2ValveOpen())
                {
                    largeN2OpenTime = npList.Count - 1;
                    ioC.openLargeN2Valve();
                    return true;
                }
            }
            return false;
        }

        public void stopCoolingFan()
        {
            ioC.startCoolingFan();
        }

        public void startCoolingFan()
        {
            if (phase == Phase.Idle || phase == Phase.Finished || phase == Phase.Failed)
            {
                if (normalPressure > 0.9)
                {
                    ioC.startCoolingFan();
                }
            }
        }

        private void normalPressureCheck()
        {
            if (phase == Phase.Started || phase == Phase.Heating || phase == Phase.Brazing || phase == Phase.Cooling)
            {
                if (largeN2OpenTime > -1 && npList.Count > largeN2OpenTime + 80)
                {
                    if (npList[npList.Count - 1] < 0.1)
                    {
                        //large N2 valve has been opened for 80s but normal pressure has almost not increased at all
                        largeN2OpenTime = -1;
                        ioC.closeLargeN2Valve();
                        allowLargeN2valve = false;
                        if (pressureErrorEvent != null)
                        {
                            pressureErrorEvent(this, null, DateTime.Now.ToString() + " Problem with the pressure not increasing when the large N2 valve is opened. Closing the large N2 valve", "Problem with the pressure not increasing when the large N2 valve is opened. Closing the large N2 valve");
                        }
                        liveUploader.addToLog(DateTime.Now.ToString() + " Problem with the pressure not increasing when the large N2 valve is opened. Closing the large N2 valve", true);
                    }

                }
            }
        }



        private bool isBrazing()
        {
            for (int i = 0; i < tempSection.Length; i++)
            {
                if (tempSection[i] < 1195 && tempSection[i] >= brazeTemp)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkPressureError()
        {
            if (normalPressure < 1.1 && normalPressure > 0.9 && vacuumPressure < 1.1 && vacuumPressure > 0.9)
                return true;
            else
            {

                return false;

            }
        }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BrazeSettings brazeSettings = (BrazeSettings)e.Argument;
            if (loweringPressure())
            {
                if (heatingUp(brazeSettings))
                {
                    braze(brazeSettings);
                }
                cooling(brazeSettings);
            }
            finish();
        }


        private bool loweringPressure()
        {
            //open vacuum valve
            processWorker.ReportProgress((int)ReportType.VacuumValveSignal, "on");
            //open vacuum pump
            processWorker.ReportProgress((int)ReportType.VacuumPumpSignal, "on");

            processWorker.ReportProgress((int)ReportType.SetStatus, "Lowering pressure.");

            //vacuum pump needs to run for at least 5 minutes
            int i = 0;
            while (paused || stoppedHeating || i < 5 * 60)
            {
                if (stoppedHeating)
                {
                    return false;
                }
                Thread.Sleep(1000);
                i++;

            }

            //open roots pump when vacuumPressure <=0.03Bar
            while (vacuumPressure > 0.03 || paused || stoppedHeating)
            {
                if (stoppedHeating)
                { return false; }
                Thread.Sleep(100);
            }

            processWorker.ReportProgress((int)ReportType.BoosterPumpSignal, "on");


            //open three heaters when vacuumPressure <=0.004Bar
            while (vacuumPressure > 0.002 || paused || stoppedHeating)
            {
                if (stoppedHeating)
                { return false; }
                Thread.Sleep(100);
            }
            return true;
        }
        private bool heatingUp(BrazeSettings brazeSettings)
        {
            processWorker.ReportProgress((int)ReportType.SetPhase, Phase.Heating);
            bool smallopened = false;

            processWorker.ReportProgress((int)ReportType.SetStatus, "Heating up to " + brazeSettings.brazeTemp + " °C.");
            //start  heater for section1,section2 and section3 
            processWorker.ReportProgress((int)ReportType.HeaterPowerSignal, "on");

            // syncronize the temperatures             

            double gTemp = 0;

            if (brazeSettings.speedStartL1 == -1)
            {
                gTemp = brazeSettings.syncL1;
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.syncL1);

            }
            else
            {

                gTemp = (tempSection[0] + tempSection[1] + tempSection[2]) / 3;
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, gTemp);
            }


            while (paused || stoppedHeating || !(reachedLevel(brazeSettings.syncL1) && targetTemp >= brazeSettings.syncL1))
            {
                if (gTemp < brazeSettings.syncL1 && brazeSettings.speedStartL1 != -1)
                {
                    gTemp = gTemp + (brazeSettings.speedStartL1 / 60);
                    processWorker.ReportProgress((int)ReportType.SetGoalTemp, gTemp);
                }
                if (tempSection[1] > 800 && !smallopened)
                {
                    //open smallNitrogenValve                     
                    processWorker.ReportProgress((int)ReportType.SmallN2ValveSignal, "on");
                    smallopened = true;
                }
                if (stoppedHeating)
                { return false; }
                Thread.Sleep(1000);
            }
            if (brazeSettings.speedL1L2 == -1)
            {
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.syncL2);
            }
            else
            {
                gTemp = brazeSettings.syncL1;
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.syncL1);
            }

            while (paused || stoppedHeating || !(reachedLevel(brazeSettings.syncL2) && targetTemp >= brazeSettings.syncL2))
            {
                if (gTemp < brazeSettings.syncL2 && brazeSettings.speedL1L2 != -1)
                {
                    gTemp = gTemp + (brazeSettings.speedL1L2 / 60);
                    processWorker.ReportProgress((int)ReportType.SetGoalTemp, gTemp);
                }
                if (tempSection[1] > 800 && !smallopened)
                {
                    //open smallNitrogenValve                     
                    processWorker.ReportProgress((int)ReportType.SmallN2ValveSignal, "on");
                    smallopened = true;
                }
                if (stoppedHeating)
                { return false; }
                Thread.Sleep(1000);
            }
            if (brazeSettings.speedL2Braze == -1)
            {
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.brazeTemp);
            }
            else
            {
                gTemp = brazeSettings.syncL2;
                processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.syncL2);
            }


            while (paused || stoppedHeating || !(reachedLevel(brazeSettings.brazeTemp) && targetTemp >= brazeSettings.brazeTemp))
            {
                if (gTemp < brazeSettings.brazeTemp && brazeSettings.speedL2Braze != -1)
                {
                    gTemp = gTemp + (brazeSettings.speedL2Braze / 60);
                    processWorker.ReportProgress((int)ReportType.SetGoalTemp, gTemp);
                }
                if (tempSection[1] > 800 && !smallopened)
                {
                    //open smallNitrogenValve                     
                    processWorker.ReportProgress((int)ReportType.SmallN2ValveSignal, "on");
                    smallopened = true;
                }
                if (stoppedHeating)
                { return false; }
                Thread.Sleep(1000);
            }

            gTemp = brazeSettings.brazeTemp;
            processWorker.ReportProgress((int)ReportType.SetGoalTemp, brazeSettings.brazeTemp);

            return true;
        }

        private bool reachedLevel(double tempLevel)
        {
            if (tempSection[0] < tempLevel - 5 || tempSection[1] < tempLevel - 5 || tempSection[2] < tempLevel - 5)
            {
                return false;
            }
            return true;
        }
        private void braze(BrazeSettings brazeSettings)
        {


            processWorker.ReportProgress((int)ReportType.SetBrazeStart);
            //Hold brazing temp for x minute
            processWorker.ReportProgress((int)ReportType.SetStatus, "Holding brazing temperature for " + brazeSettings.brazeTime + " minutes");

            for (int j = 0; j < brazeSettings.brazeTime * 60 || paused || stoppedHeating; j++)
            {
                if (stoppedHeating)
                { return; }

                Thread.Sleep(1000);


            }
        }
        private bool cooling(BrazeSettings brazeSettings)
        {
            processWorker.ReportProgress((int)ReportType.SetPhase, Phase.Cooling);
            //close heater for section1,section2 and section3
            processWorker.ReportProgress((int)ReportType.SetStatus, "Brazing finished, self cooling.");
            processWorker.ReportProgress((int)ReportType.HeaterPowerSignal, "off");

            while ((tempSection[0] > 1020 || tempSection[1] > 1020 || tempSection[2] > 1020) || paused || stoppedCooling)
            {
                if (stoppedCooling)
                {
                    return false;
                }
                Thread.Sleep(1000);
            }


            processWorker.ReportProgress((int)ReportType.SetStatus, "Active cooling.");
            //Open large nitrogen valve
            processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "on");

            processWorker.ReportProgress((int)ReportType.SmallN2ValveSignal, "off");
            //Close vacuum valve
            processWorker.ReportProgress((int)ReportType.VacuumValveSignal, "off");

            while (normalPressure <= 0.5)//Wait until Pn  > 0.5 bar
            {

                Thread.Sleep(100);
            }

            //Close roots pump
            processWorker.ReportProgress((int)ReportType.BoosterPumpSignal, "off");

            while (normalPressure <= 1 || paused || stoppedCooling)//Wait until Pn >1 bar
            {
                if (stoppedCooling)
                {
                    return false;
                }
                Thread.Sleep(100);
            }
            // Start cooling fans 
            processWorker.ReportProgress((int)ReportType.CoolingFanSignal, "on");
            //Close vacuum pump 
            processWorker.ReportProgress((int)ReportType.VacuumPumpSignal, "off");

            while (normalPressure <= 1.1 || paused || stoppedCooling)//Wait until Pn>  1.1bar
            {
                if (stoppedCooling)
                {
                    return false;
                }
                Thread.Sleep(100);
            }
            //Close large nitrogen valve
            processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "off");

            while ((tempSection[0] > 930 && tempSection[1] > 930 && tempSection[2] > 930) || paused || stoppedCooling)   //Wait until temps < 930C
            {
                if (stoppedCooling)
                {
                    return false;
                }
                if (normalPressure > brazeSettings.pressure1)
                {
                    //Close large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "off");
                }
                else
                {
                    //open large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "on");
                }
                Thread.Sleep(100);
            }

            while ((tempSection[0] > 800 && tempSection[1] > 800 && tempSection[2] > 800) || paused || stoppedCooling)   //Wait until temps < 800C
            {
                if (stoppedCooling)
                {
                    return false;
                }
                if (normalPressure > brazeSettings.pressure2)
                {
                    //Close large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "off");
                }
                else
                {
                    //Open large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "on");
                }
                Thread.Sleep(100);
            }


            while (tempSection[0] > 150 || tempSection[1] > 150 || tempSection[2] > 150 || paused || stoppedCooling)   //Wait until temps < 150C
            {
                if (stoppedCooling)
                {
                    return false;
                }
                if (normalPressure > brazeSettings.pressure3)
                {
                    //Open large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "off");
                }
                else
                {
                    //Close large nitrogen valve
                    processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "on");
                }
                Thread.Sleep(1000);
            }
            return true;
        }
        private void finish()
        {
            //Close everything
            processWorker.ReportProgress((int)ReportType.LargeN2ValveSignal, "off");
            processWorker.ReportProgress((int)ReportType.SmallN2ValveSignal, "off");
            processWorker.ReportProgress((int)ReportType.BoosterPumpSignal, "off");
            processWorker.ReportProgress((int)ReportType.HeaterPowerSignal, "off");
            processWorker.ReportProgress((int)ReportType.VacuumPumpSignal, "off");
            processWorker.ReportProgress((int)ReportType.VacuumValveSignal, "off");


            while (tempSection[0] > 80 || tempSection[1] > 80 || tempSection[2] > 80 || paused || stoppedCooling)   //Wait until temps < 150C
            {
                if (stoppedCooling)
                {
                    //Open ventilation valve
                    processWorker.ReportProgress((int)ReportType.VentValveSignal, "on");
                    return;
                }
                Thread.Sleep(100);
            }
            //Open ventilation valve
            processWorker.ReportProgress((int)ReportType.VentValveSignal, "on");
        }

        private void processWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


            if (stoppedCooling || stoppedHeating)
            {

                if (completeEvent != null)
                {
                    completeEvent(this, null, "Status: Failed", false);
                }
                liveUploader.status = "Failed";
                phase = Phase.Failed;
            }
            else
            {
                phase = Phase.Finished;
                if (completeEvent != null)
                {
                    completeEvent(this, null, "Status: Finished", true);
                }
                liveUploader.status = "Finished";

            }
            liveUploader.uploadHistoryToWebServer();
        }

        public bool pauseProcess()
        {
            if (paused)
            {
                if (phase == Phase.Heating || phase == Phase.Brazing)
                {
                    flagPower = true;
                    resetFullPowerCounter();
                }

                allowLargeN2valve = true;
                paused = false;
            }
            else
            {
                flagPower = false;
                ioC.closeLargeN2Valve();
                allowLargeN2valve = false;
                paused = true;
            }
            return paused;
        }

        public void signalWrite(int type, bool val)
        {
            ioC.signalWrite(type, val);
        }

        public void shutdownProcess()
        {
            flagPower = false;
            ioC.shutDown();
            allowLargeN2valve = false;
            paused = true;
        }

        private void processWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case (int)ReportType.VacuumPumpSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.startVacuumPump();
                        addLogInfo("The vacuum pump is on.");

                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.stopVacuumPump();

                        addLogInfo("The vacuum pump is off.");
                    }
                    break;
                case (int)ReportType.BoosterPumpSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.startBoosterPump();
                        addLogInfo("The booster pump is on.");
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.stopBoosterPump();
                        addLogInfo("The booster pump is off.");
                    }
                    break;
                case (int)ReportType.SmallN2ValveSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.openSmallN2Valve();
                        addLogInfo("The small Nitrogen valve is opened.");
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.closeSmallN2Valve();
                        addLogInfo("The small Nitrogen valve is closed.");
                    }
                    break;
                case (int)ReportType.LargeN2ValveSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        if (allowLargeN2valve && !ioC.isLargeN2ValveOpen())
                        {
                            largeN2OpenTime = npList.Count - 1;
                            ioC.openLargeN2Valve();
                            addLogInfo("The large Nitrogen valve is opened.");
                        }
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        if (ioC.isLargeN2ValveOpen())
                        {
                            largeN2OpenTime = -1;
                            ioC.closeLargeN2Valve();
                            addLogInfo("The large Nitrogen valve is closed.");
                        }
                    }
                    break;
                case (int)ReportType.VacuumValveSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.openVacuumValve();
                        addLogInfo("The vacuum valve is opened.");
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.closeVacuumValve();
                        addLogInfo("The vacuum valve is closed.");
                    }
                    break;
                case (int)ReportType.VentValveSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.openVentilationValve();
                        addLogInfo("The ventilation valve is opened.");
                        setCurrentStep("Ventilation valve opened, cooling complete");
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.closeVentilationValve();
                        addLogInfo("The ventilation valve is closed.");
                    }
                    break;
                case (int)ReportType.CoolingFanSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        ioC.startCoolingFan();
                        addLogInfo("The cooling fan is on.");
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.stopCoolingFan();
                        addLogInfo("The cooling fan is off.");
                    }
                    break;
                case (int)ReportType.SetBrazeStart:
                    beginBrazeTime = DateTime.Now;
                    phase = Phase.Brazing;
                    break;
                case (int)ReportType.HeaterPowerSignal:
                    if (e.UserState.ToString() == "on")
                    {
                        if (!paused)
                        {
                            ioC.turnHeatersOn();
                            flagPower = true;
                            resetFullPowerCounter();
                            addLogInfo("Power to heaters turned on.");
                        }
                    }
                    else if (e.UserState.ToString() == "off")
                    {
                        ioC.turnHeatersOff();
                        flagPower = false;
                        addLogInfo("Power to heaters turned off.");
                    }
                    break;
                case (int)ReportType.SetStatus:
                    currentStepString = e.UserState.ToString();
                    setCurrentStep(currentStepString);
                    break;
                case (int)ReportType.SetGoalTemp:
                    targetTemp = (double)e.UserState;
                    setCurrentStep(currentStepString + "(Temperature goal: " + (int)targetTemp + "°C)");
                    break;
                case (int)ReportType.SetPhase:
                    phase = (Phase)e.UserState;
                    break;

            }
        }

        private void resetFullPowerCounter()
        {
            fullPowerCounter[0] = 0;
            fullPowerCounter[1] = 0;
            fullPowerCounter[2] = 0;
        }

        private void setCurrentStep(string currStep)
        {
            if (stageEvent != null)
            {
                stageEvent(this, null, currStep);
            }
            liveUploader.latestStep = currStep;
        }

        private void addLogInfo(string logText)
        {
            if (logEvent != null)
            {
                logEvent(this, null, DateTime.Now.ToString() + " " + logText);
            }
            liveUploader.addToLog(DateTime.Now.ToString() + " " + logText, false);
        }

        private void updateTime()
        {
            if (phase == Phase.Started || phase == Phase.Heating || phase == Phase.Brazing || phase == Phase.Cooling)
            {
                TimeSpan elapTime = DateTime.Now - beginTime;
                string elapsedtime = string.Format("{0:00}:{1:00}:{2:00}", (int)elapTime.TotalHours, elapTime.Minutes, elapTime.Seconds);
                liveUploader.elapsedTime = elapsedtime;
                int progress = 0;
                string brazetimeString = "";
                if (phase == Phase.Brazing)
                {
                    //calculate the brazing time from the beginning of brazing until now
                    DateTime endTime1 = DateTime.Now;
                    TimeSpan midTime = endTime1 - beginBrazeTime;
                    brazetimeString = string.Format("{0:00}:{1:00}:{2:00}", (int)midTime.TotalHours, midTime.Minutes, midTime.Seconds);
                    liveUploader.brazeTime = brazetimeString;
                    progress = (int)(midTime.TotalSeconds * 100 / (brazeTime * 60));
                   
                }
                if (updateTimeEvent != null)
                {
                    updateTimeEvent(this, null, elapsedtime, brazetimeString, progress);
                }
            }
        }

        private void temperatureControl()
        {
            if (flagPower)
            {
                for (int i = 0; i < tempSection.Length; i++)
                {
                    if (tempList[i].Count > 1)
                    {
                        double cOutput = tempC.calculateOutput(i, targetTemp, tempList[i][tempList[i].Count - 1], tempList[i][tempList[i].Count - 2]);
                        ioC.setHeaterPower(i, cOutput);
                        if (signalW != null)
                        {
                            signalW.setSignalT(i, cOutput);
                        }
                        power[i] = (float)(cOutput * 100);
                    }
                }
            }
            else
            {

                for (int i = 0; i < tempSection.Length; i++)
                {
                    ioC.setHeaterPower(i, 0);
                    power[i] = 0;

                }

            }

            if (powerUpdateEvent != null)
            {
                powerUpdateEvent(this, null, new string[] { ((int)power[0]).ToString(), ((int)power[1]).ToString(), ((int)power[0]).ToString() });

            }

            //check if power is at max. If so add to counter, else reset counter.
            for (int i = 0; i < power.Length; i++)
            {
                if (power[i] >= 100)
                {
                    fullPowerCounter[i]++;
                }
                else
                {
                    fullPowerCounter[i] = 0;
                }
            }
        }

        internal void stop()
        {
            if (phase == Phase.Heating || phase == Phase.Started || phase == Phase.Brazing)
            {
                stoppedHeating = true;
            }
            else if (phase == Phase.Cooling)
            {
                ioC.closeLargeN2Valve();
                allowLargeN2valve = false;
                stoppedCooling = true;
            }
            else
            {
                ioC.stopAll();

            }
        }

        internal void startProcess()
        {
            for (int i = 0; i < tempSection.Length; i++)
            {
                tempSection[i] = 0;
                tempList[i] = new List<double>();
                fullPowerCounter[i] = 0;
            }

            vpList = new List<double>();
            npList = new List<double>();
        }


        internal void setStartValues(string brazeText)
        {
            phase = FurnaceController.ProcessController.Phase.Started;
            ioC.stopAll();
            liveUploader.brazeName = brazeText;
        }

        BrazeSettings csettings;
        internal void setProcessSettings(BrazeSettings csettings)
        {
            if (!processWorker.IsBusy)
            {
                beginTime = DateTime.Now;
                tempC = new TemperatureController();
                processWorker.RunWorkerAsync(csettings);
                brazeTemp = csettings.brazeTemp;
                brazeTime = csettings.brazeTime;
                liveUploader.startTime = DateTime.Now.ToString("HH:mm:ss");
                liveUploader.mode = csettings.modeName;
                liveUploader.status = "Running";
            }

        }

        public string writeXml(List<string> logList, List<bool> errorTypeList)
        {
            TimeSpan tdiff = DateTime.Now - beginTime;
            string duration = string.Format("{0:00}:{1:00}:{2:00}", (int)tdiff.TotalHours, tdiff.Minutes, tdiff.Seconds);
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("stats");

                writer.WriteStartElement("BrazeNumber");
                writer.WriteStartAttribute("Value");
                writer.WriteString((Properties.Settings.Default.brazeNbr - 1).ToString());
                writer.WriteEndAttribute();

                writer.WriteStartElement("Date");
                writer.WriteStartAttribute("Value");
                writer.WriteString(DateTime.Now.ToString("yyyy-MM-dd"));
                writer.WriteEndAttribute();

                writer.WriteStartElement("Duration");
                writer.WriteStartAttribute("Value");
                writer.WriteString(duration);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Status");
                writer.WriteStartAttribute("Value");
                if (phase == FurnaceController.ProcessController.Phase.Finished)
                    writer.WriteString("Finished");
                else
                    writer.WriteString("Failed");
                writer.WriteEndAttribute();



                writer.WriteStartElement("Process");
                for (int i = 0; i < logList.Count; i++)
                {
                    writer.WriteStartElement("Log");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(logList[i]);
                    writer.WriteEndAttribute();
                    if (errorTypeList[i])
                    {
                        writer.WriteStartAttribute("Error");
                        writer.WriteString("True");
                        writer.WriteEndAttribute();
                    }
                    else
                    {
                        writer.WriteStartAttribute("Error");
                        writer.WriteString("False");
                        writer.WriteEndAttribute();
                    }
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();

                writer.WriteStartElement("VacuumPressure");
                for (int i = 0; i < vpList.Count; i++)
                {
                    writer.WriteStartElement("Measurement");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(vpList[i].ToString());
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();

                writer.WriteStartElement("NormalPressure");
                for (int i = 0; i < npList.Count; i++)
                {
                    writer.WriteStartElement("Measurement");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(npList[i].ToString());
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Temp1");
                for (int i = 0; i < tempList[0].Count; i++)
                {
                    writer.WriteStartElement("Measurement");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(tempList[0][i].ToString());
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Temp2");
                for (int i = 0; i < tempList[1].Count; i++)
                {
                    writer.WriteStartElement("Measurement");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(tempList[1][i].ToString());
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Temp3");
                for (int i = 0; i < tempList[2].Count; i++)
                {
                    writer.WriteStartElement("Measurement");
                    writer.WriteStartAttribute("Value");
                    writer.WriteString(tempList[2][i].ToString());
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                return builder.ToString();
            }
        }
    }
}
