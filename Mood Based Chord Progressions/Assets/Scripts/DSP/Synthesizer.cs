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


    public Biquad preDistFilterLeft = new Biquad();
    public Biquad preDistFilterRight = new Biquad();
    public HardClip softClip = new HardClip();
    public Biquad postDistFilterLeft = new Biquad();
    public Biquad postDistFilterRight = new Biquad();
    public Chorus chorus = new Chorus(48000, 5000, 8);
    public Haas haas = new Haas(48000);
    public Panner panner = new Panner();
    public Compressor compLeft = new Compressor(100);
    public Compressor compRight = new Compressor(100);

    public Phaser phaserLeft = new Phaser();
    public Phaser phaserRight = new Phaser();




    private LFO lfo1 = new LFO(48000);

    public void SetVoiceFilterFrequency(float f)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i].filterFreq = f;
        }
    }

    public void SetVoiceVoiceCount(int n)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i].oscillator1.numVoices = n;
        }
    }
    public void SetDetune(float d)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i].oscillator1.SetDetune(d);
        }
    }

    public void SetNoiseVolume(float v)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i].noiseVolume = v;
        }
    }

    public void SetADSR(float a, float d, float s, float r)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i].adsr.Set(a, d, s, r);
            voices[i].adsrLowpass.Set(a, d, s, r);
        }
    }

    public void SetPreDistFilter(float f, float q, float gain, BiquadType type)
    {
        preDistFilterLeft.CalcCoeffs(f,q,gain, type);
        preDistFilterRight.CalcCoeffs(f, q, gain, type);
    }

    public void SetPostDistFilter(float f, float q, float gain, BiquadType type)
    {
        postDistFilterLeft.CalcCoeffs(f, q, gain, type);
        postDistFilterRight.CalcCoeffs(f, q, gain, type);
    }

    public void SetCompressor(float att, float rel, float ratDown, float ratUp, float thrDown, float thrUp)
    {
        //compLeft.Set

    }

    public void SetPhaser(int n, float f, float feed, float lfoSt)
    {
        phaserLeft.numStages = n;
        phaserLeft.fc = f;
        phaserLeft.feedback = feed;
        phaserLeft.lfoStrength = lfoSt;

        phaserRight.numStages = n;
        phaserRight.fc = f;
        phaserRight.feedback = feed;
        phaserRight.lfoStrength = lfoSt;
    }



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
            data[i] = preDistFilterLeft.Process(data[i]);
            data[i+1] = preDistFilterRight.Process(data[i+1]);

            data[i] = softClip.ProcesSample(data[i]);
            data[i+1] = softClip.ProcesSample(data[i + 1]);

            data[i] = postDistFilterLeft.Process(data[i]);
            data[i+1] = postDistFilterRight.Process(data[i + 1]);

            data[i] = compLeft.Process(data[i]);
            data[i + 1] = compLeft.Process(data[i + 1]);

            data[i] = phaserLeft.Process(data[i]);
            data[i+1] = phaserLeft.Process(data[i+1]);
        }
        





        //granular.ProcessBlock(data, numChannels);
        
        chorus.ProcessBlock(data, numChannels);

    }
}
