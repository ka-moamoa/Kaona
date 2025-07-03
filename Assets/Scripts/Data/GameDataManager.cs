using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    private const string FileName = "gameData.json";
    private const string BackupFileName = "gameData_backup.json";
    public GameData gameData;

    private bool storytellerMode = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
            SaveGameData();
        }
    }

    public void SaveGameData(bool forceSave = false)
    {
        if (storytellerMode && !forceSave)
        {
            Debug.Log("Save prevented in Storyteller Mode.");
            return;
        }

        string json = JsonUtility.ToJson(gameData, true);
        string path = Path.Combine(Application.persistentDataPath, FileName);
        File.WriteAllText(path, json);
        Debug.Log($"Game data saved to {path}");
    }


    public void ResetGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Game data file deleted at {path}");
        }

        gameData = new GameData();
        SaveGameData();

        Debug.Log("Game data has been reset and saved as new.");
    }

    // === Storyteller Mode ===

    public void EnableStorytellerMode()
    {
        if (storytellerMode)
        {
            Debug.Log("Storyteller mode already enabled.");
            return;
        }

        string sourcePath = Path.Combine(Application.persistentDataPath, FileName);
        string backupPath = Path.Combine(Application.persistentDataPath, BackupFileName);

        if (File.Exists(sourcePath))
        {
            File.Copy(sourcePath, backupPath, true);
            File.Delete(sourcePath);
            Debug.Log("Backup created and original game data deleted for Storyteller Mode.");
        }

        gameData = new GameData();

        // Unlock all Moku tiles
        gameData.FFUnlocked = true;
        gameData.FEUnlocked = true;
        gameData.WSUnlocked = true;
        gameData.PBUnlocked = true;
        gameData.TMUnlocked = true;
        gameData.SSUnlocked = true;

        storytellerMode = true;
        SaveGameData();

        Debug.Log("Storyteller mode enabled with all Moku tiles unlocked.");
    }


    public void DisableStorytellerMode()
    {
        if (!storytellerMode)
        {
            Debug.Log("Storyteller mode already disabled.");
            return;
        }

        string backupPath = Path.Combine(Application.persistentDataPath, BackupFileName);
        string originalPath = Path.Combine(Application.persistentDataPath, FileName);

        if (File.Exists(backupPath))
        {
            File.Copy(backupPath, originalPath, true);
            File.Delete(backupPath);
            Debug.Log("Storyteller mode disabled and backup restored.");
        }
        else
        {
            Debug.LogWarning("No backup found to restore when disabling storyteller mode.");
        }

        LoadGameData(); // Reload the now-restored original data
        storytellerMode = false;
    }


    public bool IsStorytellerModeActive()
    {
        return storytellerMode;
    }

    private void OnApplicationQuit()
    {
        if (storytellerMode)
        {
            DisableStorytellerMode();
        }
    }

    // === Generic Methods ===

    public void UpdateIntroSequenceDone(bool value)
    {
        gameData.introSequenceDone = value;
        Debug.Log($"Intro Sequence Done set to {value}");
        SaveGameData();
    }

    public void UpdateGameComplete(bool value)
    {
        gameData.gameComplete = value;
        Debug.Log($"Game Complete set to {value}");
        SaveGameData();
    }

    // === FF ===
    public void UpdateFFUnlocked(bool value)
    {
        gameData.FFUnlocked = value;
        Debug.Log($"FF Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdateFFTileIntroDone(bool value)
    {
        gameData.FFTileIntroDone = value;
        Debug.Log($"FF TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdateFFHealed(bool value)
    {
        gameData.FFHealed = value;
        Debug.Log($"FF Healed set to {value}");
        SaveGameData();
    }

    public void UpdateFFData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] FFData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.FF.Length)
        {
            gameData.FF[index] = value;
            Debug.Log($"Updated FF[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === FE ===
    public void UpdateFEUnlocked(bool value)
    {
        gameData.FEUnlocked = value;
        Debug.Log($"FE Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdateFETileIntroDone(bool value)
    {
        gameData.FETileIntroDone = value;
        Debug.Log($"FE TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdateFEHealed(bool value)
    {
        gameData.FEHealed = value;
        Debug.Log($"FE Healed set to {value}");
        SaveGameData();
    }

    public void UpdateFEData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] FEData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.FE.Length)
        {
            gameData.FE[index] = value;
            Debug.Log($"Updated FE[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === WS ===
    public void UpdateWSUnlocked(bool value)
    {
        gameData.WSUnlocked = value;
        Debug.Log($"WS Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdateWSTileIntroDone(bool value)
    {
        gameData.WSTileIntroDone = value;
        Debug.Log($"WS TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdateWSHealed(bool value)
    {
        gameData.WSHealed = value;
        Debug.Log($"WS Healed set to {value}");
        SaveGameData();
    }

    public void UpdateWSData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] WSData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.WS.Length)
        {
            gameData.WS[index] = value;
            Debug.Log($"Updated WS[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === PB ===
    public void UpdatePBUnlocked(bool value)
    {
        gameData.PBUnlocked = value;
        Debug.Log($"PB Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdatePBTileIntroDone(bool value)
    {
        gameData.PBTileIntroDone = value;
        Debug.Log($"PB TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdatePBHealed(bool value)
    {
        gameData.PBHealed = value;
        Debug.Log($"PB Healed set to {value}");
        SaveGameData();
    }

    public void UpdatePBData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] PBData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.PB.Length)
        {
            gameData.PB[index] = value;
            Debug.Log($"Updated PB[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === TM ===
    public void UpdateTMUnlocked(bool value)
    {
        gameData.TMUnlocked = value;
        Debug.Log($"TM Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdateTMTileIntroDone(bool value)
    {
        gameData.TMTileIntroDone = value;
        Debug.Log($"TM TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdateTMHealed(bool value)
    {
        gameData.TMHealed = value;
        Debug.Log($"TM Healed set to {value}");
        SaveGameData();
    }

    public void UpdateTMData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] TMData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.TM.Length)
        {
            gameData.TM[index] = value;
            Debug.Log($"Updated TM[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === SS ===
    public void UpdateSSUnlocked(bool value)
    {
        gameData.SSUnlocked = value;
        Debug.Log($"SS Unlocked set to {value}");
        SaveGameData();
    }

    public void UpdateSSTileIntroDone(bool value)
    {
        gameData.SSTileIntroDone = value;
        Debug.Log($"SS TileIntroDone set to {value}");
        SaveGameData(true);
    }

    public void UpdateSSHealed(bool value)
    {
        gameData.SSHealed = value;
        Debug.Log($"SS Healed set to {value}");
        SaveGameData();
    }

    public void UpdateSSData(int index, int value)
    {
        if (storytellerMode)
        {
            Debug.Log($"[Storyteller Mode] SSData update blocked: index {index}, value {value}");
            return;
        }

        if (index >= 0 && index < gameData.SS.Length)
        {
            gameData.SS[index] = value;
            Debug.Log($"Updated SS[{index}] to {value}");
            SaveGameData();
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // === Generic TileIntroDone Accessors ===

    public void UpdateTileIntroDoneState(MokuType moku, bool value)
    {
        switch (moku)
        {
            case MokuType.FF: UpdateFFTileIntroDone(value); break;
            case MokuType.FE: UpdateFETileIntroDone(value); break;
            case MokuType.WS: UpdateWSTileIntroDone(value); break;
            case MokuType.PB: UpdatePBTileIntroDone(value); break;
            case MokuType.TM: UpdateTMTileIntroDone(value); break;
            case MokuType.SS: UpdateSSTileIntroDone(value); break;
        }
    }

    public bool GetTileIntroDoneState(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => gameData.FFTileIntroDone,
            MokuType.FE => gameData.FETileIntroDone,
            MokuType.WS => gameData.WSTileIntroDone,
            MokuType.PB => gameData.PBTileIntroDone,
            MokuType.TM => gameData.TMTileIntroDone,
            MokuType.SS => gameData.SSTileIntroDone,
            _ => false
        };
    }

    // === Other Shared Accessors ===

    public void UpdateHealedState(MokuType moku, bool value)
    {
        switch (moku)
        {
            case MokuType.FF: UpdateFFHealed(value); break;
            case MokuType.WS: UpdateWSHealed(value); break;
            case MokuType.FE: UpdateFEHealed(value); break;
            case MokuType.PB: UpdatePBHealed(value); break;
            case MokuType.TM: UpdateTMHealed(value); break;
            case MokuType.SS: UpdateSSHealed(value); break;
        }
    }

    public bool GetHealedState(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => gameData.FFHealed,
            MokuType.WS => gameData.WSHealed,
            MokuType.FE => gameData.FEHealed,
            MokuType.PB => gameData.PBHealed,
            MokuType.TM => gameData.TMHealed,
            MokuType.SS => gameData.SSHealed,
            _ => false
        };
    }

    public bool GetUnlockedState(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => gameData.FFUnlocked,
            MokuType.WS => gameData.WSUnlocked,
            MokuType.FE => gameData.FEUnlocked,
            MokuType.PB => gameData.PBUnlocked,
            MokuType.TM => gameData.TMUnlocked,
            MokuType.SS => gameData.SSUnlocked,
            _ => false
        };
    }

    // === First Moku Intro ===
    public void UpdateFirstMokuIntroDone(bool value)
    {
        gameData.firstMokuIntroDone = value;
        Debug.Log($"First Moku Intro Done set to {value}");
        SaveGameData();
    }

    public bool GetFirstMokuIntroDone()
    {
        return gameData.firstMokuIntroDone;
    }

    // === Moku Last Opened Getters/Setters ===
    public void UpdateLastOpenedState(MokuType moku, bool value)
    {
        switch (moku)
        {
            case MokuType.FF: gameData.FFLastOpened = value; break;
            case MokuType.FE: gameData.FELastOpened = value; break;
            case MokuType.WS: gameData.WSLastOpened = value; break;
            case MokuType.PB: gameData.PBLastOpened = value; break;
            case MokuType.TM: gameData.TMLastOpened = value; break;
            case MokuType.SS: gameData.SSLastOpened = value; break;
        }
        Debug.Log($"{moku} LastOpened set to {value}");
        SaveGameData(true);
    }

    public bool GetLastOpenedState(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => gameData.FFLastOpened,
            MokuType.FE => gameData.FELastOpened,
            MokuType.WS => gameData.WSLastOpened,
            MokuType.PB => gameData.PBLastOpened,
            MokuType.TM => gameData.TMLastOpened,
            MokuType.SS => gameData.SSLastOpened,
            _ => false
        };
    }

    public void UpdateWSTeleport(bool value)
    {
        gameData.WSTeleport = value;
        Debug.Log($"WS Teleport set to {value}");
        SaveGameData();
    }

    public bool GetWSTeleport()
    {
        return gameData.WSTeleport;
    }

    public void UpdateRulerPanelOpenOnReturn(MokuType moku, bool value)
    {
        switch (moku)
        {
            case MokuType.FF: gameData.FFRulerPanelOpenOnReturn = value; break;
            case MokuType.FE: gameData.FERulerPanelOpenOnReturn = value; break;
            case MokuType.WS: gameData.WSRulerPanelOpenOnReturn = value; break;
            case MokuType.PB: gameData.PBRulerPanelOpenOnReturn = value; break;
            case MokuType.TM: gameData.TMRulerPanelOpenOnReturn = value; break;
            case MokuType.SS: gameData.SSRulerPanelOpenOnReturn = value; break;
        }
        Debug.Log($"{moku} RulerPanelOpenOnReturn set to {value}");
        SaveGameData();
    }

    public bool GetRulerPanelOpenOnReturn(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => gameData.FFRulerPanelOpenOnReturn,
            MokuType.FE => gameData.FERulerPanelOpenOnReturn,
            MokuType.WS => gameData.WSRulerPanelOpenOnReturn,
            MokuType.PB => gameData.PBRulerPanelOpenOnReturn,
            MokuType.TM => gameData.TMRulerPanelOpenOnReturn,
            MokuType.SS => gameData.SSRulerPanelOpenOnReturn,
            _ => false
        };
    }
}
