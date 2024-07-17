using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiSignal
{
    public MidiEvent midiEvent;
    public int noteIndex;
    public double timeToNextEvent;
    public double absoluteTime;

    public MidiSignal(MidiEvent ev, int noteID){
        midiEvent = ev;
        noteIndex = noteID;
    }

    public MidiSignal(MidiEvent ev, int noteID, double timeToNext, double time)
    {
        midiEvent = ev;
        noteIndex = noteID;
        timeToNextEvent = timeToNext;
        absoluteTime = time;
    }
}


public enum MidiEvent
{
    NoteOff,
    NoteOn
}