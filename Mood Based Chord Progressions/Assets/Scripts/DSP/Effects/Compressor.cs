using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compressor
{
    public float upperThreshold = -8;
    public float lowerThreshold = -8;
    public float downwardsRatio = 8;
    public float upwardsRatio = 8;
    public float attack = 2;
    public float release = 20;


    private float[] lookaheadBuffer;
    private int bufferSize;

    float envelope = 0;

    int readP;


    public Compressor(int s)
    {
        bufferSize = s;
        lookaheadBuffer = new float[bufferSize];

        SetAttack(attack);
        SetRelease(release);
    }

    public void SetAttack(float attack)
    {
        this.attack = (attack == 0.0f) ? (0.0f) : Mathf.Exp(-1.0f / (48000f * attack*0.001f));
        
    }

    public void SetRelease(float release)
    {
        this.release = (release == 0.0f) ? (0.0f) : Mathf.Exp(-1.0f / (48000f * release*0.001f));
    }



    public float Process(float input)
    {
        float gain = Mathf.Abs(input);

        float envState = gain > envelope ? attack : release;

        envelope = (1 - envState) * gain + envState * envelope;

        float db = MathUtils.LinToDb(envelope);
        float diff = 1;
        if (db > upperThreshold) diff = MathUtils.DbToLin((upperThreshold - db) * (1 - 1 / downwardsRatio));
        else if(db < lowerThreshold) diff = MathUtils.DbToLin((lowerThreshold - db) * (1 - 1 / upwardsRatio));

        return diff*input;
    }



}
