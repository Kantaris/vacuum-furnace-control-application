using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurnaceController
{
    class TemperatureController
    {
        double[] iv = new double[3];
        double Kp = 0.2;
        double Ki = 0.01;
        double Kd = 8;

        public TemperatureController()
        {
            for (int i = 0; i < iv.Length; i++)
            {
                iv[i] = 0;
            }
        }

  

        public double calculateOutput(int i, double goalTemp, double currentTemp, double previousTemp)
        {
            double error = goalTemp - currentTemp;
            double errorDiff = previousTemp - currentTemp;
            if (error < 5)
            {
                iv[i] += error;
            }
            else
            {
                iv[i] = 0;
            }
            double dP = Kp * error;
            double dI = Ki * iv[i];
            double dD = Kd * errorDiff;

            double output = dP + dI + dD;
            if (output < 0)
            {
                output = 0;
            }
            if (output > 1)
            {
                output = 1;
            }

            return output;
        }

    }
}
