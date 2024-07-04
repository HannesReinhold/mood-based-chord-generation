using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChordGenerator
{
    public int octave = 0;
    public Key key = 0;
    public Scale scaleMode = 0;
    public Chord chordNum = 0;

    public bool has1st = false;
    public bool has3rd = false;
    public bool has5th = false;
    public bool has7th = false;
    public bool has9th = false;
    public bool has11th = false;

    public MidiDevice device;

    private int[,] scales =
    {
        { 0, 2, 4, 5, 7, 9, 11  },
        { 0, 2, 3, 5, 7, 8, 10  },
        { 0, 2, 4, 5, 7, 8, 11  },
        { 0, 2, 3, 5, 7, 8, 11  },
        { 0, 2, 3, 5, 7, 9, 11  },
        { 0, 3, 5, 6, 7, 10, 0  },
        { 0, 1, 4, 5, 7, 8, 11  },
        { 0, 1, 3, 5, 7, 8, 10  }
    };


    public ChordGenerator(MidiDevice device)
    {
        this.device = device;
    }

    public void Update(MidiSignal midi)
    {
        switch (midi.midiEvent)
        {
            case MidiEvent.NoteOff:
                StopChord(BuildChord(key, scaleMode, chordNum, has7th, has9th, has11th));
                break;
            case MidiEvent.NoteOn:
                StartChord(BuildChord(key, scaleMode, chordNum, has7th, has9th, has11th));
                break;
            default:
                break;
        }
    }

    public List<int> BuildChord(Key key, Scale scale, Chord chordNum, bool has7th, bool has9th, bool has11th)
    {
        List<int> noteList = new List<int>();

        int chordID = (int)chordNum;
        int keyID = (int)key;

        if (has1st) noteList.Add((scales[(int)scaleMode, (chordID) % 7]) + (octave) * 12 + keyID);
        if (has3rd) noteList.Add((scales[(int)scaleMode, (chordID + 2) % 7]) + (octave + Mathf.FloorToInt((chordID + 2)) / 7) * 12 + keyID);
        if (has5th) noteList.Add((scales[(int)scaleMode, (chordID + 4) % 7]) + (octave + Mathf.FloorToInt((chordID + 4)) / 7) * 12 + keyID);

        if(has7th) noteList.Add((scales[(int)scaleMode, (chordID + 6) % 7]) + (octave + Mathf.FloorToInt((chordID + 6)) / 7) * 12 + keyID);
        if (has9th) noteList.Add((scales[(int)scaleMode, (chordID + 8) % 7]) + (octave + Mathf.FloorToInt((chordID + 8)) / 7) * 12 + keyID);
        if (has11th) noteList.Add((scales[(int)scaleMode, (chordID + 10) % 7]) + (octave + Mathf.FloorToInt((chordID + 10)) / 7) * 12 + keyID);

        return noteList;
    }

    private void StartChord(List<int> notes)
    {
        for(int i=0; i<notes.Count; i++)
        {
            device.StartNote(notes[i]);
        }
    }

    private void StopChord(List<int> notes)
    {
        device.StopAllNotes();

        for (int i = 0; i < notes.Count; i++)
        {
            device.StopNote(notes[i]);
        }
    }

}


[System.Serializable]
public enum Mode
{
    Ionian = 0,
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,
    Locrian
}

[System.Serializable]
public enum Scale
{
    Major,
    Minor,
    Harmonic_Major,
    Harmonic_Minor,
    Melodic_Minor,
    Blues,
    Double_Harmonic_Major,
    Phrygian
}

[System.Serializable]
public enum Key
{
    C = 0,
    Cs,
    D,
    Ds,
    E,
    F,
    Fs,
    G,
    Gs,
    A,
    As,
    B
}

[System.Serializable]
public enum Chord
{
    I = 0,
    II,
    III,
    IV,
    V,
    VI,
    VII
}