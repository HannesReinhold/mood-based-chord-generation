using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput
{
    private int octave = 4;

    public MidiDevice device;

    private Dictionary<KeyCode, int> midiKeyBindings = new Dictionary<KeyCode, int>()
    {
        {KeyCode.Z, 0 },{KeyCode.X, 2 },{KeyCode.C, 4 },{KeyCode.V, 5 },{KeyCode.B, 7 },{KeyCode.N, 9 },{KeyCode.M, 11 },{KeyCode.Comma, 12 },{KeyCode.Period, 14 },
        {KeyCode.S, 1 },{KeyCode.D, 3 },{KeyCode.G, 6 },{KeyCode.H, 8 },{KeyCode.J, 10 },{KeyCode.L, 13 },
        {KeyCode.Q, 12 },{KeyCode.W, 14 },{KeyCode.E, 16 },{KeyCode.R, 17 },{KeyCode.T, 19 },{KeyCode.Y, 21 },{KeyCode.U, 23 },{KeyCode.I, 24 },{KeyCode.O, 26 },{KeyCode.P, 28 },
        {KeyCode.Alpha2, 13 },{KeyCode.Alpha3, 15 },{KeyCode.Alpha5, 18 },{KeyCode.Alpha6, 20 },{KeyCode.Alpha7, 22 },{KeyCode.Alpha9, 25 }

    };


    public void Update()
    {


        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode) && midiKeyBindings.ContainsKey(kcode))
            {

                device.StartNote(midiKeyBindings[kcode] + octave * 12,0);
            }
            if (Input.GetKeyUp(kcode) && midiKeyBindings.ContainsKey(kcode))
            {
                device.StopNote(midiKeyBindings[kcode] + octave * 12,0);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            octave++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            octave--;
        }
    }
}
