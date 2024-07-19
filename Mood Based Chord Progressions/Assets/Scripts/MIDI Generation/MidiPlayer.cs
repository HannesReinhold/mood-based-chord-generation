using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiPlayer
{
    public MidiDevice device;
    public float bpm = 140;
    public bool loop = true;

    private double sampleRate;
    private double timer = 0;
    private double deltaTimer =0;
    private double timerInc = 0;
    private double timeToNextEvent = 0;

    public List<MidiSignal> midiFile;
    private int midiIndex=0;

    private bool isPlaying = false;

    


    public MidiPlayer(float sampleRate)
    {
        this.sampleRate = (double)sampleRate;
        //timerInc = ((double)bpm / 60.0) / sampleRate;
        timerInc = 1000 / sampleRate;
        timeToNextEvent = 0;
        isPlaying = true;
    }

    public void Play()
    {
        midiIndex = 0;
        timer = 0;
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
        midiIndex = 0;
        timer = 0;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Update()
    {
        if (!isPlaying) return;
        if(midiIndex >= midiFile.Count)
        {
            if (loop) Play();
            else Stop();
        }

        if (deltaTimer >= timeToNextEvent)
        {

            timeToNextEvent = midiFile[midiIndex].timeToNextEvent;
            if (midiFile[midiIndex].midiEvent == MidiEvent.NoteOn)
            {
                device.StartNote(midiFile[midiIndex].noteIndex);
                Debug.Log("Play");
            }
            else
                device.StopNote(midiFile[midiIndex].noteIndex);

            midiIndex++;

            while (timeToNextEvent == 0 && midiIndex<midiFile.Count)
            {
                timeToNextEvent = midiFile[midiIndex].timeToNextEvent;
                if (midiFile[midiIndex].midiEvent == MidiEvent.NoteOn)
                    device.StartNote(midiFile[midiIndex].noteIndex);
                else
                    device.StopNote(midiFile[midiIndex].noteIndex);

                midiIndex++;
            }

            deltaTimer = 0;
        }


        timer += timerInc;
        deltaTimer += timerInc;
    }

    public void SetMidiFile(MidiParser.MidiFile file, int index)
    {
        List<MidiSignal> parsedMidi = new List<MidiSignal>();
        int lastTime = 0;
        for(int j=0; j<file.Tracks[0].MidiEvents.Count; j++)
        {
            

            MidiParser.MidiEvent ev = file.Tracks[0].MidiEvents[j];
            if (!(ev.Channel==3|| ev.Channel == 4|| ev.Channel == 16)) continue;
            int dt = ev.Time - lastTime;
            lastTime = ev.Time;
            MidiEvent midiEvent;
            if (ev.MidiEventType == MidiParser.MidiEventType.NoteOff) midiEvent = MidiEvent.NoteOff;
            else if (ev.MidiEventType == MidiParser.MidiEventType.NoteOn) midiEvent = MidiEvent.NoteOn;
            else continue;

            parsedMidi.Add(new MidiSignal(midiEvent, ev.Note, (double)dt*1.5, (double)ev.Time*1.5, ev.Channel));
        }

        midiFile = parsedMidi;
    }

    public List<Vector3> noteNameSeqTonoteIDSeq(List<string> seq)
    {
        List<Vector3> notes = new List<Vector3>();

        for(int i=0; i<seq.Count; i++)
        {
            string[] seperateValues = seq[i].Split(",");
            notes.Add(new Vector3((float)MathUtils.noteIDFromName(seperateValues[0]), float.Parse(seperateValues[1]), float.Parse(seperateValues[2])));
        }

        return notes;
    }

    public List<MidiSignal> SequenceToMidiFile(List<Vector3> notes)
    {
        double lastNote = 0;
        List<MidiSignal> midi = new List<MidiSignal>();
        for(int i=0; i<notes.Count; i++)
        {
            double deltaTime = 0;
            if (i < notes.Count - 1) deltaTime = (notes[i + 1].y - notes[i].y);
            else deltaTime = notes[i].z;
            midi.Add(new MidiSignal(MidiEvent.NoteOn, (int)notes[i].x, (double)deltaTime, (double)notes[i].y));
            midi.Add(new MidiSignal(MidiEvent.NoteOff, (int)notes[i].x, (double)notes[i].z, (double)notes[i].z+(double)notes[i].y));

        }

        midi.Sort((a, b) => a.absoluteTime.CompareTo(b.absoluteTime));

        for (int i = 0; i < midi.Count; i++)
        {
            double deltaTime = 0;
            if (i < midi.Count - 1) deltaTime = (midi[i + 1].absoluteTime - midi[i].absoluteTime);
            else deltaTime = 0;
            midi[i].timeToNextEvent = deltaTime;

        }

        return midi;
    }

}
