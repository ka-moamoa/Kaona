using UnityEngine;

public class RulerPanelManager : MonoBehaviour
{
    private MokuSelectorGameManager gameManager;

    void Start()
    {
        GameObject managerObject = GameObject.FindGameObjectWithTag("Game Manager");

        if (managerObject == null)
        {
            Debug.LogError("Game Manager not found.");
            return;
        }

        gameManager = managerObject.GetComponent<MokuSelectorGameManager>();

        if (gameManager == null)
        {
            Debug.LogError("MokuSelectorGameManager component not found on Game Manager.");
            return;
        }

        CheckAndOpenRuler(MokuType.FF);
        CheckAndOpenRuler(MokuType.FE);
        CheckAndOpenRuler(MokuType.WS);
        CheckAndOpenRuler(MokuType.PB);
        CheckAndOpenRuler(MokuType.TM);
        CheckAndOpenRuler(MokuType.SS);
    }

    private void CheckAndOpenRuler(MokuType moku)
    {
        if (GameDataManager.Instance.GetRulerPanelOpenOnReturn(moku))
        {
            gameManager.OpenRuler(moku);
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(moku, false);
            Debug.Log($"Opened {moku} ruler panel and reset flag.");
        }
    }
}
