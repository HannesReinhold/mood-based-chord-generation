using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderLowpass
{

    private float z0;
    public float Process(float input, float fc)
    {
        z0 = input * fc + z0 * (1-fc);
        return z0;
    }
}
