using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialSet
{
    public Material[] materials = new Material[3];
}

public class MokuSelectorGameManager : MonoBehaviour
{

    public Animator RulerPanelAnim;

    public Animator FFAnimator;
    public Animator WSAnimator;
    public Animator FEAnimator;
    public Animator PBAnimator;
    public Animator TMAnimator;
    public Animator SSAnimator;


    public MeshRenderer[] lokahiWheelRenderers = new MeshRenderer[6];
    public MaterialSet[] lokahiWheelMaterials = new MaterialSet[6];


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
