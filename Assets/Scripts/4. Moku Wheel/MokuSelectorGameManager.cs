using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialSet
{
    public Material[] materials = new Material[3]; // 0: Deactivated, 1: Unhealed, 2: Healed
}

[System.Serializable]
public class MokuState
{
    public GameObject unhealed;
    public GameObject healed;
}

public enum MokuType { FF = 0, WS = 1, FE = 2, PB = 3, TM = 4, SS = 5 }

public class MokuSelectorGameManager : MonoBehaviour
{
    [Header("Main Panel Animator")]
    public Animator RulerPanelAnim;

    [Header("Moku Animators")]
    public Animator FFAnimator;
    public Animator WSAnimator;
    public Animator FEAnimator;
    public Animator PBAnimator;
    public Animator TMAnimator;
    public Animator SSAnimator;

    [Header("Visual Components")]
    public MeshRenderer[] lokahiWheelRenderers = new MeshRenderer[6];
    public MaterialSet[] lokahiWheelMaterials = new MaterialSet[6];
    public MokuState[] mokus = new MokuState[6];

    private RotateOnDrag rotateOnDrag;

    void Start()
    {
        GameObject wheel = GameObject.FindWithTag("Lokahi Wheel");
        if (wheel != null)
        {
            rotateOnDrag = wheel.GetComponent<RotateOnDrag>();
        }

        foreach (MokuType moku in System.Enum.GetValues(typeof(MokuType)))
        {
            bool isHealed = GameDataManager.Instance.GetHealedState(moku);
            SetMokuVisual(moku, isHealed);

            if (isHealed)
            {
                SetHealedMaterial(moku);
            }
        }
    }

    private void SetMokuVisual(MokuType moku, bool healed)
    {
        int index = (int)moku;

        if (index >= mokus.Length || mokus[index] == null)
        {
            Debug.LogWarning($"Moku index {index} is missing in 'mokus' array.");
            return;
        }

        if (mokus[index].unhealed == null || mokus[index].healed == null)
        {
            Debug.LogWarning($"Moku {moku} is missing unhealed/healed references.");
            return;
        }

        mokus[index].unhealed.SetActive(!healed);
        mokus[index].healed.SetActive(healed);
    }


    private void SetHealedMaterial(MokuType moku)
    {
        int index = (int)moku;

        if (index >= lokahiWheelRenderers.Length || index >= lokahiWheelMaterials.Length)
            return;

        if (lokahiWheelMaterials[index].materials.Length <= 2 || lokahiWheelRenderers[index] == null)
            return;

        lokahiWheelRenderers[index].material = lokahiWheelMaterials[index].materials[2];
    }


    private Animator GetAnimatorForMoku(MokuType moku)
    {
        return moku switch
        {
            MokuType.FF => FFAnimator,
            MokuType.WS => WSAnimator,
            MokuType.FE => FEAnimator,
            MokuType.PB => PBAnimator,
            MokuType.TM => TMAnimator,
            MokuType.SS => SSAnimator,
            _ => null
        };
    }

    public void OpenRuler(MokuType moku)
    {
        RulerPanelAnim.Play($"Open {moku} Ruler");
        if (rotateOnDrag != null) rotateOnDrag.enabled = false;
    }

    public void CloseRuler(MokuType moku)
    {
        RulerPanelAnim.Play($"Close {moku} Ruler");
        if (rotateOnDrag != null) rotateOnDrag.enabled = true;
    }

    public void HealRuler(MokuType moku)
    {
        //Animator mokuAnim = GetAnimatorForMoku(moku);
        //mokuAnim?.Play($"{moku} Turn Healed");

        RulerPanelAnim.Play($"Heal {moku} Ruler");

        SetMokuVisual(moku, true);
        SetHealedMaterial(moku);
    }

    public void CloseHealedRuler(MokuType moku)
    {
        RulerPanelAnim.Play($"Close {moku} Healed Ruler");
        GameDataManager.Instance.UpdateHealedState(moku, true);
        if (rotateOnDrag != null) rotateOnDrag.enabled = true;
    }

    // === Button Wrappers for Unity Inspector ===

    public void OpenFFButton() => OpenRuler(MokuType.FF);
    public void CloseFFButton() => CloseRuler(MokuType.FF);
    public void HealFFButton() => HealRuler(MokuType.FF);
    public void CloseHealedFFButton() => CloseHealedRuler(MokuType.FF);

    public void OpenWSButton() => OpenRuler(MokuType.WS);
    public void CloseWSButton() => CloseRuler(MokuType.WS);
    public void HealWSButton() => HealRuler(MokuType.WS);
    public void CloseHealedWSButton() => CloseHealedRuler(MokuType.WS);

    public void OpenFEButton() => OpenRuler(MokuType.FE);
    public void CloseFEButton() => CloseRuler(MokuType.FE);
    public void HealFEButton() => HealRuler(MokuType.FE);
    public void CloseHealedFEButton() => CloseHealedRuler(MokuType.FE);

    public void OpenPBButton() => OpenRuler(MokuType.PB);
    public void ClosePBButton() => CloseRuler(MokuType.PB);
    public void HealPBButton() => HealRuler(MokuType.PB);
    public void CloseHealedPBButton() => CloseHealedRuler(MokuType.PB);

    public void OpenTMButton() => OpenRuler(MokuType.TM);
    public void CloseTMButton() => CloseRuler(MokuType.TM);
    public void HealTMButton() => HealRuler(MokuType.TM);
    public void CloseHealedTMButton() => CloseHealedRuler(MokuType.TM);

    public void OpenSSButton() => OpenRuler(MokuType.SS);
    public void CloseSSButton() => CloseRuler(MokuType.SS);
    public void HealSSButton() => HealRuler(MokuType.SS);
    public void CloseHealedSSButton() => CloseHealedRuler(MokuType.SS);
}
