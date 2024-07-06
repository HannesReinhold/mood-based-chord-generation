using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondOrderAllpass
{
    private float z0, z1;
    private float a0, a1;

    public void CalcCoeffs(float fc, float q, float sampleRate)
    {
        a0 = (Mathf.Tan(Mathf.PI*q/sampleRate)-1) / (Mathf.Tan(Mathf.PI * q / sampleRate) + 1);
        a1 = -Mathf.Cos(2*Mathf.PI*fc/sampleRate);
    }

    public float Process(float input)
    {
        float x = input - a1 * (1 - a0) * z0 + a0 * z1;
        float y = -a0 * x + a1 * (1 - a0) * z0 + z1;
        z1 = z0;
        z0 = x;
        return y;
    }
}
