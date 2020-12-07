using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipper : MonoBehaviour
{
    [SerializeField] Transform t_cam;
    [SerializeField] Text ta_canvasText;
    [SerializeField] Image i_buttonPrompt;
    private RaycastHit hit;
    private PlayerInputManager pim;

    private void Start()
    {
        pim = GetComponent<CameraController>().pim_inputs;
        i_buttonPrompt.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(t_cam.position, t_cam.forward);
        if (Physics.Raycast(t_cam.position, t_cam.forward, out hit, 10.0f))
        {
            ToolTip _tt_ = hit.transform.GetComponent<ToolTip>();
            if (_tt_)
            {
                if ((_tt_.b_hostOnly && pim.GetID() > 0))
                    return;

                ta_canvasText.text = _tt_.Tip;
                i_buttonPrompt.sprite = _tt_.buttonSprite;
                i_buttonPrompt.color = Color.white;
            }
        }
        else
        {
            ta_canvasText.text = "";
            i_buttonPrompt.color = Color.clear;
        }
    }
}
