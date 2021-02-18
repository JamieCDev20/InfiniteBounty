using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflinePlayer : MonoBehaviour
{
    [SerializeField] private CameraController c_control;
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private OfflineToolLoad tool;

    private ToolHandler th;

    private void Awake()
    {
        th = GetComponent<ToolHandler>();
        pim.SetCamera(c_control);
        Invoke(nameof(GiveTool), 0.5f);
    }

    private void GiveTool()
    {
        th.SwapTool(ToolSlot.moblility, (int)tool);
    }

}

public enum OfflineToolLoad
{
    Teleporter,
    Jetpack,
    Lunchbox,
    BubbleShield
}
