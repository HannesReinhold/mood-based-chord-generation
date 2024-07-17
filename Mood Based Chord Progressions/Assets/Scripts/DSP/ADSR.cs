using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSR
{
    public float attack;
    public float decay;
    public float sustain;
    public float release;


    private float time;
    private float increment;

    private bool canPlay;
    public bool forceStop = false;

    private float currentValue;
    private float lastValue;
    private float totalTime;

    private float stopTime;

    private float releaseMult = 0;


    public ADSR(float sampleRate, float a, float d, float s, float r)
    {
        increment = 1f / sampleRate;
        canPlay = false;

        attack = a;
        decay = d;
        sustain = s;
        release = r;

        totalTime = a + d + r;
    }

    public void Start()
    {
        time = 0;
        canPlay = true;
        forceStop = false;

    }

    public void Stop()
    {
        canPlay = false;
        stopTime = time;
    }

    public float GetValue()
    {

        if(time >= attack+decay && canPlay) time += 0;
        else time += increment;
        if (time >= stopTime + release && !canPlay) forceStop = true;


        if (!canPlay)
        {
            //currentValue = Mathf.Lerp(lastValue, 0, (time - stopTime) / release);
            currentValue = (1 - ((time - stopTime) / release)) * lastValue;
            return currentValue;
        }
        else lastValue = currentValue;
        if (time < attack) {
            //currentValue = Mathf.Lerp(0, 1, time / attack);
            currentValue = time / attack;
        }
        else if (time >= attack && canPlay) {
            //currentValue = Mathf.Lerp(1, sustain, (time - attack) / decay);
            float t = ((time - attack) / decay);
            currentValue = 1*(1-t)+sustain*t;
        }
        
        return currentValue;
    }

}
