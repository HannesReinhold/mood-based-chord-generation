using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spatializer
{
    public bool enableITD;
    public bool enableILD;
    public bool enableIID;

    private FeedbackDelay delayLeft;
    private FeedbackDelay delayRight;

    private FirstOrderLowpass lowpassLeft;
    private FirstOrderLowpass lowpassRight;

    private Vector3 sourceDir=new Vector3();
    private float distance;

    private float itdLeft=0;
    private float itdRight=0;

    private float ildLeft=0;
    private float ildRight=0;

    private float iidLeft=0;
    private float iidRight=0;

    private float attenuation=1;

    public Spatializer(float sampleRate)
    {
        // setup itd delays
        delayLeft = new FeedbackDelay(sampleRate, (int)sampleRate);
        delayLeft.feedback = 0;
        delayRight = new FeedbackDelay(sampleRate, (int)sampleRate);
        delayRight.feedback = 0;
        delayLeft.delaySmoothing = 0.9f;
        delayRight.delaySmoothing = 0.9f;

        // setup iid filters
        lowpassLeft = new FirstOrderLowpass();
        lowpassRight = new FirstOrderLowpass();
    }

    public void SetDirection(Vector3 dir, float dist)
    {
        sourceDir = dir;
        distance = dist;

        UpdateSpatializer();
    }

    public void ProcessBlock(float[] data, int numChannels)
    {
        int timer = 0;
        for (int i = 0; i < data.Length; i += numChannels)
        {

            float mono = data[i] + data[i + 1];
            data[i] = lowpassLeft.ProcessSample(delayLeft.Process(mono), iidLeft)*ildLeft * attenuation;
            data[i + 1] = lowpassRight.ProcessSample(delayRight.Process(mono), iidRight) * ildRight * attenuation;
        }
    }

    private void UpdateSpatializer()
    {
        itdLeft = Mathf.Max(0.025f,(1 - (Vector3.Dot(new Vector3(-1,0,0), sourceDir)+1)*0.5f) * 0.7f);
        itdRight = Mathf.Max(0.025f,(1 - (Vector3.Dot(new Vector3(1, 0, 0), sourceDir) + 1) * 0.5f) * 0.7f);

        delayLeft.SetDelayInMs(itdLeft);
        delayRight.SetDelayInMs(itdRight);

        float ildFront = (Vector3.Dot(new Vector3(0,0,1), sourceDir)+0.5f)*0.25f;
        float ildTop = (Vector3.Dot(new Vector3(0, 1, 0), sourceDir)+0.5f) * 0.25f;
        ildLeft = Mathf.Lerp(0.2f,1, (Vector3.Dot(new Vector3(-1, 0, 0), sourceDir) + 1) * 0.5f + ildFront);
        ildRight = Mathf.Lerp(0.2f, 1, (Vector3.Dot(new Vector3(1, 0, 0), sourceDir) + 1) * 0.5f + ildFront);

        iidLeft = Mathf.Min(Mathf.Max(ildLeft+ildFront*0.25f + ildTop * 0.25f, 0),1);
        iidRight = Mathf.Min(Mathf.Max(ildRight + ildFront * 0.25f + ildTop * 0.25f, 0), 1);

        attenuation = 0.1f / Mathf.Max(0.1f, Mathf.Pow(distance,0.95f)*0.45f);

        //Debug.Log("ITD: "+itdLeft+" "+itdRight + ", ILD: "+ildLeft+" "+ildRight+", IID: "+iidLeft+" "+iidRight);
    }
}
