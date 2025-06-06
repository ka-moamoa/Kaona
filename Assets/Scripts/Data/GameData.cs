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
    public int[] FF; // Now uses int values: 0, 1, 2

    public bool FEUnlocked;
    public bool FETileIntroDone;
    public bool FEHealed;
    public bool FELastOpened;
    public int[] FE;

    public bool WSUnlocked;
    public bool WSTileIntroDone;
    public bool WSHealed;
    public bool WSLastOpened;
    public bool WSTeleport; // ✅ New line
    public int[] WS;

    public bool PBUnlocked;
    public bool PBTileIntroDone;
    public bool PBHealed;
    public bool PBLastOpened;
    public int[] PB;

    public bool TMUnlocked;
    public bool TMTileIntroDone;
    public bool TMHealed;
    public bool TMLastOpened;
    public int[] TM;

    public bool SSUnlocked;
    public bool SSTileIntroDone;
    public bool SSHealed;
    public bool SSLastOpened;
    public int[] SS;

    public GameData()
    {
        FF = new int[9];
        FE = new int[9];
        WS = new int[9];
        PB = new int[9];
        TM = new int[9];
        SS = new int[9];
    }
}
