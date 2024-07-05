using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderAllpass1
{
    public float a;

    private float x;
    private float y;

    public float Process(float input)
    {
        float output = -a * input + x + a * y;
        x = input;
        y = output;

        return output;
    }

    public float Process(float input, float a)
    {
        float output = -a * input + x + a * y;
        x = input;
        y = output;

        return output;
    }
}
