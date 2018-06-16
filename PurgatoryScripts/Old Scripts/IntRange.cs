using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntRange
{
	//Custom class just for ease of use 
	//Returns a random value between the set values in other parts IntRange(2, 3) returns a number between 2 and 3
    public int valueMin;
    public int valueMax;

    public IntRange(int min, int max)
    {
        valueMax = max;
        valueMin = min;
    }

    public int Random
    {
        get {

            float number = UnityEngine.Random.Range(valueMin, valueMax);
            Mathf.Round(number);
            int randomNumber = (int)number;

            return randomNumber;

        }
    }
}
