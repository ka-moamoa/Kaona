using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenRulerPanelOnReturn : MonoBehaviour
{

    [Header("Scene Override Settings")]
    public bool overrideFF = false;
    public bool overrideFE = false;
    public bool overrideWS = false;
    public bool overridePB = false;
    public bool overrideTM = false;
    public bool overrideSS = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InvokeChangeAfterDelay()
    {
        if (overrideFF)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.FF, true);
        }
        else if (overrideFE)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.FE, true);
        }
        else if (overrideWS)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.WS, true);
        }
        else if (overridePB)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.PB, true);
        }
        else if (overrideTM)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.TM, true);
        }
        else if (overrideSS)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.SS, true);
        }
        else
        {

        }
    }

}
