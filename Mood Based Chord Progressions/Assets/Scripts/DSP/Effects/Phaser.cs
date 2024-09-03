using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phaser
{
    public int numStages = 4;
    public int maxStages = 32;

    public float fc;
    public float feedback;
    public bool positiveFeedback = true;

    private float z0;

    private FirstOrderAllpass[] allpasses;

    public float lfoStrength;

    private float realFc = 0;
    private float phase = 0;

    public Phaser()
    {
        allpasses = new FirstOrderAllpass[maxStages];
        for(int i=0; i<maxStages; i++)
        {
            allpasses[i] = new FirstOrderAllpass();
        }
    }

    public float Process(float input)
    {
        realFc = Mathf.Clamp(fc + Mathf.Sin(phase)*lfoStrength,0.0001f,0.9999f);
        phase += 1f / 48000f;

        float sum = input + z0 * feedback;
        for(int i=0; i<numStages; i++)
        {
            sum = allpasses[i].ProcessSample(sum, 1-realFc);
        }
        z0 = (1 - feedback) * z0 + (positiveFeedback ? sum : -sum) * (feedback);
        return input + sum;
    }
}
