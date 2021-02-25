using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipper : MonoBehaviour
{
    [SerializeField] Transform t_cam;
    [SerializeField] Text ta_canvasText;
    [SerializeField] GameObject[] goA_buttonPrompts = new GameObject[0];
    private RaycastHit hit;
    private PlayerInputManager pim;
    [SerializeField] private LayerMask lm_mask;
    private bool b_shouldShow = true;

    private void Start()
    {
        pim = GetComponent<CameraController>().pim_inputs;

        for (int i = 0; i < goA_buttonPrompts.Length; i++)
            goA_buttonPrompts[i].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (b_shouldShow)
            if (Physics.Raycast(t_cam.position, t_cam.forward, out hit, 10, lm_mask, QueryTriggerInteraction.Ignore))
            {
                ToolTip _tt_ = hit.transform.GetComponentInChildren<ToolTip>();

                if (_tt_)
                {
                    if (_tt_.b_hostOnly && pim.GetID() > 0)
                        return;
                    if (_tt_.transform.root == pim.transform) //Ignores self
                        return;

                    ta_canvasText.text = _tt_.Tip;
                    goA_buttonPrompts[_tt_.i_buttonSpriteIndex].SetActive(true);

                }
                else
                {
                    ta_canvasText.text = "";
                    for (int i = 0; i < goA_buttonPrompts.Length; i++)
                        goA_buttonPrompts[i].SetActive(false);
                }
            }
            else
            {
                ta_canvasText.text = "";
                for (int i = 0; i < goA_buttonPrompts.Length; i++)
                    goA_buttonPrompts[i].SetActive(false);
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