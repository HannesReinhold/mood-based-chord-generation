using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderLowpass
{

    private float z0;
    public float ProcessSample(float input, float fc)
    {
        z0 = z0 + fc * (input - z0);
        return z0;
    }

}
