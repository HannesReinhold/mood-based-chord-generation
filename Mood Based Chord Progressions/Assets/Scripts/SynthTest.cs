using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthTest : MonoBehaviour
{
    private KeyboardInput input = new KeyboardInput();
    private Synthesizer synth = new Synthesizer();
    private Arpeggiator arp = new Arpeggiator();

    public LineRenderer oscilloscope;
    private float[] dataCopy;


    // Start is called before the first frame update
    void Start()
    {
        arp.device = synth;
        input.device = arp;
        synth.PrepareToPlay();

        arp.bpm = 140;
        arp.rate = 1 / 4f;
    }

    // Update is called once per frame
    void Update()
    {
        input.Update();


        oscilloscope.positionCount = dataCopy.Length / 2;
        for (int i = 0; i < dataCopy.Length; i += 2)
        {
            oscilloscope.SetPosition(i / 2, new Vector3(Mathf.Lerp(-1, 1, (float)i / dataCopy.Length), dataCopy[i] * 0.25f, 0));
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i=0; i<data.Length; i+=2)
        {
            arp.UpdateArp();
        }
        
        synth.ProcessBlock(data, channels);

        dataCopy = data;
    }
}
