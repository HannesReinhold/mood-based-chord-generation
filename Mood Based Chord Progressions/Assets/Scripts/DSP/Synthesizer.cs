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

    public float delayInMs = 0;
    public float delayFeedback = 0;

    public int band;

    public float attack;
    public float release;
    public float upperThreshold;
    public float lowerThreshold;
    public float ratio;


    private int numActiveVoices = 0;
    public SynthVoice[] voices;

    // Effects
    private HardClip dist = new HardClip();
    private Phaser phaser = new Phaser();
    private FeedbackDelay delay = new FeedbackDelay(48000, 48000);
    private BandSplitter bandSplitter = new BandSplitter(3, new float[] {88 , 2000});
    private Compressor comp1 = new Compressor(64);
    private Compressor comp2 = new Compressor(64);
    private Compressor comp3 = new Compressor(64);
    private Biquad filter = new Biquad();

    private Chorus chorus = new Chorus(48000, 5000, 8);
    private Haas haas = new Haas(48000);
    private Panner panner = new Panner();

    private FirFilter firFilter = new FirFilter(257);
    private RingModulation ringMod = new RingModulation(48000);

    private Granular granular = new Granular(48000, 48000*5, 20);


    private LFO lfo1 = new LFO(48000);
    public float[] lfoBuffer = new float[1024];
    


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
            if (!voices[i].CanPlay() && voices[i].noteID==-100) { Debug.Log("Play "+i); voices[i].StartNote(noteID+octave*12, 1); numActiveVoices++;  return; }
        }
        //Debug.Log(numActiveVoices);
    }

    public void StopVoice(int noteID)
    {
        float oldest = 0;
        int oldestID = 0;
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].noteID == noteID && voices[i].IsPlaying())
            {
                if (voices[i].time > oldest)
                {
                    oldest = voices[i].time;
                    oldestID = i;
                }
                
            }
        
        }

        if (voices[oldestID].noteID == noteID && voices[oldestID].IsPlaying())
        {
            voices[oldestID].StopNote(noteID + octave * 12, 1);
            numActiveVoices--;
            Debug.Log("Stop "+oldestID);
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
    float z0 = 0;
    float z1 = 0;
    float m = 1;
    bool t = false;
    public void ProcessBlock(float[] data, int numChannels)
    {
        dist.SetDrive(100);
        dist.SetDcOffset(dcOffset);

        phaser.fc = phaserFreq;
        phaser.feedback = phaserFeedback;
        phaser.numStages = phaserStages;
        phaser.positiveFeedback = phaserPositiveFeedback;

        delay.SetDelayInSamples(128);
        delay.feedback = 0;
        delay.positiveFeedback = phaserPositiveFeedback;

        comp1.SetAttack(1);
        comp1.SetRelease(100);
        comp1.upperThreshold = -8.5f;
        comp1.lowerThreshold = -10.0f;
        comp1.downwardsRatio = 16;
        comp1.upwardsRatio = 16;

        comp2.SetAttack(1);
        comp2.SetRelease(100);
        comp2.upperThreshold = -8.0f;
        comp2.lowerThreshold = -10.2f;
        comp2.downwardsRatio = 16;
        comp2.upwardsRatio = 16;

        comp3.SetAttack(1);
        comp3.SetRelease(50);
        comp3.upperThreshold = -6.5f;
        comp3.lowerThreshold = -8.0f;
        comp2.downwardsRatio = 16;
        comp2.upwardsRatio = 16;

        chorus.feedback = delayFeedback;

        filter.CalcCoeffs(phaserFreq*22000,Mathf.Max(0.1f,phaserFeedback),1,BiquadType.Lowpass);

        panner.pan = panning;

        firFilter.SetHilbert(phaserFreq*10);
        ringMod.SetFrequency(phaserFeedback * 1000);

        lfo1.SetFrequency(delayFeedback * 20);



        System.Random r = new System.Random();  

        for(int i=0; i<data.Length; i += 2)
        {
            float val = lfo1.GetValue();

            for(int j=0; j<voices.Length; j++)
            {
                voices[j].freq.valueBuffer[i] = val*10;
            }
        }

        for (int i=0; i<voices.Length; i++)
        {
            //voices[i].lowpass.SetCoeffs(cutoffFrequency, Q, 0);
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
            //data[i] = (float)(r.NextDouble()*2-1) * 0.1f;
            //data[i] = dist.ProcesSample(data[i]);
            /*
            data[i] = (float)r.NextDouble() * 2 - 1;
            if(z1> 48000)
            {
                z1 = 0;
                t = !t;
            }
            z1++;

            data[i] *= t ? 0.1f : 1;
            */
            //float[] bands = bandSplitter.Process(data[i]);
            //data[i] = comp1.Process(bands[0]*5)+ comp2.Process(bands[1]*5)*0.9f+ comp3.Process(bands[2]*10)*0.8f;
            //data[i] = bands[2];
            //data[i + 1] = bands[0] + bands[1] + bands[2];
            // data[i+1] = 0;
            //float a = delay.Process(data[i]);
            //data[i] = firFilter.Process(data[i])-a;
        }
        //granular.ProcessBlock(data, numChannels);
        //ringMod.ProcessBlock(data, numChannels);
        //chorus.ProcessBlock(data, numChannels);
        //haas.ProcessBlock(data, numChannels);
        //panner.ProcessBlock(data, numChannels);

    }
}
