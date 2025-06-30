using UnityEngine;

public class MokuUnlocker : MonoBehaviour
{
    public MokuSelectorGameManager mokuSelectorGameManager; // drag in Inspector

    public void UnlockAllMokuExceptFFandWS()
    {
        GameDataManager.Instance.UpdateFEUnlocked(true);
        GameDataManager.Instance.UpdatePBUnlocked(true);
        GameDataManager.Instance.UpdateTMUnlocked(true);
        GameDataManager.Instance.UpdateSSUnlocked(true);

        Debug.Log("Unlocked all Moku except FF and WS.");

        mokuSelectorGameManager.RefreshState();
    }

    public void UnlockWS()
    {
        GameDataManager.Instance.UpdateWSUnlocked(true);
        Debug.Log("Unlocked Moku WS.");

        mokuSelectorGameManager.RefreshState();
    }
}
