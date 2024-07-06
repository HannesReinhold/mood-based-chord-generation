using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{

    public static float Sqrt2 = 1.41421356237f;
    public static float NoteToFreq(float m)
    {
        return Mathf.Pow(2f, (m-69) / 12f) * 440f;
    }

    public static float FreqToNote(float f)
    {
        return Mathf.Log(f / 440, 2) * 12f - 69;
    }

    public static float CentToFreq(float m)
    {
        return Mathf.Pow(2f, (m-69) / 1200f) * 440f;
    }

    public static float FreqToCent(float f)
    {
        return Mathf.Log(f / 440, 2) * 1200f - 69;
    }

    public static float LinToDb(float x)
    {
        return 10 * Mathf.Log10(Mathf.Max(x,0.0000001f));
    }

    public static float DbToLin(float x)
    {
        return Mathf.Pow(10, x / 10f);
    }


}
