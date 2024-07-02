using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiGeneratorTest : MonoBehaviour
{
    public Synthesizer synth;

    public float bpm;
    private float timer=0;

    private int[] majorScale = { 0, 2, 4, 5, 7, 9, 11 };
    private int[] minorScale = { 0, 2, 3, 5, 7, 8, 10 };

    private int[,] scaleModes =
    {
        { 0, 2, 4, 5, 7, 9, 11  },
        { 0, 2, 3, 5, 7, 8, 11  },
        { 0, 1, 3, 5, 7, 8, 10  },
        { 0, 2, 4, 6, 7, 9, 11  },
        { 0, 2, 4, 5, 7, 9, 10  },
        { 0, 2, 3, 5, 7, 8, 10  },
        { 0, 1, 3, 5, 6, 8, 10  }
    };

    public Mode scaleMode = 0;
    public int noteIndex = 0;
    public int octave = 24;
    public int key = 0;

    private int lastKey;

    private IEnumerator coroutine;

    private void Update()
    {
        float bps = bpm / 60f;


        if (timer > 1f/bps)
        {
            timer = 0;
            PlayNote();
        }
        timer += Time.deltaTime;
    }

    public void PlayNote()
    {

        int chosenKey = (scaleModes[(int)scaleMode, noteIndex]) + octave * 12 + key;
        synth.PlayNextAvailableVoice(chosenKey);


        //Invoke("StopNote",(1f/(bpm/60f))*0.5f);
        coroutine = DelayStop(chosenKey);
        StartCoroutine(coroutine);
    }

    IEnumerator DelayStop(int note)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds((1f / (bpm / 60f)) * 0.4f);
        StopNote(note);
    }

    public void StopNote(int note)
    {
        synth.StopAllVoices();
        noteIndex++;
        noteIndex %= 7;
    }

    [System.Serializable]
    public enum Mode
    {
        Ionian=0,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian,
        Locrian
    }
}
