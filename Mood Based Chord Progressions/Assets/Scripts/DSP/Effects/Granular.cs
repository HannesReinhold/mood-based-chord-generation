using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granular
{
    public int bufferSize;
    public float[] buffer;
    private float sampleRate;

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


    public Granular(float sampleRate, int bufferSize, int maxNumGrains)
    {
        this.sampleRate = sampleRate;
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

        SetGrainSpawnRate(20);
    }

    public void SetGrainSpawnRate(float rate)
    {
        grainSpawnTime = sampleRate / rate;
    }

    private void SpawnGrain()
    {
        grainPointers[currentGrainID] = random.Next(48000, bufferSize - 48000);
        grainTimes[currentGrainID] = 0;
        updateGrain[currentGrainID] = true;
        grainPitch[currentGrainID] = 1 + (float)random.NextDouble() * 0.01f;
        grainPan[currentGrainID] = (float)random.NextDouble();

        currentGrainID++;
        if (currentGrainID >= maxNumGrains) currentGrainID = 0;

        Debug.Log("Spawn Grain");
    }

    private void StopGrain(int i)
    {
        grainTimes[currentGrainID] = 0;
        updateGrain[i] = false;
    }

    public void ProcessBlock(float[] data, int numChannels)
    {
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

                float grainSample = buffer[(int)grainPointers[i]] * Mathf.Sin(grainTimes[i] * Mathf.PI);
                outputLeft += grainSample * (1-grainPan[i]);
                outputRight += grainSample * (grainPan[i]);

                grainTimes[i] += 1f / sampleRate * 1f;
                grainPointers[i] += 1f * grainPitch[i];


                if ((int)grainPointers[i] >= bufferSize) grainPointers[i] -= bufferSize;
                if (grainTimes[i] >= 1) StopGrain(i);
            }

            buffer[writePointer++] = data[j];
            if (writePointer >= bufferSize) writePointer -= bufferSize;


            data[j] = outputLeft * 0.25f;
            data[j + 1] = outputRight * 0.25f;
        }
    }
}
