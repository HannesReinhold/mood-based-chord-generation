using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthVoice
{

    public WavetableOscillator oscillator1;
    private ADSR adsr;
    private ADSR adsrLowpass;
    public Biquad lowpass;


    private bool canPlay;

    private float velocity;

    public int noteID;

    public SynthVoice(int numChannels)
    {
        oscillator1 = new WavetableOscillator(48000);

        oscillator1.SetSaw();
        oscillator1.numVoices = 8;
        oscillator1.randomPhase = 1;
        oscillator1.restartPhase = false;
        oscillator1.detune = 5f;

        adsr = new ADSR(48000, 1f, 0.05f, 1, 0.3f);
        canPlay = false;

        adsrLowpass = new ADSR(44800, 1f, 0.2f, 0.3f, 0.3f);
        lowpass = new Biquad();
        lowpass.type = BiquadType.Lowpass;
        lowpass.CalcCoeffs(6000,0.7f,0,BiquadType.Lowpass);
    }

    public void StartNote(int midiNote, float vel)
    {
        oscillator1.SetFrequency(MathUtils.NoteToFreq(midiNote));
        oscillator1.Reset();


        adsr.Start();
        adsrLowpass.Start();
        canPlay = true;
        velocity = vel;
        noteID = midiNote;

        Debug.Log("Play Note "+midiNote +" at frequency "+ MathUtils.NoteToFreq(midiNote));
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


    public void RenderBlock(float[] data, int numChannels, int numActiveVoices)
    {
        if(adsr.forceStop) canPlay = false;

        for(int sample=0; sample<data.Length; sample+= numChannels)
        {
            if (adsr.forceStop) continue;
            float adsrValue = adsr.GetValue();
            float adsrLowValue = adsrLowpass.GetValue();

            float output = oscillator1.RenderSample() * velocity * adsrValue * 0.1f;

            lowpass.CalcCoeffs(adsrLowValue*2200, 0.7f, 0, BiquadType.Lowpass);
            output = lowpass.Process(output);
            for (int channel=0; channel<numChannels; channel++)
            {
                data[sample + channel] += output;
            }
        }
    }
}
