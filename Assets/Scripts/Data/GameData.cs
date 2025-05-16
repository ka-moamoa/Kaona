using System;
using UnityEngine;

// This script matches the data structure of the JSON file

[Serializable]
public class GameData
{
    public bool introSequenceDone;
    public bool gameComplete;

    public bool firstMokuIntroDone;

    public bool FFUnlocked;
    public bool FFTileIntroDone;
    public bool FFHealed;
    public bool FFLastOpened;
    public bool[] FF;

    public bool FEUnlocked;
    public bool FETileIntroDone;
    public bool FEHealed;
    public bool FELastOpened;
    public bool[] FE;

    public bool WSUnlocked;
    public bool WSTileIntroDone;
    public bool WSHealed;
    public bool WSLastOpened;
    public bool[] WS;

    public bool PBUnlocked;
    public bool PBTileIntroDone;
    public bool PBHealed;
    public bool PBLastOpened;
    public bool[] PB;

    public bool TMUnlocked;
    public bool TMTileIntroDone;
    public bool TMHealed;
    public bool TMLastOpened;
    public bool[] TM;

    public bool SSUnlocked;
    public bool SSTileIntroDone;
    public bool SSHealed;
    public bool SSLastOpened;
    public bool[] SS;

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
