using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToWS : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void GoToWSMokuWheel()
    {
        GameDataManager.Instance.UpdateWSUnlocked(true);

        ResetAllLastOpenedFlags();

        GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, true);
        GameDataManager.Instance.UpdateWSTeleport(true);

        Invoke("LoadWSScene", 1f); // Delay for 1 second before loading
    }

    private void LoadWSScene()
    {
        SceneManager.LoadScene(5);
    }

    private void ResetAllLastOpenedFlags()
    {
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.FF, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.FE, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.PB, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.TM, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.SS, false);
    }
}
