using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiGeneratorTest : MonoBehaviour
{
    public Synthesizer synth;
    public Arpeggiator arp;

    public bool playNote = false;
    [Range(0,6)] public int octave = 2;
    public Key key = 0;
    public Scale scaleMode = 0;
    public Chord chordNum=0;

    public bool has1st = false;
    public bool has3rd = false;
    public bool has5th = false;
    public bool has7th = false;
    public bool has9th = false;
    public bool has11th = false;

    private IEnumerator coroutine;


    private ChordGenerator generator;

    private void Awake()
    {
        generator = new ChordGenerator(arp);
    }

    private void OnValidate()
    {
        //StopNote(0);
        //PlayNote();

        generator.octave = octave;
        generator.key = key;
        generator.scaleMode = scaleMode;
        generator.chordNum = chordNum;
        generator.has1st = has1st;
        generator.has3rd = has3rd;
        generator.has5th = has5th;
        generator.has7th = has7th;
        generator.has9th = has9th;
        generator.has11th = has11th;

        generator.Update(new MidiSignal(MidiEvent.NoteOff, 0));
        generator.Update(new MidiSignal(playNote ? MidiEvent.NoteOn : MidiEvent.NoteOff, 0));

        
    }


}
