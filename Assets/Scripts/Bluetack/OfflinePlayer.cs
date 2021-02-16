using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflinePlayer : MonoBehaviour
{
    [SerializeField] private CameraController c_control;
    [SerializeField] private PlayerInputManager pim;

    private void Awake()
    {
        pim.SetCamera(c_control);
    }

}
