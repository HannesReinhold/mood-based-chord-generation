using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biquad
{
    float a0, a1, a2, b1, b2;
    float z1, z2;

    private float lastF = 0;

    private float sampleRate;

    public BiquadCalculator.BiquadType type;

    public void SetCoeffs(float Fc, float Q, float peakGain, BiquadCalculator.BiquadType type)
    {
        if(Mathf.Abs(Fc-lastF) < 3f)
        {
            return;
        }

        lastF = Fc;

        float[] coeffs = BiquadCalculator.CalcCoeffs(Fc, Q, peakGain, type, sampleRate);
        a0 = coeffs[0];
        a1 = coeffs[1];
        a2 = coeffs[2];
        b1 = coeffs[3];
        b2 = coeffs[4];
    }

    public void SetCoeffs(float Fc, float Q, float peakGain)
    {
        SetCoeffs(Fc, Q, peakGain, type);
    }

    public float Process(float input)
    {
        float output = input * a0 + z1;
        z1 = input * a1 + z2 - b1 * output;
        z2 = input * a2 - b2 * output;
        return output;
    }
}
