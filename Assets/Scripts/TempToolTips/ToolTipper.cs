﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipper : MonoBehaviour
{
    private Transform t_cam;
    [SerializeField] Text ta_canvasText;
    [SerializeField] GameObject[] goA_buttonPrompts = new GameObject[0];
    private RaycastHit hit;
    private PlayerInputManager pim;
    [SerializeField] private LayerMask lm_mask;
    private bool b_shouldShow = true;
    [SerializeField] private GameObject go_hasHelp;


    private void Start()
    {
        for (int i = 0; i < goA_buttonPrompts.Length; i++)
            goA_buttonPrompts[i].SetActive(false);
    }

    // Update is called once per frame

    void Update()
    {
        if (t_cam == null)
        {
            t_cam = NetworkedPlayer.x?.GetCamera()?.transform;
            pim = NetworkedPlayer.x?.GetPlayer()?.GetComponent<PlayerInputManager>();

            return;
        }
        if (b_shouldShow)
            if (Physics.Raycast(t_cam.position, t_cam.forward, out hit, 10, lm_mask, QueryTriggerInteraction.Ignore))
            {
                ToolTip _tt_ = hit.collider.transform.GetComponent<ToolTip>();

                if (_tt_)
                {
                    if (_tt_.b_hostOnly && pim.GetID() > 0)
                        return;
                    if (_tt_.transform.root == pim.transform) //Ignores self
                        return;

                    ta_canvasText.text = _tt_.Tip;
                    goA_buttonPrompts[_tt_.i_buttonSpriteIndex].SetActive(true);

                    if (_tt_.b_hasHelpData)
                    {
                        go_hasHelp.SetActive(true);

                        if (Input.GetKeyDown(KeyCode.H))
                            HUDController.x.ShowHelpText(_tt_.HelpText);
                    }
                    else
                        go_hasHelp.SetActive(false);
                }
                else
                {
                    ta_canvasText.text = "";
                    for (int i = 0; i < goA_buttonPrompts.Length; i++)
                        goA_buttonPrompts[i].SetActive(false);

                    go_hasHelp.SetActive(false);
                }
            }
            else
            {
                ta_canvasText.text = "";
                for (int i = 0; i < goA_buttonPrompts.Length; i++)
                    goA_buttonPrompts[i].SetActive(false);

                go_hasHelp.SetActive(false);
            }
    }

    internal void StartShowing()
    {
        b_shouldShow = true;
    }

    internal void StopShowing()
    {
        b_shouldShow = false;
        ta_canvasText.text = "";
        for (int i = 0; i < goA_buttonPrompts.Length; i++)
            goA_buttonPrompts[i].SetActive(false);
    }
}