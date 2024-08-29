using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MidiDevice
{

    public int numChannels = 2;
    public int maxVoices = 32;

    public float masterGain = 1;
    public float panning = 0.5f;
    public float drive = 1;
    public float dcOffset;
    public float cutoffFrequency = 22000;
    public float Q = 1;

    public int octave = 0;

    [Range(0, 1)] public float phaserFreq = 0;
    [Range(0, 0.9999f)] public float phaserFeedback = 0;
    [Range(1, 32)] public int phaserStages = 4;
    public bool phaserPositiveFeedback = true;



    private int numActiveVoices = 0;
    public SynthVoice[] voices;

    // possible Effects
    private Biquad filter = new Biquad();
    private Chorus chorus = new Chorus(48000, 5000, 8);
    private Haas haas = new Haas(48000);
    private Panner panner = new Panner();
    private RingModulation ringMod = new RingModulation(48000);
    private Granular granular = new Granular(48000, 48000*5, 20);


    private LFO lfo1 = new LFO(48000);
    


    public override void StartNote(int noteID, int startOffset)
    {
        PlayNextAvailableVoice(noteID, startOffset);
    }

    public override void StopNote(int noteID, int stopOffset)
    {
        StopVoice(noteID, stopOffset);
    }

    public override void StopAllNotes()
    {
        StopAllVoices();
    }

    public void PlayNextAvailableVoice(int noteID, int startOffset)
    {
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay() && voices[i].noteID==-100) { voices[i].StartNote(noteID+octave*12, 1, startOffset); numActiveVoices++;  return; }
        }
    }

    public void StopVoice(int noteID, int stopOffset)
    {
        float oldest = 0;
        int oldestID = 0;
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].noteID == noteID && voices[i].IsPlaying())
            {
                voices[i].StopNote(noteID + octave * 12, 1);
                numActiveVoices--;
                if (voices[i].time > oldest)
                {
                    oldest = voices[i].time;
                    oldestID = i;
                }
                
            }
        
        }

        if (voices[oldestID].noteID == noteID && voices[oldestID].IsPlaying())
        {
           // voices[oldestID].StopNote(noteID + octave * 12, 1);
            //numActiveVoices--;
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



        System.Random r = new System.Random();  

        // try sample accurate parameter automation
        for(int i=0; i<data.Length; i += 2)
        {
            // get lfo value
            //float val = lfo1.GetValue();

            for(int j=0; j<voices.Length; j++)
            {
                //voices[j].freq.valueBuffer[i] = val*10;
            }
        }

        // Get samples from generators per voice
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) continue;

            voices[i].RenderBlock(data, numChannels, numActiveVoices);
        }

        

        // Process effects
        for (int i=0; i<data.Length; i+=2)
        {

        }

        granular.ProcessBlock(data, numChannels);
        chorus.ProcessBlock(data, numChannels);

    }
}
