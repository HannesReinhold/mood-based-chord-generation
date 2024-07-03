using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownSample : DistortionBase
{

    float currentTime=0;
    float currentSample = 0;
    public override float ProcesSample(float input)
    {
        float clampedDrive = Mathf.Max(drive, 1);
        if (overSample == 1)
        {
            input += dcOffset;

            if (input > 1) input = 1;
            else if (input < -1) input = -1;

            currentTime += drive / 48000f;
            if(currentTime > 1)
            {
                currentTime = 0;
                currentSample = input;
            }
            return currentSample;
        }

        float sum = 0;
        for (int i = 0; i < overSample; i++)
        {
            float sampleOffset = i * overSampleIncrement;
            float sample = lastSample * (1 - sampleOffset) + input * sampleOffset;

            sample += dcOffset;

            if (sample > 1) sample = 1;
            else if (sample < -1) sample = -1;

            currentTime += drive * overSampleIncrement / 48000f;
            if (currentTime > 1)
            {
                currentTime = 0;
                currentSample = input;
            }

            sum += currentSample;
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
