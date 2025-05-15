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
    public GameObject introTileDone;
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

    [Header("Moku Audio")]
    public AudioSource[] MokuStartAudio = new AudioSource[6];

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
            int index = (int)moku;

            if (index >= mokus.Length || index >= lokahiWheelRenderers.Length || index >= lokahiWheelMaterials.Length)
                continue;

            var mokuState = mokus[index];
            var wheelRenderer = lokahiWheelRenderers[index];
            var materials = lokahiWheelMaterials[index]?.materials;

            if (mokuState == null)
                continue;

            bool isUnlocked = GameDataManager.Instance.GetUnlockedState(moku);
            bool isActivated = GameDataManager.Instance.GetTileIntroDoneState(moku);
            bool isHealed = GameDataManager.Instance.GetHealedState(moku);

            if (!isUnlocked)
            {
                mokuState.introTileDone?.SetActive(false);
                mokuState.unhealed?.SetActive(false);
                mokuState.healed?.SetActive(false);

                if (wheelRenderer != null && materials != null && materials.Length > 0)
                    wheelRenderer.material = materials[0]; // Deactivated

                continue;
            }

            if (!isHealed && wheelRenderer != null && materials != null && materials.Length > 1)
                wheelRenderer.material = materials[1]; // Unhealed

            if (!isActivated)
            {
                mokuState.introTileDone?.SetActive(true);
                mokuState.unhealed?.SetActive(true);
                mokuState.healed?.SetActive(false);

                Animator mokuAnimator = GetAnimatorForMoku(moku);
                if (mokuAnimator != null)
                {
                    mokuAnimator.enabled = true;

                    if (moku == MokuType.FF && !GameDataManager.Instance.GetFirstMokuIntroDone())
                    {
                        mokuAnimator.Play("IntroTilePlaying");
                    }
                    else
                    {
                        mokuAnimator.Play("IntroTileStart");
                    }
                }
            }
            else
            {
                SetMokuVisual(moku, isHealed);
                if (isHealed)
                    SetHealedMaterial(moku);
            }
        }
    }

    private void SetMokuVisual(MokuType moku, bool healed)
    {
        int index = (int)moku;
        if (index >= mokus.Length || mokus[index] == null)
            return;

        mokus[index].unhealed?.SetActive(!healed);
        mokus[index].healed?.SetActive(healed);
    }

    private void SetHealedMaterial(MokuType moku)
    {
        int index = (int)moku;
        if (index >= lokahiWheelRenderers.Length || index >= lokahiWheelMaterials.Length)
            return;

        if (lokahiWheelMaterials[index].materials.Length > 2 && lokahiWheelRenderers[index] != null)
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

    public void ActivateMoku(MokuType moku)
    {
        GameDataManager.Instance.UpdateTileIntroDoneState(moku, true);
        int index = (int)moku;
        if (index < mokus.Length && mokus[index] != null)
        {
            mokus[index].introTileDone?.SetActive(false);
            mokus[index].unhealed?.SetActive(true);
        }
    }

    public void PlayIntroTileLoop(MokuType moku)
    {
        int index = (int)moku;

        if (index < MokuStartAudio.Length && MokuStartAudio[index] != null)
        {
            MokuStartAudio[index].Play();
        }

        Animator mokuAnimator = GetAnimatorForMoku(moku);
        if (mokuAnimator != null)
        {
            mokuAnimator.Play("IntroTilePlaying");
        }

        GameDataManager.Instance.UpdateTileIntroDoneState(moku, true);

        if (rotateOnDrag != null)
        {
            rotateOnDrag.enabled = false;
        }
    }

    public void PlayIntroTileFinished(MokuType moku)
    {
        Animator mokuAnimator = GetAnimatorForMoku(moku);
        if (mokuAnimator != null)
        {
            mokuAnimator.Play("IntroTileFinished");
        }

        if (rotateOnDrag != null)
        {
            rotateOnDrag.enabled = true;
        }
    }

    // === Inspector Button Bindings ===
    public void OpenFFButton() => OpenRuler(MokuType.FF);
    public void CloseFFButton() => CloseRuler(MokuType.FF);
    public void HealFFButton() => HealRuler(MokuType.FF);
    public void CloseHealedFFButton() => CloseHealedRuler(MokuType.FF);
    public void ActivateFFButton() => ActivateMoku(MokuType.FF);
    public void PlayFFIntroTileLoop() => PlayIntroTileLoop(MokuType.FF);
    public void PlayFFIntroTileFinished() => PlayIntroTileFinished(MokuType.FF);

    public void OpenWSButton() => OpenRuler(MokuType.WS);
    public void CloseWSButton() => CloseRuler(MokuType.WS);
    public void HealWSButton() => HealRuler(MokuType.WS);
    public void CloseHealedWSButton() => CloseHealedRuler(MokuType.WS);
    public void ActivateWSButton() => ActivateMoku(MokuType.WS);
    public void PlayWSIntroTileLoop() => PlayIntroTileLoop(MokuType.WS);
    public void PlayWSIntroTileFinished() => PlayIntroTileFinished(MokuType.WS);

    public void OpenFEButton() => OpenRuler(MokuType.FE);
    public void CloseFEButton() => CloseRuler(MokuType.FE);
    public void HealFEButton() => HealRuler(MokuType.FE);
    public void CloseHealedFEButton() => CloseHealedRuler(MokuType.FE);
    public void ActivateFEButton() => ActivateMoku(MokuType.FE);
    public void PlayFEIntroTileLoop() => PlayIntroTileLoop(MokuType.FE);
    public void PlayFEIntroTileFinished() => PlayIntroTileFinished(MokuType.FE);

    public void OpenPBButton() => OpenRuler(MokuType.PB);
    public void ClosePBButton() => CloseRuler(MokuType.PB);
    public void HealPBButton() => HealRuler(MokuType.PB);
    public void CloseHealedPBButton() => CloseHealedRuler(MokuType.PB);
    public void ActivatePBButton() => ActivateMoku(MokuType.PB);
    public void PlayPBIntroTileLoop() => PlayIntroTileLoop(MokuType.PB);
    public void PlayPBIntroTileFinished() => PlayIntroTileFinished(MokuType.PB);

    public void OpenTMButton() => OpenRuler(MokuType.TM);
    public void CloseTMButton() => CloseRuler(MokuType.TM);
    public void HealTMButton() => HealRuler(MokuType.TM);
    public void CloseHealedTMButton() => CloseHealedRuler(MokuType.TM);
    public void ActivateTMButton() => ActivateMoku(MokuType.TM);
    public void PlayTMIntroTileLoop() => PlayIntroTileLoop(MokuType.TM);
    public void PlayTMIntroTileFinished() => PlayIntroTileFinished(MokuType.TM);

    public void OpenSSButton() => OpenRuler(MokuType.SS);
    public void CloseSSButton() => CloseRuler(MokuType.SS);
    public void HealSSButton() => HealRuler(MokuType.SS);
    public void CloseHealedSSButton() => CloseHealedRuler(MokuType.SS);
    public void ActivateSSButton() => ActivateMoku(MokuType.SS);
    public void PlaySSIntroTileLoop() => PlayIntroTileLoop(MokuType.SS);
    public void PlaySSIntroTileFinished() => PlayIntroTileFinished(MokuType.SS);
}
