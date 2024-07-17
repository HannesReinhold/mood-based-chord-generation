using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granular
{
    public int bufferSize;
    public float[] buffer;
    private float sampleRate;
    private float sampleTime;

    private int maxNumGrains;

    private int writePointer;

    private float[] grainPointers;
    private float[] grainTimes;
    private bool[] updateGrain;
    private float[] grainPitch;
    private float[] grainPan;

    private int currentGrainID = 0;

    System.Random random = new System.Random();

    public float grainSpawnRate = 1;
    private float grainSpawnTimer;
    private float grainSpawnTime;
    private float[] grainPitches = new float[1] {1};


    float[] window = new float[512];


    public Granular(float sampleRate, int bufferSize, int maxNumGrains)
    {
        this.sampleRate = sampleRate;
        sampleTime = 1f / sampleRate;
        writePointer = 0;
        this.bufferSize = bufferSize;
        buffer = new float[bufferSize];

        this.maxNumGrains = maxNumGrains;
        grainPointers = new float[maxNumGrains];
        grainTimes = new float[maxNumGrains];
        updateGrain = new bool[maxNumGrains];
        grainPitch = new float[maxNumGrains];
        grainPan = new float[maxNumGrains];

        grainSpawnTimer = 0;

        SetGrainSpawnRate(50);

        for(int i=0; i<512; i++)
        {
            //window[i] = Mathf.Sin(Mathf.PI*(float)i/(511));
            window[i] = Mathf.Lerp(1,0,(float)i/511);
        }

    }

    public void SetGrainSpawnRate(float rate)
    {
        grainSpawnTime = sampleRate / (rate);
    }

    private void SpawnGrain()
    {
        //grainPointers[currentGrainID] = random.Next(48000, bufferSize - 48000);
        grainPointers[currentGrainID] = writePointer - (1+random.Next(20000));
        while (grainPointers[currentGrainID] < 0) grainPointers[currentGrainID] += bufferSize;
        if (grainPointers[currentGrainID] > bufferSize) grainPointers[currentGrainID] -= bufferSize;
        grainTimes[currentGrainID] = 0;
        updateGrain[currentGrainID] = true;
        grainPitch[currentGrainID] = grainPitches[random.Next(0, grainPitches.Length)] + (float)random.NextDouble() * 0.01f;
        //grainPitch[currentGrainID] = grainPitches[random.Next(0, grainPitches.Length)];
        grainPan[currentGrainID] = (float)random.NextDouble()*0+0.5f;

        currentGrainID++;
        if (currentGrainID >= maxNumGrains) currentGrainID = 0;

    }

    private void StopGrain(int i)
    {
        grainTimes[currentGrainID] = 0;
        updateGrain[i] = false;
    }

    public void ProcessBlock(float[] data, int numChannels)
    {

        if (grainPitches[0] == 0) return;

        for (int j = 0; j < data.Length; j += numChannels)
        {
            float outputLeft = 0;
            float outputRight = 0;

            grainSpawnTimer++;
            if (grainSpawnTimer >= grainSpawnTime)
            {
                grainSpawnTimer = 0;
                SpawnGrain();
            }



            for (int i = 0; i < maxNumGrains; i++)
            {
                if (!updateGrain[i]) continue;

                float grainSample = buffer[(int)grainPointers[i]] * window[(int)(grainTimes[i]*512f)];
                outputLeft += grainSample * (1-grainPan[i]);
                outputRight += grainSample * (grainPan[i]);

                grainTimes[i] += sampleTime * grainPitch[i];
                grainPointers[i] += grainPitch[i];


                if ((int)grainPointers[i] >= bufferSize) grainPointers[i] -= bufferSize;
                //if ((int)grainPointers[i] < 0) grainPointers[i] += bufferSize;
                if (grainTimes[i] >= 1) StopGrain(i);
            }

            buffer[writePointer++] = data[j]+data[j+1];
            if (writePointer >= bufferSize) writePointer -= bufferSize;


            data[j] = outputLeft;
            data[j + 1] = outputRight;
        }
    }
}
