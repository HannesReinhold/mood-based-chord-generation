using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MidiDevice
{

    public abstract void StartNote(int noteID);
    public abstract void StopNote(int noteID);

    public abstract void StopAllNotes();
}
