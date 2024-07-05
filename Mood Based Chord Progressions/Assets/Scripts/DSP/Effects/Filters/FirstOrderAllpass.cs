using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderAllpass
{

    private float z0, z1;

    public float ProcessSample(float input, float fc)
    {
        float output = -fc * input + z0 + fc * z1;
        z0 = input;
        z1 = output;
        return output;
    }
}
