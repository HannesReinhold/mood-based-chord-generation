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
        float sum = input + z0;
        for(int i=0; i<numStages; i++)
        {
            sum = allpasses[i].ProcessSample(sum, 1-fc);
        }
        z0 = (1 - feedback) * z0 + (positiveFeedback ? sum : -sum) * (feedback);
        return input + sum;
    }
}
