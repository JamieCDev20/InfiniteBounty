using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScroller : MonoBehaviour
{
    [SerializeField] private float f_scrollSpeed;

    void Update()
    {
        transform.localPosition += Vector3.up * Time.deltaTime * f_scrollSpeed;
    }
}
