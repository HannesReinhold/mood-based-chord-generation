using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiSignal
{
    public MidiEvent midiEvent;
    public int noteIndex;

    public MidiSignal(MidiEvent ev, int noteID){
        midiEvent = ev;
        noteIndex = noteID;
    }

}


public enum MidiEvent
{
    NoteOff,
    NoteOn
}