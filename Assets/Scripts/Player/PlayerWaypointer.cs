using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWaypointer : MonoBehaviour
{

    [SerializeField] private GameObject go_onScreenMarker;
    [SerializeField] private GameObject go_offScreenMarker;
    [SerializeField] private float f_xRadius;
    [SerializeField] private float f_yRadius;
    [SerializeField] private float f_yOnScreenOffset;
    [SerializeField] private float f_maxDistance;

    private int i_scrWidth;
    private int i_scrHeight;

    private Camera cam;

    private Transform t_targetPlayer;
    private Transform t_hudCanvas;

    private GameObject go_onMarker;
    private RectTransform rt_on;

    private GameObject go_offMarker;
    private RectTransform rt_off;

    void Start()
    {
        t_targetPlayer = NetworkedPlayer.x.GetPlayer();
        t_hudCanvas = HUDController.x.GetHudCanvasTransform();

        go_onMarker = Instantiate(go_onScreenMarker, t_hudCanvas);
        rt_on = go_onMarker.GetComponent<RectTransform>();
        go_offMarker = Instantiate(go_offScreenMarker, t_hudCanvas);
        rt_off = go_offMarker.GetComponent<RectTransform>();

        cam = NetworkedPlayer.x.GetCamera();

        i_scrWidth = Screen.width;
        i_scrHeight = Screen.height;

    }

    void Update()
    {
        if ((transform.position - t_targetPlayer.position).sqrMagnitude > (f_maxDistance * f_maxDistance))
        {
            Vector2 targetPos = cam.WorldToScreenPoint(transform.position);

            if(targetPos.x > i_scrWidth || targetPos.x < 0)
            {
                go_onMarker.SetActive(false);
                go_offMarker.SetActive(true);
            }
            else
            {
                go_onMarker.SetActive(true);
                go_offMarker.SetActive(false);
            }

            targetPos.x = Mathf.Clamp(targetPos.x, f_xRadius, i_scrWidth - f_xRadius);
            targetPos.y = Mathf.Clamp(targetPos.y, f_yRadius, i_scrHeight - f_yRadius);

            rt_on.position = targetPos;
            rt_off.position = targetPos;

        }
        else
        {
            go_onMarker.SetActive(false);
            go_offMarker.SetActive(false);
        }
    }
}
