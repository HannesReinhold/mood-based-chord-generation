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

    private void Start()
    {
    }

    private void OnDisable()
    {
       canPlay = false;
    }

    public void UpdateArp()
    {
        if (device == null) return;

        timer += 1f / 48000f;
        if (timer > 60f / bpm * rate)
        {
            timer = 0;
            device.StopAllNotes();

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
                device.StartNote(notes[Mathf.Min(noteLayout[noteIndex], notes.Count - 1)]);
            else
                device.StartNote(notes[noteIndex]);
        }

    }

    public override void StartNote(int noteID)
    {
        if (notes.Contains(noteID)) return;
        notes.Sort();
        notes.Add(noteID);
    }

    public override void StopNote(int noteID)
    {
        if (device == null) return;
        notes.Remove(noteID);
        notes.Sort();
        device.StopNote(noteID);
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
