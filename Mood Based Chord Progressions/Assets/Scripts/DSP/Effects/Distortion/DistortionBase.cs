using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DistortionBase
{

    protected float drive = 1;
    protected float dcOffset = 0;
    protected int overSample = 1;
    protected float overSampleIncrement = 1;

    protected float lastSample = 0;

    public void SetDrive(float drive)
    {
        this.drive = drive;
    }
    public void SetDcOffset(float dcOffset)
    {
        this.dcOffset = dcOffset;
    }

    public void SetOverSample(int oversample)
    {
        this.overSample = oversample;
        overSampleIncrement = 1f / oversample;
    }

    public abstract float ProcesSample(float input);
    public abstract void ProcessBlock(float[] input, int numChannels);

}
