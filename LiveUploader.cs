using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace FurnaceController
{
    class LiveUploader
    {
        public int[] temps = new int[3];
        public int[] power = new int[3];
        public double nPressure = 0;
        public double vPressure = 0;
        public string startTime = "";
        public string elapsedTime = "";
        public string status = "";
        public string brazeTime = "";
        public string brazeName = "";
        public string latestStep = "";
        public string mode = "";
        string logfile = "";
        BackgroundWorker bw = new BackgroundWorker();
        BackgroundWorker bwh = new BackgroundWorker();

        public LiveUploader()
        {
            for (int i = 0; i < temps.Length; i++)
            {
                temps[i] = 0;
            }
            for (int i = 0; i < power.Length; i++)
            {
                power[i] = 0;
            }
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bwh.DoWork += new DoWorkEventHandler(bwh_DoWork);
        }

        void bwh_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Collections.Specialized.NameValueCollection reqparm = (System.Collections.Specialized.NameValueCollection)e.Argument;
                using (WebClient client = new WebClient())
                {
                    byte[] responsebytes = client.UploadValues("http://chrispersson.com/liveupload.ashx", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                }
            }
            catch (Exception ex)
            {
            }
        }


        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Collections.Specialized.NameValueCollection reqparm = (System.Collections.Specialized.NameValueCollection)e.Argument;
                using (WebClient client = new WebClient())
                {
                    byte[] responsebytes = client.UploadValues("http://chrispersson.com/historyupload.ashx", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void uploadToWebServer()
        {
            System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("temp1", temps[0].ToString());
            reqparm.Add("temp2", temps[1].ToString());
            reqparm.Add("temp3", temps[2].ToString());
            reqparm.Add("power1", power[0].ToString());
            reqparm.Add("power2", power[1].ToString());
            reqparm.Add("power3", power[2].ToString());
            reqparm.Add("startTime", startTime);
            reqparm.Add("elapsedTime", elapsedTime);
            reqparm.Add("status", status);
            reqparm.Add("brazeTime", brazeTime);
            reqparm.Add("brazeName", brazeName);
            reqparm.Add("latestStep", latestStep);
            reqparm.Add("mode", mode);
            reqparm.Add("logfile", mode);
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync(reqparm);
            }
        }

        public void uploadHistoryToWebServer()
        {
            System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("startTime", startTime);
            reqparm.Add("elapsedTime", elapsedTime);
            reqparm.Add("status", status);
            reqparm.Add("brazeTime", brazeTime);
            reqparm.Add("brazeName", brazeName);
            reqparm.Add("latestStep", latestStep);
            reqparm.Add("mode", mode);
            reqparm.Add("logfile", mode);
            if (!bwh.IsBusy)
            {
                bwh.RunWorkerAsync(reqparm);
            }
        }

        public void addToLog(string text, bool error)
        {
            logfile += text;
            if (error)
            {
                logfile += " [Error]\n";
            }
            else
            {
                logfile += "\n";
            }
        }
    }
}
