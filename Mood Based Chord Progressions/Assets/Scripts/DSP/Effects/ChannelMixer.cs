using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelMixer : MonoBehaviour
{
    public float leftToLeft = 1;
    public float leftToRight = 0;
    public float rightToRight = 0;
    public float rightToLeft = 1;


    public void ProcessBlock(float[] data, int numChannels)
    {
        for(int i=0; i<data.Length; i += numChannels)
        {
            data[i] = data[i] * leftToLeft + data[i + 1] * rightToLeft;
            data[i + 1] = data[i + 1] * rightToRight + data[i] * leftToRight;
        }
    }
}
