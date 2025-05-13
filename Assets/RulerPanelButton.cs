using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerPanelButton : MonoBehaviour
{

    public Animator RulerPanelAnim;

    public Animator FFAnimator;

    public MeshRenderer[] lokahiWheelRenderers = new MeshRenderer[6];
    public Material[,] lokahiWheelMaterials = new Material[6, 3];

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
        RulerPanelAnim.Play("Heal FF Ruler");
        FFAnimator.Play("FF Turn Healed");
    }

    public void CloseHealedRuler()
    {
        RulerPanelAnim.Play("Close FF Healed Ruler");
    }
}
