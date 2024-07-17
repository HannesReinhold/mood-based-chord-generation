using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chorus
{

    public int numDelays = 1;
    private int maxNumDelays;

    public float feedback = 0;
    public float delaySmoothing = 0.9999f;
    public bool positiveFeedback = true;
    public float minDelay;
    public float maxDelay;
    public float speed = 0.1f;
    public float stereoSpread = 1;


    private float modPhase = 0;
    private FeedbackDelay[] delays;

    float[] pannings;

    private float[] lfo;
    private int lfoSize = 512;



    public void SetDelayRangeInMs(float min, float max)
    {
        minDelay = min;
        maxDelay = max;
    }

    public Chorus(float sampleRate, int bufferSize, int maxDelays)
    {
        this.maxNumDelays = maxDelays;
        this.numDelays = maxNumDelays;
        pannings = new float[maxNumDelays];

        SetDelayRangeInMs(2,20);

        delays = new FeedbackDelay[maxDelays];
        for(int i=0; i<maxDelays; i++)
        {
            delays[i] = new FeedbackDelay(48000, 5000);
            pannings[i] = (float)i /(maxDelays-1);
            Debug.Log(pannings[i]);
        }
        if (maxNumDelays == 1) pannings[0] = 0.5f;

        lfo = new float[lfoSize];

        for(int i=0; i<lfoSize; i++)
        {
            lfo[i] = Mathf.Sin(Mathf.PI*i/(float)lfoSize)*0.5f+0.5f;
        }
    }


    public void ProcessBlock(float[] data, int numChannels)
    {
        // old chorus code 
        /*
        for(int i=0; i<data.Length; i += numChannels)
        {
            float delayedSum = 0;

            if(i%10 ==0)
            {
                for(int j=0; j<numDelays; j++)
                {
                    delayMod[j] = Mathf.Lerp(minDelay, maxDelay,Mathf.Sin(modPhase + Mathf.Lerp(0,Mathf.PI*2, (float)j/numDelays))*0.5f+0.5f);
                }
                modPhase += Mathf.PI * 1 / 48000f;
            }

            buffer[writePointer] = data[i];

            for(int j=0; j<numDelays; j++)
            {
                smoothedDelayInSamples[j] = smoothedDelayInSamples[j] * delaySmoothing + (delayInSamples[j]+delayMod[j] * 48) * (1 - delaySmoothing);

                readPointer[j] = writePointer - (int)smoothedDelayInSamples[j];
                while (readPointer[j] < 0) readPointer[j] += bufferSize;

                //buffer[writePointer] *= feedback;
                buffer[writePointer] += (positiveFeedback ? buffer[(int)readPointer[j]] : -buffer[(int)readPointer[j]]) * feedback;


                int sample = Mathf.FloorToInt(readPointer[j]);
                float offset = readPointer[j] - (float)sample;
                while (sample < 0) sample += bufferSize;
                delayedSum += (offset) * buffer[sample%bufferSize] + (1 - offset) * buffer[(int)(sample + 1) % (int)Mathf.Max(1, bufferSize)];
            }
            

            writePointer = (writePointer + 1) % bufferSize;

            data[i] = data[i] + delayedSum;
            data[i + 1] = data[i + 1] + delayedSum;

        }
        */

        for(int i=0; i<numDelays; i++)
        {
            delays[i].feedback = feedback;
        }


        for (int i = 0; i < data.Length; i += numChannels) {

            if (i % 10 == 0)
            {
                for (int j = 0; j < numDelays; j++)
                {
                    //delays[j].SetDelayInMs(Mathf.Lerp(minDelay, maxDelay, Mathf.Sin(modPhase + Mathf.Lerp(0, Mathf.PI * 2, (float)j / numDelays)) * 0.5f + 0.5f));
                    int index=(int)((modPhase+(float)j/numDelays)*lfoSize);
                    if(index >= lfoSize) index -= lfoSize;
                    delays[j].SetDelayInMs(minDelay + lfo[index] * (maxDelay-minDelay));
                }
                modPhase += speed / 4800f;
                if (modPhase >= 1) modPhase = 0;
            }

            float sumLeft = 0;
            float sumRight = 0;
            for (int j = 0; j < numDelays; j++)
            {
                float delayedSample = delays[j].Process(data[i]+data[i+1]);
                sumLeft += delayedSample * (1-pannings[j]);
                sumRight += delayedSample * (pannings[j]);
            }

            data[i] += sumLeft * 0.25f;
            data[i + 1] += sumRight * 0.25f;
        }
    }
}
