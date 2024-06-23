using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavetableOscillator
{
    public float gain;
    public float frequency;
    public float startPhase;


    private float increment;
    private float phase;

    private float sampleRate;

    private float[] waveTable;

    private float[] waveTable2;
    private float[] waveTable4;
    private float[] waveTable8;
    private float[] waveTable16;


    private int wavetableSize = 1024;

    public int oversampling = 4;

    private Biquad lowpass;
    private Biquad[] antialiazingfilters = new Biquad[4];

    public WavetableOscillator(float sampleR)
    {
        gain = 1.0f;
        frequency = 440f;
        startPhase = 0.0f;

        increment = 0.0f;
        phase = 0.0f;

        sampleRate = sampleR;

        lowpass = new Biquad();
        lowpass.SetCoeffs(0.1f, 0.7f, 1);

        Reset();
        SetSaw();

        
    }

    public void SetSaw()
    {
        waveTable = new float[wavetableSize];
        waveTable2 = new float[wavetableSize];
        waveTable4 = new float[wavetableSize];
        waveTable8 = new float[wavetableSize];
        waveTable16 = new float[wavetableSize];

        antialiazingfilters = new Biquad[4];
        for (int j = 0; j < 4; j++)
        {
            antialiazingfilters[j] = new Biquad();
            antialiazingfilters[j].SetCoeffs(0.5f,0.7f,0);
        }
        for (int i=0; i<wavetableSize; i++)
        {
            waveTable[i] = Mathf.Lerp(1, -1, (float)i / (wavetableSize - 1));
            for (int j = 0; j < 4; j++)
            {
                waveTable[i] = antialiazingfilters[j].Process(waveTable[i]);
            }
        }

        for (int j = 0; j < 4; j++)
        {
            antialiazingfilters[j].SetCoeffs(0.25f, 0.7f, 0);
        }

        for (int i=0; i< wavetableSize; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                waveTable2[i] = antialiazingfilters[j].Process(waveTable[i]);
            }
        }

        for (int j = 0; j < 4; j++)
        {
            antialiazingfilters[j].SetCoeffs(0.125f, 0.7f, 0);
        }

        for (int i = 0; i < wavetableSize; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                waveTable4[i] = antialiazingfilters[j].Process(waveTable[i]);
            }
        }

        for (int j = 0; j < 4; j++)
        {
            antialiazingfilters[j].SetCoeffs(0.25f/4f, 0.7f, 0);
        }

        for (int i = 0; i < wavetableSize; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                waveTable8[i] = antialiazingfilters[j].Process(waveTable[i]);
            }
        }

        for (int j = 0; j < 4; j++)
        {
            antialiazingfilters[j].SetCoeffs(0.25f/16f, 0.7f, 0);
        }

        for (int i = 0; i < wavetableSize; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                waveTable16[i] = antialiazingfilters[j].Process(waveTable[i]);
            }
        }


    }

    public void Reset()
    {
        phase = startPhase;
        startPhase = Random.Range(0, wavetableSize);
    }

    public void SetFrequency(float f)
    {
        frequency = f;
        increment = wavetableSize * frequency / sampleRate / oversampling;
    }



    public float RenderSample()
    {
        float output = 0;
        for (int i=0; i< oversampling; i++)
        {
            output += waveTable8[Mathf.FloorToInt(phase)];
            phase += increment;
            if (phase >= wavetableSize) phase -= wavetableSize;
        }
        output /= oversampling;
        output *= gain;

        return output;
    }
}
