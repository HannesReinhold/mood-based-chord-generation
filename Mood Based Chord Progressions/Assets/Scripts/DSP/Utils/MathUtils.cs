using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float NoteToFreq(float m)
    {
        return Mathf.Pow(2f, m / 12f) * 440f;
    }

    public static float FreqToNote(float f)
    {
        return Mathf.Log(f / 440, 2) * 12f;
    }

    public static float CentToFreq(float m)
    {
        return Mathf.Pow(2f, m / 12f) * 440f;
    }

    public static float FreqToCent(float f)
    {
        return Mathf.Log(f / 440, 2) * 12f;
    }


}
