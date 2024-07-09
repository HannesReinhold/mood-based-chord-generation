using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haas
{

    public bool delayOnLeft = true;

    private FeedbackDelay delay;

    public Haas(float sampleRate)
    {
        delay = new FeedbackDelay(sampleRate, (int)sampleRate);
        delay.feedback = 0;

        SetDelayInMs(40);
    }

    public void SetDelayInMs(float del)
    {
        delay.SetDelayInMs(del);
    }

    public void ProcessBlock(float[] data, int numChannels)
    {
        for(int i=0; i<data.Length; i += numChannels)
        {
            if (delayOnLeft)
                data[i] = delay.Process(data[i]);
            else
                data[i + 1] = delay.Process(data[i + 1]);
        }
    }

}
