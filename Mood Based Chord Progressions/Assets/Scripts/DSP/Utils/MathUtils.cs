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

    public static int noteIDFromName(string name)
    {
        bool sharp = name.Substring(1,1).Equals("#");
        string note = name.Substring(0,1) + (sharp ? "#" : "");

        int octave = 0;
        if(sharp) octave =int.Parse(name.Substring(2, 1));
        else octave = int.Parse(name.Substring(1, 1));
        switch (note.ToLower())
        {
            case "a":
                return 12 + octave * 12;
            case "a#":
                return 13 + octave * 12;
            case "b":
                return 14 + octave * 12;
            case "c":
                return 3 + octave * 12;
            case "c#":
                return 4 + octave * 12;
            case "d":
                return 5 + octave * 12;
            case "d#":
                return 6 + octave * 12;
            case "e":
                return 7 + octave * 12;
            case "f":
                return 8 + octave * 12;
            case "f#":
                return 9 + octave * 12;
            case "g":
                return 10 + octave * 12;
            case "g#":
                return 11 + octave * 12;
            default:
                return 0;
        }
    }


}
