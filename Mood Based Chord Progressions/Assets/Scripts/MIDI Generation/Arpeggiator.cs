using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Arpeggiator : MidiDevice
{


    public float bpm = 120;
    public float rate = 1;

    private float timer = 0;

    private List<int> notes = new List<int>();

    int noteIndex=0;

    public bool canPlay = false;

    private bool up = true;

    public ArpeggiatorMode mode = ArpeggiatorMode.Ascending;

    private System.Random random = new System.Random();

    public int[] noteLayout;

    private int lastNote = 0;

    private int offsetTimer = 0;
    

    private void Start()
    {
    }

    private void OnDisable()
    {
       canPlay = false;
    }

    public void UpdateArp()
    {
        timer += 1f / 48000f;
        offsetTimer++;
        if (offsetTimer >= 1024) offsetTimer = 0;
        if (timer > 60f / bpm * rate)
        {
            timer = 0;
            device.StopNote(lastNote,0);

            if (mode == ArpeggiatorMode.Ascending)
            {
                noteIndex++;
                if (noteIndex >= notes.Count)
                {
                    noteIndex = 0;
                }
            }
            else if (mode == ArpeggiatorMode.Descending)
            {
                noteIndex--;
                if (noteIndex < 0) noteIndex = notes.Count - 1;
            }
            else if (mode == ArpeggiatorMode.UpDown)
            {
                noteIndex += up ? 1 : -1;
                if (noteIndex >= notes.Count - 1) up = false;
                if (noteIndex <= 0) up = true;
                if (notes.Count == 1) noteIndex = 0;
            }
            else if (mode == ArpeggiatorMode.Random)
            {
                noteIndex = random.Next(0, Mathf.Max(0, notes.Count - 1));
            }
            else
            {
                noteIndex++;
                if (noteIndex >= noteLayout.Length)
                {
                    noteIndex = 0;
                }

            }

            if (noteIndex >= notes.Count && mode != ArpeggiatorMode.Layout) return;
            if (notes.Count == 0) return;
            if (mode == ArpeggiatorMode.Layout)
            {
                device.StartNote(notes[Mathf.Min(noteLayout[noteIndex], notes.Count - 1)],offsetTimer);
                lastNote = notes[Mathf.Min(noteLayout[noteIndex], notes.Count - 1)];
            }
            else
            {
                device.StartNote(notes[noteIndex],offsetTimer);
                lastNote = notes[noteIndex];
            }
        }

    }

    public override void StartNote(int noteID, int startOffset)
    {
        if (notes.Contains(noteID)) return;
        notes.Sort();
        notes.Add(noteID);
    }

    public override void StopNote(int noteID, int stopOffset)
    {
        notes.Remove(noteID);
        notes.Sort();
        device.StopNote(noteID,stopOffset);
    }

    public override void StopAllNotes()
    {
        notes.Clear();
    }
}

public enum ArpeggiatorMode
{
    Ascending,
    Descending,
    UpDown,
    Random,
    Layout
}
