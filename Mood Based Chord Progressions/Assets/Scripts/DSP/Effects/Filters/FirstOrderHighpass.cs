using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderHighpass
{

    private float z0;
    public float ProcessSample(float input, float fc)
    {
        z0 = (fc - 1) * input + fc * z0;
        return z0;
    }

}
