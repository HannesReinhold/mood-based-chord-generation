using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardClip : DistortionBase
{
    public override float ProcesSample(float input)
    {
        if (overSample == 1)
        {
            input += dcOffset;
            input *= drive;
            return input > 1 ? 1 : input < -1 ? -1 : input;
        }

        float sum = 0;
        for (int i = 0; i < overSample; i++)
        {
            float sampleOffset = i * overSampleIncrement;
            float sample = lastSample * (1 - sampleOffset) + input * sampleOffset;
            sample *= drive;
            sample += dcOffset;
            sum += sample > 1 ? 1 : sample < -1 ? -1 : sample;
        }
        return sum * overSampleIncrement;
    }

    public override void ProcessBlock(float[] buffer, int numChannels)
    {
        for(int i=0; i< buffer.Length; i += numChannels)
        {
            for(int j=0; j<numChannels; j++)
            {
                buffer[i] = ProcesSample(buffer[i]);
            }
        }
    }
}
