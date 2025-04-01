using System;
using UnityEngine;

//This script matches the data structure of the JSON file

[Serializable]
public class GameData
{
    public bool introSequenceDone;
    public bool gameComplete;
    public bool FFUnlocked;
    public bool FFHealed;
    public bool[] FF;
    public bool FEUnlocked;
    public bool FEHealed;
    public bool[] FE;
    public bool WSUnlocked;
    public bool WSHealed;
    public bool[] WS;
    public bool PBUnlocked;
    public bool PBHealed;
    public bool[] PB;
    public bool TMUnlocked;
    public bool TMHealed;
    public bool[] TM;
    public bool SSUnlocked;
    public bool SSHealed;
    public bool[] SS;

    // Constructor to initialize arrays
    public GameData()
    {
        FF = new bool[9];
        FE = new bool[9];
        WS = new bool[9];
        PB = new bool[9];
        TM = new bool[9];
        SS = new bool[9];
    }
}
