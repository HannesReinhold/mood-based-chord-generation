using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthVoice
{

    private WavetableOscillator oscillator;
    private ADSR adsr;


    private bool canPlay;

    private float velocity;

    public int noteID;

    public SynthVoice(int numChannels)
    {
        oscillator = new WavetableOscillator(48000);
        adsr = new ADSR(48000, 0.01f, 0.5f, 1, 0.01f);
        canPlay = false;
    }

    public void StartNote(int midiNote, float vel)
    {
        oscillator.SetFrequency(midiToFreq(midiNote));
        adsr.Start();
        canPlay = true;
        velocity = vel;
        noteID = midiNote;

        Debug.Log("Play");
    }

    public void StopNote(int midiNote, float vel)
    {
        adsr.Stop();
        //canPlay = false;
        velocity = vel;
    }

    public bool CanPlay()
    {
        return canPlay;
    }

    private float midiToFreq(int id)
    {
        id -= 24;
        return Mathf.Pow(2, (float)id / 12) * 440f;
    }


    public void RenderBlock(float[] data, int numChannels, int numActiveVoices)
    {
        if(adsr.forceStop) canPlay = false;

        for(int sample=0; sample<data.Length; sample+= numChannels)
        {
            if (adsr.forceStop) continue;
            float output = oscillator.RenderSample() * velocity * adsr.GetValue() * 0.1f;
            for (int channel=0; channel<numChannels; channel++)
            {
                data[sample + channel] += output;
            }
        }
    }
}
