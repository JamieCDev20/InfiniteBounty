using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private string s_toolTip;
    [SerializeField] internal Sprite buttonSprite;
    [SerializeField] internal bool b_hostOnly;

    public string Tip { get { return s_toolTip; } }
}
