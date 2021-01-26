using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreen : MonoBehaviour
{

    [SerializeField] private Canvas can;
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform onSc;
    [SerializeField] private RectTransform offSc;

    private RectTransform canvasRect;

    private void Start()
    {
        onSc.gameObject.SetActive(true);
        offSc.gameObject.SetActive(true);
        canvasRect = can.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
        screenPos.x /= Screen.width;
        screenPos.y /= Screen.height;

        onSc.anchoredPosition = new Vector2(canvasRect.rect.width * screenPos.x, canvasRect.rect.height * screenPos.y);
        offSc.anchoredPosition = new Vector2((screenPos.x > 0.5 ? canvasRect.rect.width - 50 : 50), canvasRect.rect.height * screenPos.y);

        if (Vector3.Dot(cam.transform.forward, transform.position - cam.transform.position) < 0)
        {
            if (screenPos.x < 0.5f)
            {
                offSc.anchoredPosition = new Vector2(canvasRect.rect.width - 50, canvasRect.rect.height * screenPos.y);
            }
            else
            {
                offSc.anchoredPosition = new Vector2(50, canvasRect.rect.height * screenPos.y);

            }
            screenPos.x = -1;
        }

        if (screenPos.x < 0.01 || screenPos.x > 0.99)
        {
            offSc.gameObject.SetActive(true);
            onSc.gameObject.SetActive(false);
        }
        else
        {
            offSc.gameObject.SetActive(false);
            onSc.gameObject.SetActive(true);
        }

    }

}
