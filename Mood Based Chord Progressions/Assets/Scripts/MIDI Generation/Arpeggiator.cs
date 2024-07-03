using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Arpeggiator : MonoBehaviour
{
    public Synthesizer synthesizerRef;

    public float bpm;
    public float rate;

    private float timer = 0;

    private List<int> notes;

    public bool canPlay = false;

    private bool lastCanPlay = false;

    private IEnumerator updateLoop;

    private Thread thread;

    private void Start()
    {
    }

    private void OnDisable()
    {
       canPlay = false;
    }

    private void OnValidate()
    {
        if (canPlay != lastCanPlay)
        {
            lastCanPlay = canPlay;
            updateLoop = UpdateArp();
            //StartCoroutine(updateLoop);
            //Play();
            thread = new Thread(Run);
            thread.Start();
        }
        
    }
    int i = 0;
    void Run()
    {
        while (canPlay)
        {
            synthesizerRef.StopAllVoices();
            synthesizerRef.PlayNextAvailableVoice(48);

            Debug.Log(i);
            Thread.Sleep((int)(60f / bpm * rate*1000));
            i++;
        }
    }

    private void Play()
    {
        synthesizerRef.StopAllVoices();
        synthesizerRef.PlayNextAvailableVoice(48);

        if (canPlay) Invoke("Play", 60f / bpm * rate);
    }

    private IEnumerator UpdateArp()
    {
        synthesizerRef.StopAllVoices();
        synthesizerRef.PlayNextAvailableVoice(48);

        while (canPlay)
        {
            yield return new WaitForSecondsRealtime(60/bpm*rate);

            synthesizerRef.StopAllVoices();
            synthesizerRef.PlayNextAvailableVoice(48);
        }

        

    }

}
