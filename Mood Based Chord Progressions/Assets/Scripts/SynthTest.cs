using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthTest : MonoBehaviour
{
    private KeyboardInput input = new KeyboardInput();
    private Synthesizer synth = new Synthesizer();
    private Synthesizer synth2 = new Synthesizer();
    private ChordGenerator chordGenerator = new ChordGenerator();
    private ChordGenerator chordGenerator2 = new ChordGenerator();
    private Arpeggiator arp = new Arpeggiator();

    public LineRenderer oscilloscope;
    private float[] dataCopy;

    public MarkovChain chain = new MarkovChain();

    private float timer = 0;
    private float bpm = 140;
    private float rate = 8;

    private int chord = 0;


    public List<int> chords;
    int chordIndex = 0;

    [Range(0,1)]public float phaserFreq = 0;
    [Range(0, 0.9999f)] public float phaserFeedback = 0;
    [Range(1, 32)] public int phaserStages = 4;
    public bool phaserPositiveFeedback = true;
    [Range(0, 20)] public float delayInMs = 0;
    [Range(0, 1)] public float delayFeedback = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        arp.device = synth;
        //input.device = arp;
        chordGenerator.device = arp;
        chordGenerator.octave = 2;
        chordGenerator.has3rd = false;
        chordGenerator.has5th = false;

        chordGenerator2.device = synth2;
        chordGenerator2.octave = 5;
        //chordGenerator2.has9th = true;

        chordGenerator.scaleMode = Scale.Minor;
        chordGenerator2.scaleMode = Scale.Minor;

        synth.PrepareToPlay();
        synth2.PrepareToPlay();

        for(int i=0; i<synth2.maxVoices; i++)
        {
            synth2.voices[i].oscillator1.detune = 30;
        }

        arp.bpm = 140;
        arp.rate = 2 / 1f;

        timer = 60f / bpm * rate;
    }

    // Update is called once per frame
    void Update()
    {
        //input.Update();


        oscilloscope.positionCount = dataCopy.Length / 2;
        for (int i = 0; i < dataCopy.Length; i += 2)
        {
            oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), dataCopy[i] * 0.25f, 0));
        }

        synth.phaserFreq = phaserFreq;
        synth.phaserFeedback = phaserFeedback;
        synth.phaserStages = phaserStages;
        synth.phaserPositiveFeedback = phaserPositiveFeedback;

        synth.delayInMs = delayInMs;
        synth.delayFeedback = delayFeedback;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            timer+= 1f / 48000f;
            if(timer > 60f / bpm * rate)
            {
                timer = 0;
                chord = chords[chordIndex];
                chordIndex++;
                if (chordIndex >= chords.Count) chordIndex = 0;
                chordGenerator.StopAllNotes();
                chordGenerator.StartNote(chord);

                chordGenerator2.StopAllNotes();
                chordGenerator2.StartNote(chord);
            }

            arp.UpdateArp();
        }


        
        synth.ProcessBlock(data, channels);
        //synth2.ProcessBlock(data,channels);

        dataCopy = data;
    }
}
