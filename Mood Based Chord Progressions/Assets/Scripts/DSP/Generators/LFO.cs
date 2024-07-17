using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LFO
{
    
    public LFOShape shape;
    public float frequency;

    private float sampleRate;
    private float increment;
    private float phase;

    private int tableSize = 1024;
    private float[] lfoTable;

    public LFO(float sampleRate)
    {
        this.sampleRate = sampleRate;
        tableSize = 1024;
        lfoTable = new float[tableSize];

        SetLFO(LFOShape.Sin);
    }

    public void SetLFO(LFOShape s)
    {
        for (int i = 0; i < tableSize; i++)
        {
            lfoTable[i] = Mathf.Sin((float)i/(tableSize-1)*Mathf.PI);
        }
    }

    public void SetFrequency(float f)
    {
        this.frequency = f;
        increment = f / sampleRate * tableSize;
    }
        
    public float GetValue()
    {
        phase += increment;
        if ((int)phase >= tableSize) phase = 0;

        return lfoTable[(int)phase];
    }
}



public enum LFOShape
{
    Sin,
    Saw,
    Square,
    CustomLinear,
    CustomBezier
}
