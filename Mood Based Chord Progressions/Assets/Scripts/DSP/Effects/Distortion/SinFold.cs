using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinFold : DistortionBase
{
    public override float ProcesSample(float input)
    {
        if(overSample == 1) 
            return System.MathF.Sin((input * drive + dcOffset) * Mathf.PI);

        float sum = 0;
        for(int i=0; i<overSample; i++)
        {
            float sampleOffset = i * overSampleIncrement;
            float sample = lastSample * (1-sampleOffset) + input * sampleOffset;
            sum+= System.MathF.Sin((sample * drive + dcOffset) * Mathf.PI);
        }
        return sum * overSampleIncrement;
    }

    public override void ProcessBlock(float[] buffer, int numChannels)
    {
        for (int i = 0; i < buffer.Length; i += numChannels)
        {
            for (int j = 0; j < numChannels; j++)
            {
                buffer[i] = ProcesSample(buffer[i]);
            }
        }
    }
}
