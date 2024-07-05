using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MidiDevice
{

    public MidiDevice device;

    public abstract void StartNote(int noteID);
    public abstract void StopNote(int noteID);

    public abstract void StopAllNotes();
}
