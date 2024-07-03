using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{

    public int numChannels = 2;
    public int maxVoices = 16;


    private int numActiveVoices = 0;


    private SynthVoice[] voices;


    public LineRenderer lineRenderer;
    private float[] dataCopy;



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
        // Get samples from generators per voice
        for(int i=0; i < voices.Length; i++)
        {
            if (!voices[i].CanPlay()) continue;

            voices[i].RenderBlock(data, numChannels, numActiveVoices);
        }

        dataCopy = data;

        // Process effects


    }
}
