using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MidiDevice
{

    public int numChannels = 2;
    public int maxVoices = 16;

    public float masterGain = 1;
    public float drive = 1;
    public float dcOffset;
    public float cutoffFrequency = 22000;
    public float Q = 1;

    public int octave = 0;

    private int numActiveVoices = 0;
    public SynthVoice[] voices;

    // Effects
    private HardClip dist = new HardClip();


    public override void StartNote(int noteID)
    {
        PlayNextAvailableVoice(noteID);
    }

    public override void StopNote(int noteID)
    {
        StopVoice(noteID);
    }

    public override void StopAllNotes()
    {
        StopAllVoices();
    }


    public void PlayNextAvailableVoice(int noteID)
    {
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) { voices[i].StartNote(noteID+octave*12, 1); numActiveVoices++;  return; }
        }
    }

    public void StopVoice(int noteID)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].noteID == noteID && voices[i].CanPlay())
            {
                voices[i].StopNote(noteID+octave*12, 1);
                numActiveVoices--;
                return;
            }
        
        }
    }

    public void StopAllVoices()
    {
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].CanPlay())
            {
                voices[i].StopNote(i, 1);
                numActiveVoices--;
                //return;
            }

        }
    }


    public void PrepareToPlay()
    {
        // Setup voices
        voices = new SynthVoice[maxVoices];

        for(int i = 0; i < voices.Length; i++)
        {
            voices[i] = new SynthVoice(numChannels);
        }

        // Setup Effects
    }

    public void ProcessBlock(float[] data, int numChannels)
    {
        dist.SetDrive(drive);
        dist.SetDcOffset(dcOffset);

        for(int i=0; i<voices.Length; i++)
        {
            voices[i].lowpass.SetCoeffs(cutoffFrequency, Q, 0);
        }

        // Get samples from generators per voice
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) continue;

            voices[i].RenderBlock(data, numChannels, numActiveVoices);
        }


        // Process effects
        for (int i=0; i<data.Length; i++)
        {
            data[i] = dist.ProcesSample(data[i]) * masterGain;
        }

        
    }
}
