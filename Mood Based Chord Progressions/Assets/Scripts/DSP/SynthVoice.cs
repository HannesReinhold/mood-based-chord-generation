using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthVoice
{

    private WavetableOscillator oscillator1;
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
        oscillator1.numVoices = 1;
        oscillator1.randomPhase = 0;
        oscillator1.restartPhase = true;
        oscillator1.detune = 0.05f;

        adsr = new ADSR(48000, 0.01f, 0.2f, 0, 0.1f);
        canPlay = false;

        adsrLowpass = new ADSR(44800, 0.02f, 0.5f, 1, 0.1f);
        lowpass = new Biquad();
        lowpass.type = BiquadCalculator.BiquadType.LOWPASS;
        lowpass.SetCoeffs(6000,0.7f,0,BiquadCalculator.BiquadType.LOWPASS);
    }

    public void StartNote(int midiNote, float vel)
    {
        oscillator1.SetFrequency(MathUtils.NoteToFreq(midiNote-69));
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

            output = lowpass.Process(output);
            for (int channel=0; channel<numChannels; channel++)
            {
                data[sample + channel] += output;
            }
        }
    }
}
