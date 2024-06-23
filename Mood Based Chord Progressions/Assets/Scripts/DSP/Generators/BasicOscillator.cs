using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicOscillator
{

    public float gain;
    public float frequency;
    public float startPhase;


    private float increment;
    private float phase;

    private float sampleRate;

    public BasicOscillator(float sampleR)
    {
        gain = 1.0f;
        frequency = 440f;
        startPhase = 0.0f;

        increment = 0.0f;
        phase = 0.0f;

        sampleRate = sampleR;
    }

    public void Reset()
    {
        phase = startPhase;
    }

    public void SetFrequency(float f)
    {
        frequency = f;
        increment = Mathf.PI * 2 * frequency / sampleRate;
    }



    public float RenderSample()
    {
        float output = Mathf.Sin(phase) * gain;
        phase += increment;
        //if (phase > Mathf.PI * 2) phase = 0;
        return output;
    }

}
