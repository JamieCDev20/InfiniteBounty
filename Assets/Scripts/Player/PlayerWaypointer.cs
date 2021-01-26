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

        rt_on.anchorMin = Vector2.zero;
        rt_off.anchorMin = Vector2.zero;

        cam = NetworkedPlayer.x.GetCamera();

    }

    void Update()
    {
        PositionWaypoint();
    }

    private void PositionWaypoint()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
        float w = Screen.width;
        float h = Screen.height;
        //screenPos.x /= t_hudCanvas.GetComponent<RectTransform>().rect.width;
        //screenPos.y /= t_hudCanvas.GetComponent<RectTransform>().rect.height;
        screenPos.x /= w;
        screenPos.y /= h;

        rt_on.anchoredPosition = new Vector2(w * screenPos.x, h * screenPos.y);
        rt_off.anchoredPosition = new Vector2((screenPos.x > 0.5 ? w - f_xRadius : f_xRadius), h * screenPos.y);

        if(Vector3.Dot(cam.transform.forward, transform.position - cam.transform.position) < 0)
        {
            if(screenPos.x < 0.5f)
                rt_off.anchoredPosition = new Vector2(w - f_xRadius, h * screenPos.y);
            else
                rt_off.anchoredPosition = new Vector2(f_xRadius, h * screenPos.y);
            screenPos.x = -1;
        }
        if(screenPos.x < 0.01f || screenPos.x > 0.99f)
        {
            rt_on.gameObject.SetActive(false);
            rt_off.gameObject.SetActive(true);
        }
        else
        {
            rt_on.gameObject.SetActive(true);
            rt_off.gameObject.SetActive(false);

        }

    }

}
