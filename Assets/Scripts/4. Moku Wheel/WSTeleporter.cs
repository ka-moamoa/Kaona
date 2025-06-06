using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSTeleporter : MonoBehaviour
{
    public GameObject teleportObject; // Assign in Inspector
    public Animator teleportAnimator; // Assign in Inspector

    void Start()
    {
        if (GameDataManager.Instance != null && teleportObject != null)
        {
            bool shouldShowTeleport = GameDataManager.Instance.GetWSTeleport();
            teleportObject.SetActive(shouldShowTeleport);
            Debug.Log($"Teleport object set to active: {shouldShowTeleport}");
        }
        else
        {
            Debug.LogWarning("GameDataManager or teleportObject not assigned.");
        }
    }

    public void CloseTeleporter()
    {
        if (teleportAnimator != null)
        {
            teleportAnimator.Play("Dismiss Teleport");
            Debug.Log("Dismiss Teleport animation played.");

            GameDataManager.Instance.UpdateWSTeleport(false);

            Invoke(nameof(DisableTeleportObject), 1f); // Delay call
        }
        else
        {
            Debug.LogWarning("teleportAnimator reference is not assigned.");
        }
    }

    private void DisableTeleportObject()
    {
        if (teleportObject != null)
        {
            teleportObject.SetActive(false);
            Debug.Log("Teleport object deactivated.");
        }
    }
}
