using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWaypointer : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject go_onScreenMarker;
    [SerializeField] private GameObject go_offScreenMarker;
    [SerializeField] private float f_xRadius;
    [SerializeField] private float f_yOnScreenOffset;
    [SerializeField] private float f_displayRange = 10;

    private int i_scrWidth;
    private int i_scrHeight;
    private float curRadius;

    private Camera cam;
    private RectTransform canRect;

    private Transform t_targetPlayer;
    private Transform t_hudCanvas;

    private GameObject go_onMarker;
    private RectTransform rt_on;

    private GameObject go_offMarker;
    private RectTransform rt_off;
    private string playerName = "Samuel L Jackson";
    private Text onText;
    private Text offText;

    private bool run = true;

    private void Update()
    {
        if (!run)
            return;
        PositionWaypoint();
    }

    public void SetNames(string _name)
    {
        StartCoroutine(WhyDoIHaveToDoThis(_name));
    }

    IEnumerator WhyDoIHaveToDoThis(string _name)
    {

        yield return new WaitForSeconds(0.5f);

        t_targetPlayer = NetworkedPlayer.x.GetPlayer();
        if (t_targetPlayer == transform)
        {
            run = false;
        }
        else
        {
            t_hudCanvas = HUDController.x.GetHudCanvasTransform();

            go_onMarker = Instantiate(go_onScreenMarker, t_hudCanvas);
            rt_on = go_onMarker.GetComponent<RectTransform>();
            go_offMarker = Instantiate(go_offScreenMarker, t_hudCanvas);
            rt_off = go_offMarker.GetComponent<RectTransform>();

            rt_on.anchorMin = Vector2.zero;
            rt_off.anchorMin = Vector2.zero;

            cam = NetworkedPlayer.x.GetCamera();

            canRect = t_hudCanvas.GetComponent<RectTransform>();
            curRadius = f_xRadius;
            onText = go_onMarker.transform.GetChild(0).GetComponent<Text>();
            offText = go_offMarker.transform.GetChild(0).GetComponent<Text>();

            playerName = _name;
            onText.text = playerName;
            offText.text = $"{playerName}   {playerName}";

        }

    }

    private void PositionWaypoint()
    {

        if ((transform.position - t_targetPlayer.position).magnitude < f_displayRange)
        {
            go_offMarker.SetActive(false);
            go_onMarker.SetActive(false);
            return;
        }

        Vector2 screenPos = cam.WorldToScreenPoint(transform.position + (f_yOnScreenOffset * Vector3.up));
        float w = Screen.width;
        float h = Screen.height;

        screenPos.x /= w;
        screenPos.y /= h;

        float dot = Vector3.Dot(cam.transform.forward, transform.position - cam.transform.position);

        if (dot < 0)
        {
            rt_on.gameObject.SetActive(false);
            rt_off.gameObject.SetActive(true);

            if (screenPos.x < 0.5f)
            {
                screenPos.x = 1;
            }
            else
            {
                screenPos.x = 0;
            }
        }

        if (screenPos.x < 0.01f || screenPos.x > 0.99f)
        {
            rt_on.gameObject.SetActive(false);
            rt_off.gameObject.SetActive(true);
        }
        else
        {
            rt_on.gameObject.SetActive(true);
            rt_off.gameObject.SetActive(false);

        }

        screenPos.x = Mathf.Clamp(screenPos.x, 0, 1);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, 1);

        rt_on.anchoredPosition = new Vector2(canRect.rect.width * screenPos.x, canRect.rect.height * screenPos.y);
        rt_off.anchoredPosition = new Vector2(canRect.rect.width * screenPos.x, canRect.rect.height * screenPos.y);



    }

    public void Remove()
    {
        if (run)
        {
            Destroy(go_offMarker);
            Destroy(go_onMarker);
        }

    }

    public void ForRemove()
    {
        foreach (PlayerWaypointer pw in FindObjectsOfType<PlayerWaypointer>())
        {
            pw.Remove();
        }
    }

    private void OnDisable()
    {
        Remove();
    }

    private void OnDestroy()
    {
        Remove();
    }

    public override void OnLeftRoom()
    {
        ForRemove();
    }

}
