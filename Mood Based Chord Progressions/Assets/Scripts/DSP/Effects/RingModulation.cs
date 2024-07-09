using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingModulation
{

    public float frequency = 440;
    public float strength = 1;

    private float sampleRate;
    private float increment;
    private float phase;

    public RingModulation(float sampleRate)
    {
        this.sampleRate = sampleRate;
        phase = 0;
    }


    public void SetFrequency(float f)
    {
        frequency = f;
        increment = frequency / sampleRate * Mathf.PI * 2;
    }


    public float Process(float input)
    {

        input *= Mathf.Sin(phase);
        phase += increment;

        return input;
    }

    public void ProcessBlock(float[] data, int numChannels)
    {
        for(int i=0; i<data.Length; i += 2)
        {
            float mod = Mathf.Sin(phase);
            data[i] *= mod;
            data[i + 1] *= mod;

            phase += increment;
            if (phase >= Mathf.PI * 2) phase -= Mathf.PI * 2;
        }
    }
}
