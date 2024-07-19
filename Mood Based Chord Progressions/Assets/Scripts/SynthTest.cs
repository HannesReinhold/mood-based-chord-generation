using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using MidiParser;

public class SynthTest : MonoBehaviour
{
    private KeyboardInput input = new KeyboardInput();
    private Synthesizer synth = new Synthesizer();
    private Synthesizer synth2 = new Synthesizer();
    private ChordGenerator chordGenerator = new ChordGenerator();
    private ChordGenerator chordGenerator2 = new ChordGenerator();
    private Arpeggiator arp = new Arpeggiator();
    private MidiPlayer midiPlayer = new MidiPlayer(48000);

    public LineRenderer oscilloscope;
    private float[] dataCopy;

    public MarkovChain chain = new MarkovChain();

    private float timer = 0;
    private float bpm = 140;
    private float rate = 8;

    private int chord = 0;


    public List<int> chords;
    int chordIndex = 0;

    [Range(0, 1)] public float panning = 0.5f;
    [Range(0,1)]public float phaserFreq = 0;
    [Range(0, 10)] public float phaserFeedback = 0;
    [Range(1, 32)] public int phaserStages = 4;
    public bool phaserPositiveFeedback = true;
    [Range(0, 20)] public float delayInMs = 0;
    [Range(0, 1)] public float delayFeedback = 0.5f;
    [Range(0, 2)] public int band=0;

    [Range(0, 100)] public float attack;
    [Range(0, 100)] public float release;
    [Range(-50, 10)] public float upperThreshold;
    [Range(-50, 10)] public float lowerThreshold;
    [Range(0, 10)] public float ratio;


