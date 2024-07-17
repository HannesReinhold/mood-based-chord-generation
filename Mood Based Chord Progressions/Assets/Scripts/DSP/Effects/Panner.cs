using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panner
{

    public float pan = 0.5f;

    public void ProcessBlock(float[] data, int numChannels)
    {
        for (int i = 0; i < data.Length; i+=numChannels)
        {
            data[i+1] *= Mathf.Sin(pan*Mathf.PI*0.5f);
            data[i] *= Mathf.Cos(pan * Mathf.PI * 0.5f);
        }
    }
}
