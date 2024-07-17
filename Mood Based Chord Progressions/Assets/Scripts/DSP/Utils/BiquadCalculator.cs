using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BiquadCalculator
{
    

    static float sqrt2 = System.MathF.Sqrt(2);

    /// <summary>
    /// Biquad calculator from: https://www.earlevel.com/main/2011/01/02/biquad-formulas/
    /// </summary>
    /// <param name="f">Cutoff frequency</param>
    /// <param name="q">Resonance</param>
    /// <param name="pG">Gain</param>
    /// <param name="type">Filter type</param>
    /// <param name="sampleRate">Sample rate of the filter</param>
    /// <returns></returns>
    public static float[] CalcCoeffs(float f, float q, float pG, BiquadType type, float sampleRate)
    {
        f = f / sampleRate;
        float norm;
        float V = System.MathF.Pow(10, System.MathF.Abs(pG) / 20f);
        float K = System.MathF.Tan(System.MathF.PI * f);
        float kk = K*K;
        float kq = K/q;

        float a0 = 0, a1 = 0, a2 = 0, b1 = 0, b2 = 0;

        switch (type)
        {
            case BiquadType.Lowpass:
                norm = 1 / (1 + kq + kk);
                a0 = kk * norm;
                a1 = 2 * a0;
                a2 = a0;
                b1 = 2 * (kk - 1) * norm;
                b2 = (1 - kq + kk) * norm;
                break;

            case BiquadType.Highpass:
                norm = 1 / (1 + kq + kk);
                a0 = 1 * norm;
                a1 = -2 * a0;
                a2 = a0;
                b1 = 2 * (kk - 1) * norm;
                b2 = (1 - kq + kk) * norm;
                break;

            case BiquadType.Bandpass:
                norm = 1 / (1 + kq + kk);
                a0 = kq * norm;
                a1 = 0;
                a2 = -a0;
                b1 = 2 * (kk - 1) * norm;
                b2 = (1 - kq + kk) * norm;
                break;

            case BiquadType.Notch:
                norm = 1 / (1 + kq + kk);
                a0 = (1 + kk) * norm;
                a1 = 2 * (kk - 1) * norm;
                a2 = a0;
                b1 = a1;
                b2 = (1 - kq + kk) * norm;
                break;

            case BiquadType.Peak:
                if (pG >= 0)
                {    // boost
                    float qInv = 1 / q * K;
                    norm = 1 / (1 + qInv + kk);
                    a0 = (1 + V * qInv + kk) * norm;
                    a1 = 2 * (kk - 1) * norm;
                    a2 = (1 - V * qInv + kk) * norm;
                    b1 = a1;
                    b2 = (1 - qInv + kk) * norm;
                }
                else
                {    // cut
                    float qInv = 1 / q * K;
                    norm = 1 / (1 + V * qInv + kk);
                    a0 = (1 + qInv + kk) * norm;
                    a1 = 2 * (kk - 1) * norm;
                    a2 = (1 - qInv + kk) * norm;
                    b1 = a1;
                    b2 = (1 - V * qInv + kk) * norm;
                }
                break;
            case BiquadType.Lowshelf:
                if (pG >= 0)
                {    // boost
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2 * K + kk);
                    a0 = (1 + sqrt2V * K + V * kk) * norm;
                    a1 = 2 * (V * kk - 1) * norm;
                    a2 = (1 - sqrt2V * K + V * kk) * norm;
                    b1 = 2 * (kk - 1) * norm;
                    b2 = (1 - sqrt2 * K + kk) * norm;
                }
                else
                {    // cut
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2V * K + V * kk);
                    a0 = (1 + sqrt2 * K + kk) * norm;
                    a1 = 2 * (kk - 1) * norm;
                    a2 = (1 - sqrt2 * K + kk) * norm;
                    b1 = 2 * (V * kk - 1) * norm;
                    b2 = (1 - sqrt2V * K + V * kk) * norm;
                }
                break;
            case BiquadType.Highshelf:
                if (pG >= 0)
                {    // boost
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2 * K + kk);
                    a0 = (V + sqrt2V * K + kk) * norm;
                    a1 = 2 * (kk - V) * norm;
                    a2 = (V - sqrt2V * K + kk) * norm;
                    b1 = 2 * (kk - 1) * norm;
                    b2 = (1 - sqrt2 * K + kk) * norm;
                }
                else
                {    // cut
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (V + sqrt2V * K + kk);
                    a0 = (1 + sqrt2 * K + kk) * norm;
                    a1 = 2 * (kk - 1) * norm;
                    a2 = (1 - sqrt2 * K + kk) * norm;
                    b1 = 2 * (kk - V) * norm;
                    b2 = (V - sqrt2V * K + kk) * norm;
                }
                break;
            case BiquadType.Allpass:
                norm = 1 / (1 + kq + kk);
                a0 = (1 - kq + kk) * norm;
                a1 = 2 * (kk - 1) * norm;
                a2 = 1;
                b1 = a1;
                b2 = a0;
                break;

        }

        return new float[5] {a0, a1, a2, b1, b2};
    }


    public static float GetFrequencyResponse(float freq, float[] coeffs, float sampleRate)
    {


        float w = freq / sampleRate * Mathf.PI*2;
        float phi = Mathf.Pow(Mathf.Sin(w),2);

        float sinW = Mathf.Sin(w);
        float cosW = Mathf.Cos(w);

        float mag = 0.25f * Mathf.Log((Mathf.Sqrt(square(coeffs[0] * square(cosW) - coeffs[0] * square(sinW) + coeffs[1] * cosW + coeffs[2]) + square(2f * coeffs[0] * cosW * sinW + coeffs[1] * (sinW))) /
                          Mathf.Sqrt(square(square(cosW) - square(sinW) + coeffs[3] * cosW + coeffs[4]) + square(2f * cosW * sinW + coeffs[3] * (sinW)))));

        return MathUtils.DbToLin(Mathf.Max(-1.5f,mag));
    }

    private static float square(float input)
    {
        return input * input;
    }

    
}
public enum BiquadType
{
    Lowpass,
    Highpass,
    Bandpass,
    Notch,
    Peak,
    Lowshelf,
    Highshelf,
    Allpass

}