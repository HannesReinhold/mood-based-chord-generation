using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandSplitter
{
    public int numBands;

    public Butterwoth[] filters;



    public BandSplitter(int numBands, float[] frequencies)
    {
        this.numBands = numBands;
        filters = new Butterwoth[(numBands-1)*2];

        for(int i=0; i < filters.Length; i++)
        {
            filters[i] = new Butterwoth();
        }

        for(int i=0; i < numBands; i++)
        {
            if (i == 0)
            {
                filters[i].CalcCoeffs(frequencies[0],48000, true);
            }
            else if(i>0 && i < numBands - 1)
            {
                filters[i].CalcCoeffs(frequencies[i - 1], 48000, false);
                filters[i+1].CalcCoeffs(frequencies[i], 48000, true);
            }
            else if(i== numBands - 1)
            {
                filters[filters.Length-1].CalcCoeffs(frequencies[numBands-2],48000, false);
            }
            else
            {
            }
        }

        for(int i=0; i<filters.Length; i++)
        {
            Debug.Log((filters[i].lowpass ? "Lowpass" : "Highpass")+ " At "+filters[i].freq);
        }
    }

    public float[] Process(float input)
    {
        float[] output = new float[numBands];

        for(int i=0; i<numBands; i++)
        {
            if (i == 0)
            {
                output[i] = filters[i].Process(input);
            }
            else if(i>=0 && i< numBands - 1)
            {
                output[i] = filters[i].Process(filters[i + 1].Process(input));
            }
            else if (i == numBands - 1)
            {
                output[i] = filters[filters.Length-1].Process(input);
            }

            
        }

        return output;
    }


}