    // Start is called before the first frame update
    void Start()
    {
        midiPlayer.device = synth;
        /*midiPlayer.midiFile = midiPlayer.midiFile = midiPlayer.SequenceToMidiFile(midiPlayer.noteNameSeqTonoteIDSeq(new List<string>() { "g6,0,200", "d#6,200,200", "c6,400,200", "d#6,600,200", "f6,800,200", "d#6,1000,200", "c6,1200,200", "d#6,1400,200" ,
                                                                                                                                         "g6,1600,200", "d#6,1800,200", "c6,2000,200", "d#6,2200,200", "f6,2400,200", "d#6,2600,200", "c6,2800,200", "d#6,3000,200",
                                                                                                                                         "g6,3200,200", "d#6,3400,200", "d6,3600,200", "d#6,3800,200", "a#6,4000,200", "d#6,4200,200", "d6,4400,200", "d#6,4600,200",
                                                                                                                                         "g6,4800,200", "d#6,5000,200", "d6,5200,200", "d#6,5400,200", "a#6,5600,200", "d#6,5800,200", "d6,6000,200", "d#6,6200,200",
                                                                                                                                         "d6,6400,200", "a#5,6600,200", "a5,6800,200", "a#5,7000,200", "f6,7200,200", "a#5,7400,200", "a5,7600,200", "a#5,7800,200",
                                                                                                                                         "d6,8000,200", "a#5,8200,200", "a5,8400,200", "a#5,8600,200", "f6,8800,200", "a#5,9000,200", "a5,9200,200", "a#5,9400,200",
                                                                                                                                         "d6,9600,200", "a#5,9800,200", "a5,10000,200", "a#5,10200,200", "f6,10400,200", "a#5,10600,200", "a5,10800,200", "a#5,11000,200",
                                                                                                                                         "c6,11200,200", "a#5,11400,200", "a5,11600,200", "a#5,11800,200", "c6,12000,200", "a#5,12200,200", "a5,12400,200", "a#5,12600,200",
                                                                                                                                         "c2,0,3200","d#2,3200,3200","a#2,6400,3200","f3,9600,3200","g3,0,3200","a#3,3200,3200","f3,6400,3200","c3,9600,3200",
                                                                                                                                         "g5,0,400","g5,400,400","g5,800,400","g5,1200,400","g5,1600,400","g5,2000,400","g5,2400,400","g5,2800,400",
                                                                                                                                         "d#5,0,400","d#5,400,400","d#5,800,400","d#5,1200,400","d#5,1600,400","d#5,2000,400","d#5,2400,400","d#5,2800,400",
                                                                                                                                         "g5,3200,400","g5,3600,400","g5,4000,400","g5,4400,400","g5,4800,400","g5,5200,400","g5,5600,400","g5,6000,400",
                                                                                                                                         "d#5,3200,400","d#5,3600,400","d#5,4000,400","d#5,4400,400","d#5,4800,400","d#5,5200,400","d#5,5600,400","d#5,6000,400",
                                                                                                                                         "f5,6400,400","f5,6800,400","f5,7200,400","f5,7600,400","f5,8000,400","f5,8400,400","f5,8800,400","f5,9200,400",
                                                                                                                                         "d5,6400,400","d5,6800,400","d5,7200,400","d5,7600,400","d5,8000,400","d5,8400,400","d5,8800,400","d5,9200,400",
                                                                                                                                         "f5,9600,400","f5,10000,400","f5,10400,400","f5,10800,400","f5,11200,400","f5,11600,400","f5,12000,400","f5,12400,400",
                                                                                                                                         "c5,9600,400","c5,10000,400","c5,10400,400","c5,10800,400","c5,11200,400","c5,11600,400","c5,12000,400","c5,12400,400"}));
        */
        //List<MidiSignal> midiFile = MidiParser.ParseMidi(File.ReadAllBytes(Application.dataPath + "/Ressources/MidiTest.mid"));
        MidiFile file = new MidiFile(File.ReadAllBytes(Application.dataPath + "/Ressources/Never-Gonna-Give-You-Up-3.mid"));
        List<MidiSignal> midiFile = new List<MidiSignal>();
        for (int i = 0; i < file.Tracks[0].MidiEvents.Count; i++)
        {
            MidiParser.MidiEvent ev = file.Tracks[0].MidiEvents[i];
            MidiEvent midiEvent;
            if (ev.MidiEventType == MidiEventType.NoteOff) midiEvent = MidiEvent.NoteOff;
            else if (ev.MidiEventType == MidiEventType.NoteOn) midiEvent = MidiEvent.NoteOn;
            else continue;
            
            midiFile.Add(new MidiSignal(midiEvent, ev.Note, (double)ev.DeltaTime, (double)ev.Time));
        }

        midiPlayer.midiFile = midiFile;

       // midiPlayer.midiFile = midiFile;

        //input.device = arp;
        chordGenerator.device = arp;
        chordGenerator.octave = 4;
        chordGenerator.has3rd = true;
        chordGenerator.has5th = true;

        chordGenerator2.device = synth2;
        chordGenerator2.octave = 5;
        //chordGenerator2.has9th = true;

        chordGenerator.scaleMode = Scale.Minor;
        chordGenerator2.scaleMode = Scale.Minor;

        synth.PrepareToPlay();
        synth2.PrepareToPlay();

        synth.attack = attack;
        synth2.release = release;
        synth2.lowerThreshold = lowerThreshold;
        synth2.upperThreshold = upperThreshold;
        synth2.ratio = ratio;

        for(int i=0; i<synth2.maxVoices; i++)
        {
            synth2.voices[i].oscillator1.detune = 30;
        }

        arp.bpm = 140;
        arp.rate = 1 / 4f;

        timer = 60f / bpm * rate;


        for(int i=0; i<midiPlayer.midiFile.Count; i++)
        {
            Debug.Log((midiPlayer.midiFile[i].midiEvent == MidiEvent.NoteOn ? "Play " : "Stop ") + "note " + midiPlayer.midiFile[i].noteIndex + " at time " + midiPlayer.midiFile[i].absoluteTime+", "+midiPlayer.midiFile[i].timeToNextEvent);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        input.Update();


        oscilloscope.positionCount = dataCopy.Length / 2;
        for (int i = 0; i < dataCopy.Length; i += 2)
        {
            //oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), dataCopy[i] * 0.25f, 0));
        }

        float[] coeffs = BiquadCalculator.CalcCoeffs(phaserFreq*24000, phaserFeedback, 0, BiquadType.Lowpass, 48000);
        for (int i = 0; i < 1024; i += 1)
        {
            float fLog = Mathf.Lerp(Mathf.Log10(20)*10, Mathf.Log10(24000)*10, (float)i / 1024);
            float f = MathUtils.DbToLin(fLog);
            oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), BiquadCalculator.GetFrequencyResponse(f,coeffs, 48000)*2-1, 0));
        }

        synth.phaserFreq = phaserFreq;
        synth.phaserFeedback = phaserFeedback;
        synth.phaserStages = phaserStages;
        synth.phaserPositiveFeedback = phaserPositiveFeedback;

        synth.delayInMs = delayInMs;
        synth.delayFeedback = delayFeedback;

        synth.band = band;

        synth.attack = attack;
        synth.release = release;
        synth.lowerThreshold = lowerThreshold;
        synth.upperThreshold = upperThreshold;
        synth.ratio = ratio;
        synth.panning = panning;
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
                //chordGenerator.StopAllNotes();
                //chordGenerator.StartNote(chord);

                //chordGenerator2.StopAllNotes();
                //chordGenerator2.StartNote(chord);
            }

            //arp.UpdateArp();
            midiPlayer.Update();
        }


        
        synth.ProcessBlock(data, channels);
        //synth2.ProcessBlock(data,channels);

        dataCopy = data;
    }
}
