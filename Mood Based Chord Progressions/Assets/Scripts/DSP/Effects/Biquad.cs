using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biquad
{
    float a0, a1, a2, b1, b2;
    float z1, z2;

    private float lastF = 0;

    public void SetCoeffs(float Fc, float Q, float peakGain)
    {
        if(Mathf.Abs(Fc-lastF) < 3f)
        {
            return;
        }

        lastF = Fc;

        Fc = Fc / 48000f;

        float norm;
        float V = Mathf.Pow(10, Mathf.Abs(peakGain) / 20f);
        float K = Mathf.Tan(Mathf.PI * Fc);

        norm = 1 / (1 + K / Q + K * K);
        a0 = K * K * norm;
        a1 = 2 * a0;
        a2 = a0;
        b1 = 2 * (K * K - 1) * norm;
        b2 = (1 - K / Q + K * K) * norm;
    }

    public float Process(float input)
    {
        float output = input * a0 + z1;
        z1 = input * a1 + z2 - b1 * output;
        z2 = input * a2 - b2 * output;
        return output;
    }
}
