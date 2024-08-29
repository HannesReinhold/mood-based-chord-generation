using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChainTest : MonoBehaviour
{
    private MarkovChain chain = new MarkovChain();

    public ChordGenerator generator;
    public Synthesizer synth;

    float timer = 0;

    int chord = 0;

    public bool playNote = false;
    [Range(0, 6)] public int octave = 4;
    public Key key = 0;
    public Scale scaleMode = 0;
    public Chord chordNum = 0;

    public bool has1st = false;
    public bool has3rd = false;
    public bool has5th = false;
    public bool has7th = false;
    public bool has9th = false;
    public bool has11th = false;

    private string[] chordNames = {"I", "II" , "III" , "IV" , "V" , "VI" , "VII" };


    // Start is called before the first frame update
    void Awake()
    {
        generator = new ChordGenerator(synth);
    }

    // Update is called once per frame
    void Update()
    {

        generator.octave = octave;
        generator.key = key;
        generator.scaleMode = scaleMode;
        generator.chordNum = chordNum;
        generator.has1st = has1st;
        generator.has3rd = has3rd;
        generator.has5th = has5th;
        generator.has7th = has7th;
        generator.has9th = has9th;
        generator.has11th = has11th;



        if (timer > 4)
        {
            timer = 0;
            PlayNextChord();
        }

        timer += Time.deltaTime;
    }

    public void PlayNextChord()
    {
        

        chord = chain.GetNextChord(chord);
        Debug.Log("Play Chord " + chordNames[chord] +" in " + key.ToString() + " " + scaleMode.ToString());
        generator.StopAllNotes();
        generator.StartNote(chord,0);
    }
}


public enum Mood
{
    Happy,
    Sad,

}