using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthVoice
{

    private WavetableOscillator oscillator1;
    private WavetableOscillator oscillator2;
    private ADSR adsr;
    private ADSR adsrLowpass;
    private Biquad lowpass;


    private bool canPlay;

    private float velocity;

    public int noteID;

    public SynthVoice(int numChannels)
    {
        oscillator1 = new WavetableOscillator(48000);
        oscillator2 = new WavetableOscillator(48000);

        oscillator1.SetSine();
        oscillator2.SetSine();

        oscillator2.gain = 0.5f;
        adsr = new ADSR(48000, 0.01f, 0.7f, 1, 0.1f);
        canPlay = false;

        adsrLowpass = new ADSR(44800, 0.02f, 0.5f, 1, 0.1f);
        lowpass = new Biquad();
    }

    public void StartNote(int midiNote, float vel)
    {
        oscillator1.SetFrequency(midiToFreq(midiNote-36));
        oscillator1.Reset();

        oscillator2.SetFrequency(midiToFreq(midiNote));
        oscillator2.Reset();

        adsr.Start();
        adsrLowpass.Start();
        canPlay = true;
        velocity = vel;
        noteID = midiNote;

        Debug.Log("Play Note "+midiNote +" at frequency "+ midiToFreq(midiNote));
    }

    public void StopNote(int midiNote, float vel)
    {
        adsr.Stop();
        adsr.Stop();
        velocity = vel;
    }

    public bool CanPlay()
    {
        return canPlay;
    }

    private float midiToFreq(int id)
    {
        return Mathf.Pow(2, (float)id / 12) * 440f;
    }

    private float midiToFreq(float id)
    {
        return Mathf.Pow(2, id / 12f);
    }


    public void RenderBlock(float[] data, int numChannels, int numActiveVoices)
    {
        if(adsr.forceStop) canPlay = false;

        for(int sample=0; sample<data.Length; sample+= numChannels)
        {
            if (adsr.forceStop) continue;
            float adsrValue = adsr.GetValue();
            float adsrLowValue = adsrLowpass.GetValue();

            float mod = oscillator2.RenderSample() * velocity * adsrValue * 0.1f;
            oscillator1.AddPhase(mod*1000f);

            float output = oscillator1.RenderSample() * velocity * adsrValue * 0.1f;

            


            lowpass.SetCoeffs(Mathf.Max(10, Mathf.Min(midiToFreq(adsrLowValue*172f),22000)),1,0);
            output = lowpass.Process(output);
            for (int channel=0; channel<numChannels; channel++)
            {
                data[sample + channel] += output;
            }
        }
    }
}
