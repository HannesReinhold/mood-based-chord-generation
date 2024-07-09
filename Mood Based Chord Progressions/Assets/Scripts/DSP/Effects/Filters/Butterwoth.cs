using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterwoth
{
    float x0, x1, y0, y1;

    float a1, a2, b0, b1, b2;

    public bool lowpass = true;

    public float freq;


    public void CalcCoeffs(float fc, float fs, bool lowPass)
    {
        freq = fc;
        this.lowpass = lowPass;
         float k = 1/Mathf.Tan(Mathf.PI * fc / (fs));
         float k2 = k * k;
         float q = Mathf.Sqrt(2);
         float norm = 1 / (1 + k * q + k2);
        if (lowPass)
        {
            b0 = norm;
            b1 = 2 * b0;
            b2 = b0;
        }
        else
        {
            b0 = norm;
            b1 = 2 * b0;
            b2 = b0;
        }
        a1 = 2 * (k2 - 1) * norm;
        a2 = -(1 - k * q + k2) * norm;
    }

    public float Process(float input)
    {

        /*
        float x = input + a1 * x0 + a2 * x1;
        float y = b0 * x + b1 * y0 + b2 * y1;
        x1 = x0;
        x0 = input;
        y1 = y0;
        y0 = y;
        */
        float x = input + a1 * x0 + a2 * x1;
        float y = b0 * x + b1 *x0 + b2 * x1;
        x1 = x0;
        x0 = x;

        return lowpass ? y : input - y;
    }
}
