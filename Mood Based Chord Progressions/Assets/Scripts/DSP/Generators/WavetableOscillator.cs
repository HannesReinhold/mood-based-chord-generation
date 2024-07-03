using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;

public class WavetableOscillator
{
    // basic params
    public float gain = 1;
    public float frequency = 440f;
    public float startPhase = 0;

    // voices
    public int maxNumVoices = 16;
    public int numVoices = 1;
    public float detune = 0;
    public float randomPhase = 0;
    public bool restartPhase = true;
    


    public int oversampling = 1;


    
    private float[] increment;
    private float[] phase;

    private float sampleRate;

    private int wavetableSize = 1024;
    private float[] waveTable;

    private float[,] antializedWaveTable;

    
    private int wavetableID=0;

    

    public WavetableOscillator(float sampleR)
    {
        gain = 1.0f;
        frequency = 440f;
        startPhase = 0.0f;

        increment = new float[maxNumVoices];
        phase = new float[maxNumVoices];

        sampleRate = sampleR;

        Reset();
        SetSquare();

        
    }

    public void SetSaw()
    {
        waveTable = new float[wavetableSize];

        for (int i=0; i<wavetableSize; i++)
        {
            waveTable[i] = Mathf.Lerp(1, -1, (float)i / (wavetableSize - 1));

        }


        Antialiazing();
    }

    public void SetSquare()
    {
        waveTable = new float[wavetableSize];

        for (int i = 0; i < wavetableSize; i++)
        {
            waveTable[i] = i>=wavetableSize/2 ? 1 : -1;

        }


        Antialiazing();
    }

    public void SetSine()
    {
        waveTable = new float[wavetableSize];

        for (int i = 0; i < wavetableSize; i++)
        {
            waveTable[i] = Mathf.Sin((float)i/wavetableSize * 2 * Mathf.PI);

        }


        Antialiazing();
    }

    private void Antialiazing()
    {

        antializedWaveTable = new float[8, wavetableSize];

        for (int j = 0; j < 8; j++)
        {
            Complex[] complexWaveTable = new Complex[wavetableSize];

            for (int i = 0; i < wavetableSize; i++)
            {
                complexWaveTable[i] = new Complex(waveTable[i], 0);

            }

            FFT.PerformFFT(complexWaveTable);

            float div = (float)wavetableSize / Mathf.Pow(2, (float)j);
            for (int i = 0; i < wavetableSize; i++)
            {
                if (i > div) complexWaveTable[i] = new Complex(0, 0);
            }

            FFT.PerformIFFT(complexWaveTable);

            for (int i = 0; i < wavetableSize; i++)
            {
                antializedWaveTable[j,i] = (float)complexWaveTable[i].Real;
            }
        }
    }

    public void Reset()
    {
        if (restartPhase) return;

        System.Random rand = new System.Random();

        startPhase = (float)rand.NextDouble() * wavetableSize * randomPhase;

        for (int i=0; i<numVoices; i++)
        {
            phase[i] = (startPhase + (float)rand.NextDouble() * wavetableSize * randomPhase)% wavetableSize;
        }
        
    }

    public void AddPhase(float p)
    {
        for(int i=0; i<numVoices; i++)
        {
            phase[i] += p;
        }
    }

    public void SetFrequency(float f)
    {
        frequency = f;
        for(int i=0; i < numVoices; i++)
        {
            float midiNote = MathUtils.FreqToCent(f);
            float detunedFreq = MathUtils.CentToFreq(midiNote+ Mathf.Lerp(-detune, detune, (float)i / (numVoices - 1)));

            increment[i] = wavetableSize * detunedFreq / sampleRate / oversampling;

            Debug.Log(detunedFreq + " "+f);
        }

        if(numVoices == 1) increment[0] = wavetableSize * (frequency) / sampleRate / oversampling;

        // set antialiazin wavetable based on frequency
        if (frequency > 2500) wavetableID = 7;
        else if (frequency > 1250) wavetableID = 6;
        else if (frequency > 680) wavetableID = 5;
        else if (frequency > 340) wavetableID = 4;
        else if (frequency > 171) wavetableID = 3;
        else if (frequency > 85) wavetableID = 2;
        else wavetableID = 1;
    }


    public float RenderSample()
    {
        float output = 0;

        for (int i=0; i< oversampling; i++)
        {
            for(int j=0; j<numVoices; j++)
            {
                phase[j] += increment[j];
                if (Mathf.FloorToInt(phase[j]) < 0) phase[j] += wavetableSize;
                if (phase[j] >= wavetableSize) phase[j] -= wavetableSize;
                output += antializedWaveTable[wavetableID,Mathf.FloorToInt(phase[j])];
                
                
            }
        }

        output /= oversampling;
        output *= gain;

        return output;
    }
}
