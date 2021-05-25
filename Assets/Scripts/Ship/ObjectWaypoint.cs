using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWaypoint : MonoBehaviour
{

    [SerializeField] private GameObject go_marker;
    [SerializeField] private float f_yOnScreenOffset;
    [SerializeField] private string nameText;

    private bool run;
    private GameObject liveMarker;
    private Camera cam;
    private Transform player;
    private Transform t_hudCanvas;
    private RectTransform rt_on;
    private RectTransform canRect;

    public void TurnOn()
    {
        cam = NetworkedPlayer.x.GetCamera();
        player = NetworkedPlayer.x.GetPlayer();
        run = true;
        liveMarker = Instantiate(go_marker, Vector3.zero, Quaternion.identity);
        t_hudCanvas = HUDController.x.GetHudCanvasTransform();
        canRect = t_hudCanvas.GetComponent<RectTransform>();
    }

    public void TurnOff()
    {
        run = false;
        Destroy(liveMarker);
    }

    private void Update()
    {
        if (!run)
            return;


        Vector2 screenPos = cam.WorldToScreenPoint(transform.position + (f_yOnScreenOffset * Vector3.up));
        float w = Screen.width;
        float h = Screen.height;

        screenPos.x /= w;
        screenPos.y /= h;

        float dot = Vector3.Dot(cam.transform.forward, transform.position - cam.transform.position);

        if (dot < 0)
        {
            rt_on.gameObject.SetActive(false);

            if (screenPos.x < 0.5f)
            {
                screenPos.x = 1;
            }
            else
            {
                screenPos.x = 0;
            }
        }

        screenPos.x = Mathf.Clamp(screenPos.x, 0, 1);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, 1);

        rt_on.anchoredPosition = new Vector2(canRect.rect.width * screenPos.x, canRect.rect.height * screenPos.y);



    }

}
