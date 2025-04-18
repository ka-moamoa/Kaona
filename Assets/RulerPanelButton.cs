using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerPanelButton : MonoBehaviour
{

    public Animator RulerPanelAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFFRuler()
    {
        RulerPanelAnim.Play("Open FF Ruler");
    }

    public void CloseFFRuler()
    {
        RulerPanelAnim.Play("Close FF Ruler");
    }

    public void HealFFRuler()
    {
        RulerPanelAnim.Play("OpenFFRuler");
    }
}
