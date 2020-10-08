using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        MenuController.x.ClickedPlay();
        gameObject.SetActive(false);
    }
}
