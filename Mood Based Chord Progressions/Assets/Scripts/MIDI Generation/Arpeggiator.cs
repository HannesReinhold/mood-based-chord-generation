using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Arpeggiator : MonoBehaviour, MidiDevice
{
    public MidiDevice device;


    public float bpm;
    public float rate;

    private float timer = 0;

    private List<int> notes = new List<int>();

    int noteIndex=0;

    public bool canPlay = false;

    private bool up = true;

    public ArpeggiatorMode mode = ArpeggiatorMode.Ascending;

    private System.Random random = new System.Random();

    private void Start()
    {
    }

    private void OnDisable()
    {
       canPlay = false;
    }


    private void OnAudioFilterRead(float[] data, int channels)
    {

        for(int i=0; i<data.Length; i+=2)
        {
            timer += 1f / 48000f;
            if(timer> 60f / bpm * rate)
            {
                timer = 0;
                device.StopAllNotes();

                if(mode == ArpeggiatorMode.Ascending)
                {
                    noteIndex++;
                    if (noteIndex >= notes.Count)
                    {
                        noteIndex = 0;
                    }
                }
                else if(mode == ArpeggiatorMode.Descending)
                {
                    noteIndex--;
                    if (noteIndex < 0) noteIndex = notes.Count - 1;
                }
                else if(mode == ArpeggiatorMode.UpDown)
                {
                    noteIndex += up ? 1 : -1;
                    if (noteIndex >= notes.Count-1) up = false;
                    if (noteIndex <= 0) up = true;
                    if (notes.Count == 1) noteIndex = 0;
                }
                else
                {
                    noteIndex = random.Next(0, notes.Count - 1);
                }


                
                if (noteIndex>=notes.Count) continue;

                device.StartNote(notes[noteIndex]);
            }
        }
    }

    public void StartNote(int noteID)
    {
        if (notes.Contains(noteID)) return;
        notes.Sort();
        notes.Add(noteID);
    }

    public void StopNote(int noteID)
    {
        notes.Remove(noteID);
        notes.Sort();
        device.StopNote(noteID);
    }

    public void StopAllNotes()
    {
        notes.Clear();
    }
}

public enum ArpeggiatorMode
{
    Ascending,
    Descending,
    UpDown,
    Random
}
