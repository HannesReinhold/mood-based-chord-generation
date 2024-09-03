using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using MidiParser;

public class SynthTest : MonoBehaviour
{
    private Synthesizer bassSynth = new Synthesizer();
    private Synthesizer chordSynth = new Synthesizer();
    private Synthesizer leadSynth = new Synthesizer();


    private MidiPlayer midiPlayerBass = new MidiPlayer(48000);
    private MidiPlayer midiPlayerChord = new MidiPlayer(48000);
    private MidiPlayer midiPlayerLead = new MidiPlayer(48000);

    private Spatializer spatializer = new Spatializer(48000);

    public LineRenderer oscilloscope;
    private float[] dataCopy;

    private float timer = 0;
    private float bpm = 140;
    private float rate = 8;

    private int chord = 0;


    [Range(0, 100)] public float detune = 1;
    [Header("Filters")]
    [Range(20, 20000)] public float preDistFilterFreq = 1000;
    [Range(0.01f, 10)] public float preDistFilterQ = 1;
    [Range(20, 20000)] public float postDistFilterFreq = 1000;
    [Range(0.01f, 10)] public float postDistFilterQ = 1;
    [Header("Distortion")]
    [Range(0, 100)] public float distortionDrive = 1;
    [Range(-1, 1)] public float distortionDCOffset = 1;
    [Header("Phaser")]
    [Range(0, 32)] public int phaserNumStages = 8;
    [Range(0, 1)] public float phaserFreq = 0.5f;
    [Range(0, 1)] public float phaserFeedback = 0.5f;
    [Range(0, 1)] public float phaserLfoStrength = 0.1f;
    


    private Transform camTransform;


    // Start is called before the first frame update
    void Start()
    {
        midiPlayerBass.device = bassSynth;
        midiPlayerChord.device = chordSynth;
        midiPlayerLead.device = leadSynth;
        /*
        midiPlayer.midiFile = midiPlayer.midiFile = midiPlayer.SequenceToMidiFile(midiPlayer.noteNameSeqTonoteIDSeq(new List<string>() { "g6,0,200", "d#6,200,200", "c6,400,200", "d#6,600,200", "f6,800,200", "d#6,1000,200", "c6,1200,200", "d#6,1400,200" ,
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

        //MidiFile file = new MidiFile(File.ReadAllBytes(Application.dataPath + "/Ressources/Never-Gonna-Give-You-Up-3.mid"));
        //midiPlayer.SetMidiFile(file, 1);
        midiPlayerBass.midiFile = midiPlayerBass.SequenceToMidiFile(midiPlayerBass.noteNameSeqTonoteIDSeq(new List<string>() { "f2,0,64000", "f2,64000,64000"}));
        midiPlayerChord.midiFile = midiPlayerChord.SequenceToMidiFile(midiPlayerChord.noteNameSeqTonoteIDSeq(new List<string>() { "c4,0,1600", "d#4,0,1600", "g#4,0,1600", "a#3,1600,1600", "d#4,1600,1600", "g#4,1600,1600"}));
        midiPlayerLead.midiFile = midiPlayerLead.SequenceToMidiFile(midiPlayerLead.noteNameSeqTonoteIDSeq(new List<string>() { "g#5,0,400", "g#5,400,400"}));


        bassSynth.PrepareToPlay();
        chordSynth.PrepareToPlay();
        leadSynth.PrepareToPlay();

        bassSynth.SetVoiceFilterFrequency(0.3f);
        bassSynth.SetVoiceVoiceCount(2);
        bassSynth.SetNoiseVolume(0.00f);
        bassSynth.SetDetune(10);
        bassSynth.SetADSR(0.01f,1,1,0.01f);

        bassSynth.SetPreDistFilter(1000,0.8f, 0, BiquadType.Highpass);
        bassSynth.SetPostDistFilter(3000, 0.8f, 0, BiquadType.Lowpass);
        bassSynth.softClip.SetDrive(100);

    }

    // Update is called once per frame
    void Update()
    {

        bassSynth.SetDetune(detune);
        bassSynth.SetADSR(0.01f, 1, 1, 0.01f);

        bassSynth.SetPreDistFilter(preDistFilterFreq, preDistFilterQ, 0, BiquadType.Lowpass);
        bassSynth.SetPostDistFilter(postDistFilterFreq, postDistFilterQ, 0, BiquadType.Lowpass);
        bassSynth.softClip.SetDrive(distortionDrive);
        bassSynth.softClip.SetDcOffset(distortionDCOffset);
        bassSynth.SetPhaser(phaserNumStages, phaserFreq, phaserFeedback, phaserLfoStrength);


        // update spatializer
        spatializer.SetDirection(Camera.main.transform.InverseTransformPoint(transform.position).normalized, Vector3.Distance(Camera.main.transform.position, transform.position));


        // draw oscilloscope
        oscilloscope.positionCount = dataCopy.Length / 2;
        for (int i = 0; i < dataCopy.Length; i += 2)
        {
            oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), dataCopy[i] * 0.25f, 0));
        }

        /*
        float[] coeffs = BiquadCalculator.CalcCoeffs(phaserFreq*24000, phaserFeedback, 0, BiquadType.Lowpass, 48000);
        for (int i = 0; i < 1024; i += 1)
        {
            float fLog = Mathf.Lerp(Mathf.Log10(20)*10, Mathf.Log10(24000)*10, (float)i / 1024);
            float f = MathUtils.DbToLin(fLog);
            oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), BiquadCalculator.GetFrequencyResponse(f,coeffs, 48000)*2-1, 0));
        }
        */
        
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            midiPlayerBass.Update();
            //midiPlayerChord.Update();
            //midiPlayerLead.Update();
        }


        
        bassSynth.ProcessBlock(data, channels);
        //chordSynth.ProcessBlock(data, channels);
        //leadSynth.ProcessBlock(data, channels);
        //synth2.ProcessBlock(data,channels);

        //spatializer.ProcessBlock(data, channels);

        dataCopy = data;

        //Debug.Log(data.Length);
    }
}
