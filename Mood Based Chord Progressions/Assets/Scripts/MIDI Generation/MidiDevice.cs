using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MidiDevice
{
    public void StartNote(int noteID);
    public void StopNote(int noteID);

    public void StopAllNotes();
}
