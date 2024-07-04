using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Arpeggiator : MonoBehaviour, MidiDevice
{
    public Synthesizer synthesizerRef;


    public float bpm;
    public float rate;

    private float timer = 0;

    private List<int> notes = new List<int>();

    int noteIndex=0;

    public bool canPlay = false;

    private void Start()
    {
    }

    private void OnDisable()
    {
       canPlay = false;
    }


    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!canPlay) return;

        for(int i=0; i<data.Length; i+=2)
        {
            timer += 1f / 48000f;
            if(timer> 60f / bpm * rate)
            {
                timer = 0;
                synthesizerRef.StopAllVoices();


                noteIndex++;
                if (noteIndex >= notes.Count)
                {
                    noteIndex = 0;
                }

                synthesizerRef.PlayNextAvailableVoice(notes[noteIndex]);
            }
        }
    }

    public void StartNote(int noteID)
    {
        notes.Add(noteID);
    }

    public void StopNote(int noteID)
    {
        notes.Remove(noteID);
    }

    public void StopAllNotes()
    {
        notes.Clear();
    }
}
