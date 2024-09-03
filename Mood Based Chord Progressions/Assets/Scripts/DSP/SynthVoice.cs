using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SynthVoice
{

    public WavetableOscillator oscillator1;
    public ADSR adsr;
    public ADSR adsrLowpass;
    public Biquad lowpass;

    private System.Random rand = new System.Random();

    private bool canPlay;
    private bool isPlaying;

    private float velocity;

    public int noteID;

    public float time = 0;

    private int filterUpdateTimer = 0;

    public Parameter freq = new Parameter();

    public int startTimer=0;
    public int startTime=0;


    public float filterFreq = 0.7f;
    public float noiseVolume = 0;

    public SynthVoice(int numChannels)
    {
        noteID = -100;
        oscillator1 = new WavetableOscillator(48000);

        oscillator1.SetSaw();
        oscillator1.numVoices = 1;
        oscillator1.randomPhase = 1;
        oscillator1.restartPhase = false;
        oscillator1.detune = 20f;

        adsr = new ADSR(48000, 0.001f, 0.5f, 0.8f, 0.2f);
        canPlay = false;

        adsrLowpass = new ADSR(4480/1f, 0.001f, 0.4f, 0.5f, 0.5f);
        lowpass = new Biquad();
        lowpass.type = BiquadType.Lowpass;
        lowpass.CalcCoeffs(22000,0.7f,0,BiquadType.Lowpass);
    }

    public void StartNote(int midiNote, float vel, int startTime)
    {
        this.startTime = startTime;
        startTimer = 0;

        oscillator1.SetFrequency(MathUtils.NoteToFreq(midiNote-5));
        oscillator1.Reset();


        adsr.Start();
        adsrLowpass.Start();
        canPlay = true;
        isPlaying = true;
        velocity = vel;
        noteID = midiNote;

        //Debug.Log("Play Note "+midiNote +" at frequency "+ MathUtils.NoteToFreq(midiNote));
        time = 0;

        filterUpdateTimer = 10;
    }

    public void StopNote(int midiNote, float vel)
    {
        time = 0;
        adsr.Stop();
        velocity = vel;
        isPlaying = false;
    }

    public bool CanPlay()
    {
        return canPlay;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }


    public void RenderBlock(float[] data, int numChannels, int numActiveVoices)
    {
        if(adsr.forceStop) canPlay = false;

        

        for(int sample=0; sample<data.Length; sample+= numChannels)
        {
            startTimer++;
            if (startTimer < startTime) continue;
            

            if (adsr.forceStop) { canPlay = false; noteID = -100; continue; }
            time += 1;
            float adsrValue = adsr.GetValue();

            oscillator1.AddPhase(freq.valueBuffer[sample]);
            float output = oscillator1.RenderSample() * velocity * adsrValue * 0.1f;

            if (filterUpdateTimer >= 10)
            {
                float adsrLowValue = adsrLowpass.GetValue();
                lowpass.CalcCoeffs(MathUtils.NoteToFreq(adsrLowValue*120), filterFreq, 0, BiquadType.Lowpass);
                filterUpdateTimer = 0;
            }
            filterUpdateTimer++;
            output = lowpass.Process(output);
            for (int channel=0; channel<numChannels; channel++)
            {
                data[sample + channel] += output + (float)rand.NextDouble()*noiseVolume;
            }
        }
    }
}
