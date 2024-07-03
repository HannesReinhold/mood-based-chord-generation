using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanFold : DistortionBase
{
    public override float ProcesSample(float input)
    {
        if (overSample == 1)
        {
            input *= drive;
            input += dcOffset;

            if (input > 1) input = 1;
            else if (input < -1) input = -1;

            return System.MathF.Tan(input * Mathf.PI / 4f);
        }

        float sum = 0;
        for (int i = 0; i < overSample; i++)
        {
            float sampleOffset = i * overSampleIncrement;
            float sample = lastSample * (1 - sampleOffset) + input * sampleOffset;

            sample *= drive;
            sample += dcOffset;

            if (sample > 1) sample = 1;
            else if (sample < -1) sample = -1;

            sum += System.MathF.Tan(sample * Mathf.PI / 4f);
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
