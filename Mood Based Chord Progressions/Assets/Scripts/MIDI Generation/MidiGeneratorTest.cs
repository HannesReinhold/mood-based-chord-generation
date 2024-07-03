using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiGeneratorTest : MonoBehaviour
{
    public Synthesizer synth;

    public float bpm;
    private float timer=0;

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

    [Range(0,6)] public int octave = 2;
    public Key key = 0;
    public Scale scaleMode = 0;
    public Chord chordNum=0;

    private IEnumerator coroutine;

    private void Update()
    {
        float bps = bpm / 60f;


        if (timer > 1f/bps)
        {
            timer = 0;
            //PlayNote();
        }
        timer += Time.deltaTime;
    }

    private void OnValidate()
    {
        StopNote(0);
        PlayNote();
    }

    public void PlayNote()
    {
        int chordID = (int)chordNum;
        int keyID = (int)key;

        synth.PlayNextAvailableVoice((scales[(int)scaleMode, chordID % 7]) + octave * 12 + keyID);
        //synth.PlayNextAvailableVoice((scales[(int)scaleMode, (chordID + 2)%7]) + (octave+ Mathf.FloorToInt((chordID + 2))/7) * 12 + keyID);
        synth.PlayNextAvailableVoice((scales[(int)scaleMode, (chordID + 4)%7]) + (octave + Mathf.FloorToInt((chordID + 4)) / 7) * 12 + keyID);


        //Invoke("StopNote",(1f/(bpm/60f))*0.5f);
        coroutine = DelayStop(0);
        //StartCoroutine(coroutine);
    }

    IEnumerator DelayStop(int note)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds((1f / (bpm / 60f)) * 0.8f);
        StopNote(note);
    }

    public void StopNote(int note)
    {
        synth.StopAllVoices();
        //noteIndex++;
        //noteIndex %= 7;
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
        I=0,
        II,
        III,
        IV,
        V,
        VI,
        VII
    }
}
