using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain
{

    private float[,] probabilities =
    {
        { 1/7f,1/7f,1/7f,1/7f,1/7f,1/7f,1/7f},
        { 1/3f,0,0,0,1/3f,0,1/3f},
        {0,1/3f,0,1/3f,0,1/3f,0 },
        {1/3f,0,0,0,1/3f,0,1/3f },
        {1/2f,0,0,0,0,1/2f,0 },
        {0,1/2f,0,1/2f,0,0,0 },
        {1/2f,0,0,0,0,1/2f,0 },
    };


    public int GetNextChord(int lastChord){
        int chosenChord = 0;


        float sum = 0;
        System.Random rand = new System.Random();
        float randValue = (float)rand.NextDouble();
        for(int i=0; i<7; i++)
        {
            sum += probabilities[lastChord, i];
            if (randValue < sum)
            {
                chosenChord = i;
                break;
            }
        }
        
        return chosenChord;
    }



}
