using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour, MidiDevice
{

    public int numChannels = 2;
    public int maxVoices = 16;

    [Range(0,1)] public float masterGain = 1;
    [Header("Distortion")]
    [Range(0, 100)] public float drive = 1;
    [Range(-1, 1)] public float dcOffset;
    [Header("Filter")]
    [Range(20, 22000)] public float cutoffFrequency = 22000;
    [Range(0, 10)] public float Q = 1;


    private int numActiveVoices = 0;


    private SynthVoice[] voices;


    public LineRenderer lineRenderer;
    private float[] dataCopy;



    private HardClip dist = new HardClip();



    void Awake()
    {
        PrepareToPlay();
        

    }

    void Update()
    {
        /*
        if (Input.GetKeyDown("a")) PlayNextAvailableVoice(0);
        if (Input.GetKeyUp("a")) StopVoice(0);
        if (Input.GetKeyDown("s")) PlayNextAvailableVoice(2);
        if (Input.GetKeyUp("s")) StopVoice(2);
        if (Input.GetKeyDown("d")) PlayNextAvailableVoice(4);
        if (Input.GetKeyUp("d")) StopVoice(4);
        if (Input.GetKeyDown("f")) PlayNextAvailableVoice(5);
        if (Input.GetKeyUp("f")) StopVoice(5);
        if (Input.GetKeyDown("g")) PlayNextAvailableVoice(7);
        if (Input.GetKeyUp("g")) StopVoice(7);
        if (Input.GetKeyDown("h")) PlayNextAvailableVoice(9);
        if (Input.GetKeyUp("h")) StopVoice(9);
        if (Input.GetKeyDown("j")) PlayNextAvailableVoice(11);
        if (Input.GetKeyUp("j")) StopVoice(11);
        if (Input.GetKeyDown("k")) PlayNextAvailableVoice(12);
        if (Input.GetKeyUp("k")) StopVoice(12);
        if (Input.GetKeyDown("l")) PlayNextAvailableVoice(14);
        if (Input.GetKeyUp("l")) StopVoice(14);
        */

        lineRenderer.positionCount = dataCopy.Length/2;
        for (int i = 0; i < dataCopy.Length; i += 2)
        {
            lineRenderer.SetPosition(i/2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), dataCopy[i]*0.25f, 0));
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        ProcessBlock(data);

    }


    public void StartNote(int noteID)
    {
        PlayNextAvailableVoice(noteID);
    }

    public void StopNote(int noteID)
    {
        StopVoice(noteID);
    }

    public void StopAllNotes()
    {
        StopAllVoices();
    }



    public void PlayNextAvailableVoice(int noteID)
    {
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) { voices[i].StartNote(noteID, 1); numActiveVoices++;  return; }
        }
    }

    public void StopVoice(int noteID)
    {
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].noteID == noteID && voices[i].CanPlay())
            {
                voices[i].StopNote(noteID, 1);
                numActiveVoices--;
                return;
            }
        
        }
    }

    public void StopAllVoices()
    {
        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].CanPlay())
            {
                voices[i].StopNote(i, 1);
                numActiveVoices--;
                //return;
            }

        }
    }




    public void PrepareToPlay()
    {
        // Setup voices
        voices = new SynthVoice[maxVoices];

        for(int i = 0; i < voices.Length; i++)
        {
            voices[i] = new SynthVoice(numChannels);
        }

        // Setup Effects
    }

    public void ProcessBlock(float[] data)
    {
        dist.SetDrive(drive);
        dist.SetDcOffset(dcOffset);

        for(int i=0; i<voices.Length; i++)
        {
            voices[i].lowpass.SetCoeffs(cutoffFrequency, Q, 0);
        }

        // Get samples from generators per voice
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) continue;

            voices[i].RenderBlock(data, numChannels, numActiveVoices);
        }

        for(int i=0; i<data.Length; i++)
        {
            data[i] = dist.ProcesSample(data[i]) * masterGain;
        }

        dataCopy = data;

        // Process effects


    }
}
