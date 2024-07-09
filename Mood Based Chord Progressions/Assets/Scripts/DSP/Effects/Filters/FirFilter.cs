using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirFilter
{
    private int bufferSize;
    private float[] buffer;
    private int readP;
    private int writeP;

    public float[] coefficients;



    public FirFilter(int order)
    {
        buffer = new float[order];
        coefficients = new float[order];
        bufferSize = order;


        for(int i=0; i<bufferSize; i++)
        {
            int index = i - order / 2;

            if(i%2==1) coefficients[i] = 2f / (Mathf.PI * index);
            if (index == 0) coefficients[i] = 0;
        }
    }

    public void SetCoeffs(float[] coeffs)
    {
        coefficients = coeffs;
    }

    public void SetHilbert(float f)
    {
        for (int i = 0; i < bufferSize; i++)
        {
            int index = i - bufferSize / 2;

            if (i % 2 == 1) coefficients[i] = 2f / (Mathf.PI * index) * Mathf.Sin(index*f);
            if (index == 0) coefficients[i] = 0;
        }
    }

    public float Process(float input)
    {

        float output = 0;
        for(int j=0; j<bufferSize; j++)
        {
            int index = j;
            if (index >= bufferSize) index -= bufferSize;
            output += buffer[(index+readP)%bufferSize] * coefficients[index];
        }

        buffer[writeP] = input;

        writeP++;
        if (writeP >= bufferSize) writeP -= bufferSize;

        readP++;
        if (readP >= bufferSize) readP -= bufferSize;


        return output;
    }
}
