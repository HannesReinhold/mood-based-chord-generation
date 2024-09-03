using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackDelay
{
    
    public float feedback;
    public float delaySmoothing = 0.9999f;
    public bool positiveFeedback = true;

    private float delayInSamples;

    private float sampleRate;
    private int bufferSize;
    private float[] buffer;

    private float readPointer;
    private int writePointer;

    private float smoothedDelayInSamples;

    private FirstOrderLowpass filter = new FirstOrderLowpass();
    public float filterFreq = 0.1f;
    public float smoothedFilterFreq = 0.1f;


    public void SetDelayInMs(float del)
    {
        delayInSamples = del * 0.001f * sampleRate;

        readPointer = writePointer - delayInSamples;
        while (readPointer < 0) readPointer += delayInSamples;
    }

    public void SetDelayInSamples(float del)
    {
        delayInSamples = del;

        readPointer = writePointer - delayInSamples;
        while (readPointer < 0) readPointer += delayInSamples;
    }

    public void SetDelayInHz(float freq)
    {
        delayInSamples = 1 + sampleRate / (freq != 0 ? freq : 1);
    }


    public FeedbackDelay(float sampleRate, int bufferSize)
    {
        this.bufferSize = bufferSize;
        buffer = new float[bufferSize];
        this.sampleRate = sampleRate;
    }

    public float Process(float input)
    {

        smoothedFilterFreq = 0.5f * smoothedFilterFreq + 0.5f * filterFreq;
        smoothedDelayInSamples = smoothedDelayInSamples * delaySmoothing + delayInSamples * (1-delaySmoothing);

        readPointer = writePointer - (int)smoothedDelayInSamples;
        while (readPointer < 0) readPointer += bufferSize;

        //buffer[writePointer] *= feedback;
        buffer[writePointer] = filter.ProcessSample((positiveFeedback ? buffer[(int)readPointer] : -buffer[(int)readPointer]) * feedback,0.9f) + input;

        writePointer = (writePointer + 1) % bufferSize;

        int sample = Mathf.FloorToInt(readPointer);
        float offset = readPointer - (float)sample;
        float delayedSample = (offset) * buffer[sample] + (1-offset) * buffer[(int)(sample + 1) % (int)Mathf.Max(1,bufferSize)];
        return delayedSample;
    }
}
