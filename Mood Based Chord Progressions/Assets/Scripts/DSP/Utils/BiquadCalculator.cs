using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BiquadCalculator
{
    public enum BiquadType
    {
        LOWPASS,
        HIGHPASS,
        BANDPASS,
        NOTCH,
        PEAK,
        LOWSHELF,
        HIGHSHELF

    }

    static float sqrt2 = System.MathF.Sqrt(2);

    public static float[] CalcCoeffs(float f, float q, float pG, BiquadType type, float sampleRate)
    {
        f = f / sampleRate;
        float norm;
        float V = System.MathF.Pow(10, System.MathF.Abs(pG) / 20f);
        float K = System.MathF.Tan(System.MathF.PI * f);
        float kk = K * K;
        float kq = K / q;

        float a0 = 0, a1 = 0, a2 = 0, b1 = 0, b2 = 0;

        switch (type)
        {
            case BiquadType.LOWPASS:
                norm = 1 / (1 + K / q + K * K);
                a0 = K * K * norm;
                a1 = 2 * a0;
                a2 = a0;
                b1 = 2 * (K * K - 1) * norm;
                b2 = (1 - K / q + K * K) * norm;
                break;

            case BiquadType.HIGHPASS:
                norm = 1 / (1 + K / q + K * K);
                a0 = 1 * norm;
                a1 = -2 * a0;
                a2 = a0;
                b1 = 2 * (K * K - 1) * norm;
                b2 = (1 - K / q + K * K) * norm;
                break;

            case BiquadType.BANDPASS:
                norm = 1 / (1 + K / q + K * K);
                a0 = K / q * norm;
                a1 = 0;
                a2 = -a0;
                b1 = 2 * (K * K - 1) * norm;
                b2 = (1 - K / q + K * K) * norm;
                break;

            case BiquadType.NOTCH:
                norm = 1 / (1 + K / q + K * K);
                a0 = (1 + K * K) * norm;
                a1 = 2 * (K * K - 1) * norm;
                a2 = a0;
                b1 = a1;
                b2 = (1 - K / q + K * K) * norm;
                break;

            case BiquadType.PEAK:
                if (pG >= 0)
                {    // boost
                    norm = 1 / (1 + 1 / q * K + K * K);
                    a0 = (1 + V / q * K + K * K) * norm;
                    a1 = 2 * (K * K - 1) * norm;
                    a2 = (1 - V / q * K + K * K) * norm;
                    b1 = a1;
                    b2 = (1 - 1 / q * K + K * K) * norm;
                }
                else
                {    // cut
                    norm = 1 / (1 + V / q * K + K * K);
                    a0 = (1 + 1 / q * K + K * K) * norm;
                    a1 = 2 * (K * K - 1) * norm;
                    a2 = (1 - 1 / q * K + K * K) * norm;
                    b1 = a1;
                    b2 = (1 - V / q * K + K * K) * norm;
                }
                break;
            case BiquadType.LOWSHELF:
                if (pG >= 0)
                {    // boost
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2 * K + K * K);
                    a0 = (1 + sqrt2V * K + V * K * K) * norm;
                    a1 = 2 * (V * K * K - 1) * norm;
                    a2 = (1 - sqrt2V * K + V * K * K) * norm;
                    b1 = 2 * (K * K - 1) * norm;
                    b2 = (1 - sqrt2 * K + K * K) * norm;
                }
                else
                {    // cut
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2V * K + V * K * K);
                    a0 = (1 + sqrt2 * K + K * K) * norm;
                    a1 = 2 * (K * K - 1) * norm;
                    a2 = (1 - sqrt2 * K + K * K) * norm;
                    b1 = 2 * (V * K * K - 1) * norm;
                    b2 = (1 - sqrt2V * K + V * K * K) * norm;
                }
                break;
            case BiquadType.HIGHSHELF:
                if (pG >= 0)
                {    // boost
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (1 + sqrt2 * K + K * K);
                    a0 = (V + sqrt2V * K + K * K) * norm;
                    a1 = 2 * (K * K - V) * norm;
                    a2 = (V - sqrt2V * K + K * K) * norm;
                    b1 = 2 * (K * K - 1) * norm;
                    b2 = (1 - sqrt2 * K + K * K) * norm;
                }
                else
                {    // cut
                    float sqrt2V = System.MathF.Sqrt(2 * V);
                    norm = 1 / (V + sqrt2V * K + K * K);
                    a0 = (1 + sqrt2 * K + K * K) * norm;
                    a1 = 2 * (K * K - 1) * norm;
                    a2 = (1 - sqrt2 * K + K * K) * norm;
                    b1 = 2 * (K * K - V) * norm;
                    b2 = (V - sqrt2V * K + K * K) * norm;
                }
                break;
        }

        return new float[5] {a0, a1, a2, b1, b2};
    }
}
