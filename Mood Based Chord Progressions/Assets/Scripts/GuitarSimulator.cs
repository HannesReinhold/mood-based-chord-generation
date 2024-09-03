using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GuitarSimulator : MonoBehaviour
{
    System.Random rand = new System.Random();
    FeedbackDelay delay1 = new FeedbackDelay(48000, 48000);
    FeedbackDelay delay2 = new FeedbackDelay(48000, 48000);
    FeedbackDelay delay3 = new FeedbackDelay(48000, 48000);

    Compressor comp = new Compressor(100);
    FirstOrderLowpass burstFilter = new FirstOrderLowpass();

    Biquad lowGain = new Biquad();


    public bool burst = false;
    public float burstTimeInMs = 1;
    public float burstIntensity = 1;
    public float burstFilterFreq = 0.3f;
    private float burstTimer = 0;

    [Range(0, 10000)] public float delayHz = 100;
    [Range(0, 0.9999f)] public float feedback;
    [Range(0, 0.9999f)] public float filterFreq = 0.3f;
    public bool negativeFeedback = false;
    public bool enableFeedbackDelay = false;

    private bool isPlaying = false;

    private float stringDelay1 = 0;
    private float stringDelay2 = 0;
    private float stringDelay3 = 0;


    private void Start()
    {
        comp.upwardsRatio = 1;
        comp.downwardsRatio = 4;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) burst = true;

        if (burst)
        {
            burst = false;
            burstTimer = 0;
            //delay1.filterFreq = 0;
            isPlaying = true;

            stringDelay1 = (float)rand.NextDouble() * 300;
            stringDelay2 = 2000 + (float)rand.NextDouble() * 300;
            stringDelay3 = 4500 + (float)rand.NextDouble() * 300;

        }
        delay1.SetDelayInHz(MathUtils.NoteToFreq(64));
        delay2.SetDelayInHz(MathUtils.NoteToFreq(68));
        delay3.SetDelayInHz(MathUtils.NoteToFreq(71));

        delay1.feedback = feedback;
        delay1.filterFreq = filterFreq;
        delay1.positiveFeedback = !negativeFeedback;

        delay2.feedback = feedback;
        delay2.filterFreq = filterFreq;
        delay2.positiveFeedback = !negativeFeedback;

        delay3.feedback = feedback;
        delay3.filterFreq = filterFreq;
        delay3.positiveFeedback = !negativeFeedback;

        lowGain.CalcCoeffs(100, 0.6f, 10, BiquadType.Lowshelf);


    }





    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            if (!isPlaying) return;
            float impulseGain = Mathf.Clamp(1f / (burstTimer / (4.8f * burstTimeInMs)), 0, 1);
            burstTimer++;
            //float impulse = (float)rand.NextDouble()*impulseGain;
            float impulse1 = Mathf.Sin(Mathf.Max(0,burstTimer - stringDelay1) * Mathf.PI * burstIntensity + Mathf.Sin(Mathf.Max(0, burstTimer - stringDelay1) * Mathf.PI * burstIntensity)) * Mathf.Clamp(1f / (Mathf.Max(0, burstTimer - stringDelay1) / (4.8f * burstTimeInMs)), 0, 1) * burstFilter.ProcessSample((float)rand.NextDouble(), burstFilterFreq);
            float impulse2 = Mathf.Sin(Mathf.Max(0, burstTimer - stringDelay2) * Mathf.PI * burstIntensity + Mathf.Sin(Mathf.Max(0, burstTimer - stringDelay2) * Mathf.PI * burstIntensity)) * Mathf.Clamp(1f / (Mathf.Max(0, burstTimer - stringDelay2) / (4.8f * burstTimeInMs)), 0, 1) * burstFilter.ProcessSample((float)rand.NextDouble(), burstFilterFreq);
            float impulse3 = Mathf.Sin(Mathf.Max(0, burstTimer - stringDelay3) * Mathf.PI * burstIntensity + Mathf.Sin(Mathf.Max(0, burstTimer - stringDelay3) * Mathf.PI * burstIntensity)) * Mathf.Clamp(1f / (Mathf.Max(0, burstTimer - stringDelay3) / (4.8f * burstTimeInMs)), 0, 1) * burstFilter.ProcessSample((float)rand.NextDouble(), burstFilterFreq);

            float output = 0;
            if (enableFeedbackDelay)
            {
                output += delay1.Process(impulse1);
                output += delay2.Process(impulse2);
                output += delay3.Process(impulse3);
                
                output = comp.Process(output);
            }

            output = lowGain.Process(output);



            data[i] = output;
            data[i + 1] = output;
        }
    }
}
