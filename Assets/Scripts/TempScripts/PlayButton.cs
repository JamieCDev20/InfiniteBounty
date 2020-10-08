using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        MenuController.x.ClickedPlay();        
        transform.localScale = new Vector3(1, 0.5f, 1);
    }
}
