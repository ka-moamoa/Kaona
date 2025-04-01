using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; } // Singleton instance
    private const string FileName = "gameData.json";
    public GameData gameData; // Make gameData public for access in other scripts

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object between scenes
            LoadGameData(); // Load data on initialization
            UpdateSSData(5, true);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance of GameDataManager exists
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
            // Initialize with default values if the file doesn't exist
            gameData = new GameData();
            SaveGameData(); // Optionally save the default data
        }
    }

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        string path = Path.Combine(Application.persistentDataPath, FileName);
        File.WriteAllText(path, json);
        Debug.Log($"Game data saved to {path}");
    }

    //Updating Classes
    public void UpdateIntroSequenceDone(bool value)
    {
        gameData.introSequenceDone = value;
        Debug.Log($"Intro Sequence Done set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateGameComplete(bool value)
    {
        gameData.gameComplete = value;
        Debug.Log($"Game Complete set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateFFUnlocked(bool value)
    {
        gameData.FFUnlocked = value;
        Debug.Log($"FF Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateFFHealed(bool value)
    {
        gameData.FFHealed = value;
        Debug.Log($"FF Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateFFData(int index, bool value)
    {
        if (index >= 0 && index < gameData.FF.Length)
        {
            gameData.FF[index] = value;
            Debug.Log($"Updated FF[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    public void UpdateFEUnlocked(bool value)
    {
        gameData.FEUnlocked = value;
        Debug.Log($"FE Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateFEHealed(bool value)
    {
        gameData.FEHealed = value;
        Debug.Log($"FE Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateFEData(int index, bool value)
    {
        if (index >= 0 && index < gameData.FE.Length)
        {
            gameData.FE[index] = value;
            Debug.Log($"Updated FE[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    public void UpdateWSUnlocked(bool value)
    {
        gameData.WSUnlocked = value;
        Debug.Log($"WS Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateWSHealed(bool value)
    {
        gameData.WSHealed = value;
        Debug.Log($"WS Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateWSData(int index, bool value)
    {
        if (index >= 0 && index < gameData.WS.Length)
        {
            gameData.WS[index] = value;
            Debug.Log($"Updated WS[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    public void UpdatePBUnlocked(bool value)
    {
        gameData.PBUnlocked = value;
        Debug.Log($"PB Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdatePBHealed(bool value)
    {
        gameData.PBHealed = value;
        Debug.Log($"PB Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdatePBData(int index, bool value)
    {
        if (index >= 0 && index < gameData.PB.Length)
        {
            gameData.PB[index] = value;
            Debug.Log($"Updated PB[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    public void UpdateTMUnlocked(bool value)
    {
        gameData.TMUnlocked = value;
        Debug.Log($"TM Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateTMHealed(bool value)
    {
        gameData.TMHealed = value;
        Debug.Log($"TM Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateTMData(int index, bool value)
    {
        if (index >= 0 && index < gameData.TM.Length)
        {
            gameData.TM[index] = value;
            Debug.Log($"Updated TM[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    public void UpdateSSUnlocked(bool value)
    {
        gameData.SSUnlocked = value;
        Debug.Log($"SS Unlocked set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateSSHealed(bool value)
    {
        gameData.SSHealed = value;
        Debug.Log($"SS Healed set to {value}");
        SaveGameData(); // Save after update
    }

    public void UpdateSSData(int index, bool value)
    {
        if (index >= 0 && index < gameData.SS.Length)
        {
            gameData.SS[index] = value;
            Debug.Log($"Updated SS[{index}] to {value}");
            SaveGameData(); // Save after update
        }
        else
        {
            Debug.LogError("Index out of bounds");
        }
    }

    // Add more update methods for other data fields as needed
}
