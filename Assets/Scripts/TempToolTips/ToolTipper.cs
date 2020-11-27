using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipper : MonoBehaviour
{
    [SerializeField] Transform t_cam;
    [SerializeField] Text ta_canvasText;
    private RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(t_cam.position, t_cam.forward);
        if(Physics.Raycast(t_cam.position, t_cam.forward, out hit, 10.0f))
        {
            if (hit.transform.GetComponent<ToolTip>())
                ta_canvasText.text = hit.transform.GetComponent<ToolTip>().Tip;
        }
        else
        {
            ta_canvasText.text = "";
        }
    }
}
